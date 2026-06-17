import React from 'react';
import { Profile } from '../types';

interface HomeHeroProps {
  profile: Profile;
}

const FEATURED_HIGHLIGHTS = [
  'Enterprise Agentic AI',
  'Governed tool use',
  'Human-in-the-loop workflows',
  'Distributed SaaS platforms',
] as const;

export const HomeHero: React.FC<HomeHeroProps> = ({ profile }) => {
  return (
    <section id="home" className="home-hero section-band">
      <div className="section-shell hero-grid">
        <div className="hero-copy">
          <p className="eyebrow">Principal Architect</p>
          <h1>{profile.name}</h1>
          <p className="hero-headline">{profile.headline}</p>
          <p className="hero-summary">{profile.summary}</p>
          <div className="hero-actions">
            <a className="primary-link" href="#profile">View full profile</a>
            <a className="secondary-link" href="#experience">Experience</a>
          </div>
        </div>
        <div className="hero-visual" aria-label="Profile highlights">
          <ProfileSignal />
          <HighlightList />
        </div>
      </div>
    </section>
  );
};

const ProfileSignal: React.FC = () => {
  return (
    <div className="profile-signal">
      <div className="monogram">AH</div>
      <div>
        <span className="signal-label">Current focus</span>
        <strong>Agentic AI platforms for enterprise workflows</strong>
      </div>
    </div>
  );
};

const HighlightList: React.FC = () => {
  return (
    <div className="highlight-list">
      {FEATURED_HIGHLIGHTS.map((highlight) => (
        <span key={highlight}>{highlight}</span>
      ))}
    </div>
  );
};
