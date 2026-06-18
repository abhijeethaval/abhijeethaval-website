import React from 'react';
import { AboutSection } from './components/AboutSection';
import { EducationSection } from './components/EducationSection';
import { ExperienceTimeline } from './components/ExperienceTimeline';
import { HomeHero } from './components/HomeHero';
import { Profile } from './types';
import { SiteHeader } from '../../shared/navigation/SiteHeader';

interface ProfilePageProps {
  profile: Profile;
}

export const ProfilePage: React.FC<ProfilePageProps> = ({ profile }) => {
  return (
    <>
      <SiteHeader activeSection="profile" />
      <HomeHero profile={profile} />
      <AboutSection profile={profile} />
      <ExperienceTimeline experiences={profile.experiences} />
      <EducationSection educations={profile.educations} />
    </>
  );
};
