import React, { useEffect, useState } from 'react';
import { authApi } from './authApi';
import type { AuthenticatedUser, AuthSession, CurrentUserResponse } from './types';

const loadingSession: AuthSession = { status: 'loading' };
const anonymousSession: AuthSession = { status: 'anonymous' };
const errorSession: AuthSession = { status: 'error' };

export const AuthStatus: React.FC = () => {
  const [session, setSession] = useState<AuthSession>(loadingSession);

  useEffect(() => {
    let isActive = true;

    const loadCurrentUser = async (): Promise<void> => {
      try {
        const response: CurrentUserResponse = await authApi.getCurrentUser();
        if (isActive) {
          setSession(toAuthSession(response));
        }
      } catch {
        if (isActive) {
          setSession(errorSession);
        }
      }
    };

    void loadCurrentUser();

    return () => {
      isActive = false;
    };
  }, []);

  if (session.status === 'authenticated') {
    return <SignedInStatus user={session.user} onLogout={setSession} />;
  }

  return <SignedOutStatus isLoading={session.status === 'loading'} />;
};

interface SignedInStatusProps {
  user: AuthenticatedUser;
  onLogout: React.Dispatch<React.SetStateAction<AuthSession>>;
}

const SignedInStatus: React.FC<SignedInStatusProps> = ({ user, onLogout }) => {
  const handleLogout = async (): Promise<void> => {
    onLogout(loadingSession);

    try {
      await authApi.logout();
      onLogout(anonymousSession);
    } catch {
      onLogout(errorSession);
    }
  };

  return (
    <div className="auth-status" aria-label="Signed in user">
      <span className="auth-initial" aria-hidden="true">{getInitial(user.displayName)}</span>
      <span className="auth-user-name">{user.displayName}</span>
      {user.isAdmin ? <a className="auth-button" href="/admin/articles">Admin</a> : null}
      <button className="auth-button" type="button" onClick={handleLogout}>Sign out</button>
    </div>
  );
};

interface SignedOutStatusProps {
  isLoading: boolean;
}

const SignedOutStatus: React.FC<SignedOutStatusProps> = ({ isLoading }) => {
  return (
    <a
      className="auth-button"
      href={authApi.getGoogleLoginUrl()}
      aria-disabled={isLoading}
    >
      {isLoading ? 'Checking' : 'Sign in'}
    </a>
  );
};

const toAuthSession = (response: CurrentUserResponse): AuthSession => {
  if (response.isAuthenticated && response.user !== null) {
    return { status: 'authenticated', user: response.user };
  }

  return anonymousSession;
};

const getInitial = (displayName: string): string => {
  return displayName.trim().slice(0, 1).toUpperCase();
};
