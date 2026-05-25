# Azure Container Apps & GitHub Actions Deployment Guide

This guide provides step-by-step instructions to configure **100% cloud-native** container building and secure deployment to **Azure Container Apps (ACA)** using **GitHub Actions CI/CD**. 

By utilizing cloud-based compilation (`az acr build`), this architecture completely eliminates any dependency on a local Docker daemon and fully bypasses local network firewall/SSL connection reset blocks.

---

## Architectural Overview

```mermaid
flowchart TD
    %% Define styles and classes
    classDef developer fill:#2d3748,stroke:#4a5568,stroke-width:2px,color:#fff;
    classDef github fill:#181717,stroke:#24292e,stroke-width:2px,color:#fff;
    classDef azure fill:#0078d4,stroke:#005a9e,stroke-width:2px,color:#fff;
    classDef security fill:#00b7c3,stroke:#008b8b,stroke-width:2px,color:#fff;
    classDef storage fill:#f25f22,stroke:#d83b01,stroke-width:2px,color:#fff;
    classDef container fill:#5c2d91,stroke:#4b1f7a,stroke-width:2px,color:#fff;
    
    subgraph Local ["💻 Developer Machine"]
        Dev["🛠️ Developer Local"]:::developer
    end

    subgraph GitHubPlatform ["🐱 GitHub SaaS Platform"]
        Repo["📂 GitHub Repository"]:::github
        Runner["🤖 GitHub Actions Runner"]:::github
    end

    subgraph Azure ["☁️ Azure Cloud"]
        Entra["🔑 Entra ID App (OIDC)"]:::security
        ACR["📦 Azure Container Registry"]:::storage
        
        subgraph ACAEnv ["🛡️ Private Container Apps VNet Environment"]
            ACA_Web["🌐 Public Web App (Nginx)"]:::container
            ACA_Api["⚙️ Internal API App (.NET)"]:::container
        end
    end
    
    Client(["👤 Browser User"])

    %% Data Flow
    Dev -->|git push| Repo
    Repo -->|Trigger Job| Runner
    Runner -->|1. Authenticate OIDC| Entra
    Runner -->|2. az acr build API| ACR
    Runner -->|3. az acr build Web| ACR
    ACR -->|4. Deploy Revision| ACA_Web
    ACR -->|4. Deploy Revision| ACA_Api
    
    Client -->|5. HTTPS Port 443| ACA_Web
    ACA_Web -->|6. Nginx HTTP Reverse Proxy| ACA_Api
```


1. **Secure Identity (OIDC)**: GitHub Actions securely logs into Azure using **OpenID Connect (OIDC)**, meaning you do **not** need to store password keys or service principal secrets in GitHub.
2. **Cloud-Native Builds (`az acr build`)**: Source directories are packaged and compiled inside Azure Container Registry tasks, freeing local developer environments from running Docker or pulling heavy images.
3. **Internal Nginx Proxy**: The public frontend (`abhijeetsite-web`) reverse-proxies API requests (`/api/*`) internally over private HTTP to the backend C# API (`abhijeetsite-api`), utilizing a startup-injected `API_UPSTREAM` variable.

---

## Step 1: Create Azure Identity (App Registration)

To allow GitHub to log into your Azure subscription securely without passwords:

