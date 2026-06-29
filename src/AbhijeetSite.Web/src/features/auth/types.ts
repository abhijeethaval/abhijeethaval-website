export interface AuthenticatedUser {
  id: string;
  displayName: string;
  email: string;
  avatarUrl: string | null;
  isAdmin: boolean;
}

export interface CurrentUserResponse {
  isAuthenticated: boolean;
  user: AuthenticatedUser | null;
}

export type AuthSession =
  | { status: 'loading' }
  | { status: 'anonymous' }
  | { status: 'authenticated'; user: AuthenticatedUser }
  | { status: 'error' };
