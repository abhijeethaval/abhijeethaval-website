import React from 'react';
import { AboutSection } from './components/AboutSection';
import { EducationSection } from './components/EducationSection';
import { ExperienceTimeline } from './components/ExperienceTimeline';
import { HomeHero } from './components/HomeHero';
import { Profile } from './types';

interface ProfilePageProps {
  profile: Profile;
}

export const ProfilePage: React.FC<ProfilePageProps> = ({ profile }) => {
  return (
    <>
      <SiteHeader />
      <HomeHero profile={profile} />
      <AboutSection profile={profile} />
      <ExperienceTimeline experiences={profile.experiences} />
      <EducationSection educations={profile.educations} />
    </>
  );
};

const SiteHeader: React.FC = () => {
  return (
    <header className="site-header">
      <a className="site-mark" href="#home">AH</a>
      <nav aria-label="Primary navigation">
        <a href="#profile">Profile</a>
        <a href="#experience">Experience</a>
        <a href="#education">Education</a>
      </nav>
    </header>
  );
};
