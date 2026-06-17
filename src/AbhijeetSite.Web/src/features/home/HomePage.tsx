import React, { useEffect, useState } from 'react';
import { ProfilePage } from '../profile/ProfilePage';
import { profileApi } from '../profile/profileApi';
import { Profile } from '../profile/types';

export const HomePage: React.FC = () => {
  const [profile, setProfile] = useState<Profile | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchProfile = async (): Promise<void> => {
    try {
      setIsLoading(true);
      setError(null);
      const loadedProfile: Profile = await profileApi.getProfile();
      setProfile(loadedProfile);
    } catch (errorValue) {
      console.error(errorValue);
      setError(getProfileLoadError(errorValue));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void fetchProfile();
  }, []);

  if (isLoading) {
    return <LoadingScreen />;
  }

  if (error !== null || profile === null) {
    return <ErrorScreen error={error ?? 'Profile data was not returned.'} onRetry={fetchProfile} />;
  }

  return <ProfilePage profile={profile} />;
};

const LoadingScreen: React.FC = () => {
  return (
    <main className="status-screen">
      <div className="loading-panel">
        <span className="loading-mark">AH</span>
        <p>Loading profile</p>
      </div>
    </main>
  );
};

interface ErrorScreenProps {
  error: string;
  onRetry: () => Promise<void>;
}

const ErrorScreen: React.FC<ErrorScreenProps> = ({ error, onRetry }) => {
  const retry = (): void => {
    void onRetry();
  };

  return (
    <main className="status-screen">
      <div className="error-panel">
        <h1>Profile unavailable</h1>
        <p>{error}</p>
        <button type="button" onClick={retry}>Retry</button>
      </div>
    </main>
  );
};

const getProfileLoadError = (errorValue: unknown): string => {
  if (errorValue instanceof Error) {
    return `Failed to load profile: ${errorValue.message}`;
  }

  return 'Failed to load profile due to an unknown client error.';
};
