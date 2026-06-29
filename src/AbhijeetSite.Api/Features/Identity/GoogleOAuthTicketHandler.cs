using System.Net.Http.Headers;
using System.Text.Json;
using AbhijeetSite.Api.SharedKernel.Result;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace AbhijeetSite.Api.Features.Identity;

internal static class GoogleOAuthTicketHandler
{
    private const string EmailClaim = "email";
    private const string EmailVerifiedClaim = "email_verified";
    private const string NameClaim = "name";
    private const string PictureClaim = "picture";
    private const string SubjectClaim = "sub";

    public static async Task HandleCreatingTicketAsync(OAuthCreatingTicketContext context)
    {
        Result<JsonDocument> userInfoResult = await ReadUserInfoAsync(context);
        if (userInfoResult.IsFailure)
        {
            context.Fail(userInfoResult.Error?.Message ?? "Google user info could not be loaded.");
            return;
        }

        using JsonDocument userInfo = userInfoResult.Value;
        Result<ExternalLoginClaims> claimsResult = MapGoogleClaims(userInfo.RootElement);
        if (claimsResult.IsFailure)
        {
            context.Fail(claimsResult.Error?.Message ?? "Google returned incomplete identity claims.");
            return;
        }

        await SignInLocalUserAsync(context, claimsResult.Value);
    }

    private static async Task<Result<JsonDocument>> ReadUserInfoAsync(
        OAuthCreatingTicketContext context)
    {
        if (string.IsNullOrWhiteSpace(context.AccessToken))
        {
            return Result<JsonDocument>.Failure(IdentityErrors.MissingExternalClaim("access_token"));
        }

        using HttpRequestMessage request = new(HttpMethod.Get, context.Options.UserInformationEndpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", context.AccessToken);

        HttpResponseMessage response = await context.Backchannel.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            context.HttpContext.RequestAborted);

        return await ReadUserInfoResponseAsync(response, context.HttpContext.RequestAborted);
    }

    private static async Task<Result<JsonDocument>> ReadUserInfoResponseAsync(
        HttpResponseMessage response,
        CancellationToken cancellationToken)
    {
        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                return Result<JsonDocument>.Failure(IdentityErrors.MissingExternalClaim("userinfo"));
            }

            string payload = await response.Content.ReadAsStringAsync(cancellationToken);
            return Result<JsonDocument>.Success(JsonDocument.Parse(payload));
        }
    }

    private static Result<ExternalLoginClaims> MapGoogleClaims(JsonElement payload)
    {
        Result<bool> emailVerifiedResult = ReadRequiredBoolean(payload, EmailVerifiedClaim);
        if (emailVerifiedResult.IsFailure)
        {
            Error error = emailVerifiedResult.Error
                ?? throw new InvalidOperationException("Failed email verification result has no error.");
            return Result<ExternalLoginClaims>.Failure(error);
        }

        return ExternalLoginClaims.Create(
            ExternalLoginProvider.Google,
            ReadOptionalString(payload, SubjectClaim),
            ReadOptionalString(payload, NameClaim),
            ReadOptionalString(payload, EmailClaim),
            emailVerifiedResult.Value,
            ReadOptionalString(payload, PictureClaim));
    }

    private static string? ReadOptionalString(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out JsonElement property))
        {
            return null;
        }

        return property.ValueKind == JsonValueKind.String ? property.GetString() : null;
    }

    private static Result<bool> ReadRequiredBoolean(JsonElement payload, string propertyName)
    {
        if (!payload.TryGetProperty(propertyName, out JsonElement property))
        {
            return Result<bool>.Failure(IdentityErrors.MissingExternalClaim(propertyName));
        }

        bool isBoolean = property.ValueKind is JsonValueKind.True or JsonValueKind.False;
        return isBoolean
            ? Result<bool>.Success(property.GetBoolean())
            : Result<bool>.Failure(IdentityErrors.MissingExternalClaim(propertyName));
    }

    private static async Task SignInLocalUserAsync(
        OAuthCreatingTicketContext context,
        ExternalLoginClaims claims)
    {
        ExternalLoginUpsertHandler handler =
            context.HttpContext.RequestServices.GetRequiredService<ExternalLoginUpsertHandler>();

        Result<SignInUserResult> result = await handler.HandleAsync(
            claims,
            context.HttpContext.RequestAborted);

        if (result.IsFailure)
        {
            context.Fail(result.Error?.Message ?? "Local identity could not be created.");
            return;
        }

        context.Principal = IdentityPrincipalFactory.Create(result.Value);
    }
}
