import React from 'react';
import { Profile } from '../types';

interface AboutSectionProps {
  profile: Profile;
}

const VISIBLE_EXPERTISE_COUNT = 10;

export const AboutSection: React.FC<AboutSectionProps> = ({ profile }) => {
  return (
    <section id="profile" className="profile-overview section-band">
      <div className="section-shell two-column">
        <div>
          <p className="eyebrow">Profile</p>
          <h2>Enterprise AI architecture, from strategy to production systems.</h2>
        </div>
        <div className="about-copy">
          {profile.about.map((paragraph) => (
            <p key={paragraph}>{paragraph}</p>
          ))}
          <ExpertiseList expertise={profile.expertise} />
        </div>
      </div>
    </section>
  );
};

const ExpertiseList: React.FC<{ expertise: readonly string[] }> = ({ expertise }) => {
  const visibleExpertise = expertise.slice(0, VISIBLE_EXPERTISE_COUNT);

  return (
    <div className="expertise-list" aria-label="Expertise">
      {visibleExpertise.map((item) => (
        <span key={item}>{item}</span>
      ))}
    </div>
  );
};