1. Open the [Azure Portal](https://portal.azure.com/) and go to **Microsoft Entra ID** (formerly Azure Active Directory).
2. In the left menu, select **App registrations** -> **New registration**.
   - **Name**: `github-actions-deploy`
   - **Supported account types**: Select the **first option**: `Accounts in this organizational directory only (Default Directory only - Single tenant)`.
   - Click **Register**.
3. Copy the following GUID values from the **Overview** page:
   - **Application (client) ID** (this will be `AZURE_CLIENT_ID`)
   - **Directory (tenant) ID** (this will be `AZURE_TENANT_ID`)

### Configure Federated Credentials (OIDC active trust)
This establishes the secure trust between Azure and your personal GitHub repository branch.

1. Inside your new App Registration page, click on **Certificates & secrets** in the left menu.
2. Select the **Federated credentials** tab, and click **Add credential**.
3. Fill out the form:
   - **Federated credential scenario**: `GitHub Actions deploying Azure resources`
   - **Organization**: Your personal GitHub username (e.g., `abhijeethaval`).
   - **Repository**: Your repository name (e.g., `abhijeethaval-website`).
   - **Entity type**: **Branch**
   - **Branch name**: `main`
   - **Name**: `github-main-branch`
4. Click **Add**.

---

## Step 2: Configure Azure Role Assignments (IAM)

Your GitHub identity needs permission to deploy resources inside your resource group and push to your container registry.

1. Navigate to your Resource Group **`rg-abhijeet-site`** in the Azure Portal.
2. Click on **Access control (IAM)** in the left menu, then click **Add** -> **Add role assignment**.
3. **Locate the generic Contributor role**:
   > [!IMPORTANT]
   > Azure groups powerful administrator roles into a separate tab to prevent accidental assignments.
   > - Click on the **`Privileged administrator roles`** tab at the top of the roles list.
   > - Select the plain **`Contributor`** role.
4. Click **Next**, and choose **Assign access to**: `User, group, or service principal`.
5. Click **+ Select members**, search for `github-actions-deploy` (your app registration), select it, and click **Select**.
6. Click **Review + assign**.
7. Navigate to your Container Registry **`acrabhijeetsite`** (or do it at the Resource Group level) and repeat the process to assign the **`AcrPush`** role to `github-actions-deploy` so it can push container images.

---

## Step 3: Configure GitHub Secrets & Variables

Navigate to your repository on **GitHub.com**, click **Settings** -> **Secrets and variables** -> **Actions**.

### 1. Add Secrets (Sensitive Connection IDs)
Under the **Secrets** tab, click **New repository secret** for:

| Secret Name | Value |
| :--- | :--- |
| **`AZURE_CLIENT_ID`** | The **Application (client) ID** copied in Step 1 |
| **`AZURE_TENANT_ID`** | The **Directory (tenant) ID** copied in Step 1 |
| **`AZURE_SUBSCRIPTION_ID`** | Your Azure Subscription ID (found on your Resource Group page) |

### 2. Add Variables (Non-sensitive resource settings)
Click the **Variables** tab (next to Secrets), and click **New repository variable** for:

| Variable Name | Value | Description |
| :--- | :--- | :--- |
| **`RESOURCE_GROUP`** | `rg-abhijeet-site` | The Azure Resource Group name |
| **`ACR_NAME`** | `acrabhijeetsite` | Your Container Registry name |
| **`ACR_LOGIN_SERVER`** | `acrabhijeetsite.azurecr.io` | Registry login server |
| **`WEB_APP`** | `abhijeetsite-web` | Image repository name for the Web App |

---

## Step 4: Configure Container Apps (In the Azure Portal)

To allow the frontend Web container to securely talk to the backend C# API internally over the private virtual network:

### 1. API App Ingress (`abhijeetsite-api`)
1. Go to your **`abhijeetsite-api`** Container App in the portal.
2. In the left menu, under **Settings**, click on **Ingress**.
3. Configure:
   - **Ingress**: `Enabled`
   - **Ingress traffic**: `Limited to Container Apps Environment` (Internal)
   - **Target Port**: **`8080`** (the port the C# API listens on internally)
   - **Transport**: `Auto`
   - **Insecure connections** (under Client Configuration): Check **`Allow insecure connections`** if you want to communicate internally over plain HTTP (`http://`). Otherwise, if unchecked (default), Azure will enforce HTTPS.
4. Click **Save**.
5. Copy the generated **Application URL** (FQDN) from the Overview page. E.g., `https://aca-abhijeet-site.internal.kindgrass-f8a936c7.centralindia.azurecontainerapps.io`

### 2. Web App Ingress (`abhijeetsite-web`)
1. Go to your **`abhijeetsite-web`** Container App in the portal.
2. In the left menu, click **Ingress**.
3. Configure:
   - **Ingress**: `Enabled`
   - **Ingress traffic**: `Accepting traffic from anywhere` (External/Public)
   - **Target Port**: **`80`** (the port the Nginx server listens on internally)
   - **Transport**: `Auto`
4. Click **Save**.

### 3. Inject the Upstream Variable in the Web App
> [!IMPORTANT]
> **Internal Ingress Protocol Match**:
> By default, Azure Container Apps internal ingress enforces HTTPS (`allowInsecure = false`). If you attempt to connect over plain HTTP (`http://`), Azure will issue a `301/307 Redirect` to `https://`, which causes Nginx to return a redirect to the browser that fails DNS resolution.
> 
> To resolve this, you have two choices:
> - **Option A (Recommended)**: Keep HTTPS enforced and configure `API_UPSTREAM` with the `https://` prefix (e.g., `https://aca-abhijeet-site.internal...`). Since Nginx in this project is already configured with robust SNI support (`proxy_ssl_server_name on;`), this works flawlessly.
> - **Option B**: Check **Allow insecure connections** in your API App's Ingress settings and configure `API_UPSTREAM` with the `http://` prefix (e.g., `http://aca-abhijeet-site.internal...`).
> 
> > [!WARNING]
> > **FQDN Environment Identifier vs. Environment Name**:
> > Do not use your environment name (e.g., `acaenv-abhijeet-site`) in the internal FQDN. You **must** use the unique environment domain identifier generated by Azure (e.g., `kindgrass-f8a936c7`), which you can copy directly from the Overview page of your API container app.
> > * **Incorrect**: `https://aca-abhijeet-site.internal.acaenv-abhijeet-site.centralindia.azurecontainerapps.io`
> > * **Correct**: `https://aca-abhijeet-site.internal.kindgrass-f8a936c7.centralindia.azurecontainerapps.io`

1. Inside **`abhijeetsite-web`**, go to **Containers** -> **Edit and deploy** (to create a new revision).
2. Click on the container image name to open the side configuration panel.
3. Under the **Environment variables** tab, add a new variable:
   - **Name**: `API_UPSTREAM`
   - **Value**: The internal FQDN of your API app. E.g. `https://aca-abhijeet-site.internal.kindgrass-f8a936c7.centralindia.azurecontainerapps.io` (Recommended Option A) or `http://aca-abhijeet-site.internal...` (Option B, requires "Allow insecure connections" to be checked on the API ingress).
4. Click **Save** -> **Create** to deploy.

---

## Step 5: Deploy via Git

Once the configuration is ready, simply commit your changes and push to `main`. This will trigger the cloud pipeline automatically:

```powershell
# 1. Stage the documentation and code changes
git add -A

# 2. Commit the changes
git commit -m "docs: add GitHub Actions and Container Apps deployment guide"

# 3. Push to your main branch
git push origin main
```

Navigate to your GitHub repository's **Actions** tab to watch the workflow build and publish both images. Once it completes successfully, you can trigger the deployment update in one of two ways:

#### Option A: Trigger the Deployment via GitHub Actions (Recommended)
1. Go to your repository's **Actions** tab.
2. Select the **Deploy Container Images to ACA** workflow from the left sidebar.
3. Click the **Run workflow** dropdown on the right.
4. Input the **Image Tag** (e.g., your git short SHA `5111d4e` or semantic version).
5. (Optional) Enter custom Container App names if you wish to override the default repository variables.
6. Click **Run workflow** to initiate the deployment.

#### Option B: Trigger the Deployment via Local PowerShell Script
Alternatively, trigger the deployment update manually using your local script:
```powershell
# Update your Container Apps to the new image tags
powershell -ExecutionPolicy Bypass -File .\deploy\update-container-apps.ps1 <your-git-short-sha-or-tag>
```

---

## 💡 Troubleshooting & Key Learnings (HTTPS Upstreams & SNI)

During the deployment of this architecture, a critical connection issue was identified and resolved regarding Nginx reverse-proxying to the API app in Azure.

### The Problem: SSL Handshake Resets (HTTP 502)
When the frontend web app (`aca-abhijeet-site-web`) attempted to proxy `/api/*` to the external HTTPS endpoint of the API app (`https://aca-abhijeet-site.kindgrass-f8a936c7.centralindia.azurecontainerapps.io`), Nginx threw the following error:
> `[error] peer closed connection in SSL handshake (104: Connection reset by peer) while SSL handshaking to upstream`

This resulted in an **HTTP 502 Bad Gateway** response to the client.

### Root Cause
1. **Wrong Host Header**: The Nginx configuration was setting `proxy_set_header Host $host;`, which forwarded the client's request host (`aca-abhijeet-site-web...`) to the API.
2. **Missing SNI (Server Name Indication)**: Nginx does not send SNI by default when handshaking with an HTTPS upstream.
3. Azure Container Apps' ingress controller terminates SSL/TLS at the environment boundary. When it received a connection request without SNI and with a mismatched `Host` header, the ingress load balancer immediately aborted the connection.

### The Solution: Robust Nginx Configuration
To successfully proxy requests to HTTPS upstreams in Azure (and modern cloud environments), the Nginx location block must be configured with:
1. `proxy_set_header Host $proxy_host;` — Sets the Host header to match the backend domain instead of forwarding the frontend host.
2. `proxy_ssl_server_name on;` — Explicitly enables SNI for the SSL handshake.
3. `proxy_ssl_name $proxy_host;` — Sends the correct server name during the handshake.

Updated `nginx.conf.template` block:
```nginx
        location /api/ {
            proxy_pass ${API_UPSTREAM}/api/;
            proxy_http_version 1.1;
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection 'upgrade';
            proxy_set_header Host $proxy_host;
            proxy_cache_bypass $http_upgrade;
            proxy_ssl_server_name on;
            proxy_ssl_name $proxy_host;
        }
```

---

## 🔍 Discovered Azure Infrastructure Details

These are the verified resource names and identifiers for this deployment, discovered via standard Azure CLI operations:

| Resource Type | Resource Name / Value | Description |
| :--- | :--- | :--- |
| **Resource Group** | `rg-abhijeet-site` | Deployment resource group |
| **Location / Region** | `centralindia` | Central India region |
| **Container Registry (ACR)** | `acrabhijeetsite` | Registry server: `acrabhijeetsite.azurecr.io` |
| **ACA Environment** | `acaenv-abhijeet-site` | Managed Container Apps environment |
| **Frontend Web App** | `aca-abhijeet-site-web` | Public Ingress FQDN: `aca-abhijeet-site-web.kindgrass-f8a936c7.centralindia.azurecontainerapps.io` |
| **Backend API App** | `aca-abhijeet-site` | Public Ingress FQDN: `aca-abhijeet-site.kindgrass-f8a936c7.centralindia.azurecontainerapps.io` |
| **Log Analytics Workspace** | `workspacergabhijeetsite8402` | Workspace Customer ID: `d738c1e6-e2da-4de8-8352-276afaaf4022` |


---

## 🌐 Step 6: Custom Domain Configuration (GoDaddy & SSL)

This section details how to bind your custom domain **`abhijeethaval.com`** (purchased via GoDaddy) to the frontend Container App **`aca-abhijeet-site-web`** using **Azure Managed Certificates** (free, auto-renewing SSL).

### 📋 DNS Record Mapping Reference

To support both the root/apex domain (`abhijeethaval.com`) and the standard `www` subdomain, you must configure two sets of records in your GoDaddy DNS Zone Editor.

| Host/Name | Type | Value | Purpose |
| :--- | :--- | :--- | :--- |
| **`@`** | `A` | **`<Container-App-Environment-Static-IP>`** | Routes `abhijeethaval.com` to Azure |
| **`asuid`** | `TXT` | **`<Azure-Verification-Code>`** | Verifies ownership of apex domain |
| **`www`** | `CNAME` | `aca-abhijeet-site-web.kindgrass-f8a936c7.centralindia.azurecontainerapps.io` | Routes `www.abhijeethaval.com` to Azure |
| **`asuid.www`** | `TXT` | **`<Azure-Verification-Code>`** | Verifies ownership of `www` subdomain |

---

### 🚀 Implementation Guide

#### Phase A: Retrieve Details from Azure Portal
1. Open the [Azure Portal](https://portal.azure.com/).
2. Navigate to your **`aca-abhijeet-site-web`** Container App in resource group `rg-abhijeet-site`.
3. In the left-hand menu, under **Settings**, click on **Custom domains**.
4. Click **Add custom domain**.
5. Copy the **Domain verification code** (e.g. `23F5D6B...`). This is a single code used for all TXT validation records.
6. Copy the **Static IP address** displayed for the environment.

#### Phase B: Add Records in GoDaddy
1. Log in to your **GoDaddy Portfolio** and select your domain **`abhijeethaval.com`**.
2. Click **DNS** to open the **DNS Records** page.
3. Click **Add New Record** to add the following **4 records**:

> [!WARNING]
> If GoDaddy already has a default `CNAME` with name `www` pointing to `@` or a parking page, click **Edit** on that record and change its value, or delete it before creating the new `CNAME` record to avoid DNS record collisions.

##### 1. Root Domain Verification
*   **Type**: `TXT`
*   **Name**: `asuid`
*   **Value**: *[Your Copied Azure Verification Code]*
*   **TTL**: `Default` (or 1 Hour)

##### 2. Root Domain A Record
*   **Type**: `A`
*   **Name**: `@`
*   **Value**: *[Your Copied Static IP Address]*
*   **TTL**: `Default` (or 1 Hour)

##### 3. Subdomain CNAME Record
*   **Type**: `CNAME`
*   **Name**: `www`
*   **Value**: `aca-abhijeet-site-web.kindgrass-f8a936c7.centralindia.azurecontainerapps.io`
*   **TTL**: `Default` (or 1 Hour)

##### 4. Subdomain Verification
*   **Type**: `TXT`
*   **Name**: `asuid.www`
*   **Value**: *[Your Copied Azure Verification Code]*
*   **TTL**: `Default` (or 1 Hour)

---

#### Phase C: Validate & Bind in Azure Portal
Once saved in GoDaddy, return to the **Custom domains** tab in the Azure Portal:

##### 1. Bind Apex Domain (`abhijeethaval.com`)
1. Click **Add custom domain**.
2. Enter **`abhijeethaval.com`**.
3. Under **TLS/SSL certificate**, select **Managed certificate** (Azure generates and manages a free SSL certificate for you).
4. Click **Validate**. *(If validation fails, wait 2–5 minutes for GoDaddy's DNS propagation and try again).*
5. Once a green checkmark appears next to validation, click **Add**.

##### 2. Bind Subdomain (`www.abhijeethaval.com`)
1. Click **Add custom domain** again.
2. Enter **`www.abhijeethaval.com`**.
3. Under **TLS/SSL certificate**, select **Managed certificate**.
4. Click **Validate**.
5. Click **Add**.

> [!NOTE]
> Azure will automatically provision a free SSL certificate for both domains. This process can take between **5 to 15 minutes** to fully complete. Once completed, the domain status in Azure will display as **Secured**, and your site will be accessible via standard HTTPS!
