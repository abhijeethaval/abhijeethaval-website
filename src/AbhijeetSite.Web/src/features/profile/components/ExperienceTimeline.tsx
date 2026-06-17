import React from 'react';
import { Experience, Role } from '../types';

interface ExperienceTimelineProps {
  experiences: readonly Experience[];
}

export const ExperienceTimeline: React.FC<ExperienceTimelineProps> = ({ experiences }) => {
  return (
    <section id="experience" className="experience-section section-band">
      <div className="section-shell">
        <div className="section-heading">
          <p className="eyebrow">Experience</p>
          <h2>Architecture work across AI, SaaS, finance, medical, and defense domains.</h2>
        </div>
        <div className="timeline">
          {experiences.map((experience) => (
            <CompanyExperience key={experience.company} experience={experience} />
          ))}
        </div>
      </div>
    </section>
  );
};

const CompanyExperience: React.FC<{ experience: Experience }> = ({ experience }) => {
  return (
    <article className="company-block">
      <div className="company-header">
        <h3>{experience.company}</h3>
        <span>{experience.location}</span>
      </div>
      <div className="role-list">
        {experience.roles.map((role) => (
          <RoleCard key={`${experience.company}-${role.title}`} role={role} />
        ))}
      </div>
    </article>
  );
};

const RoleCard: React.FC<{ role: Role }> = ({ role }) => {
  return (
    <div className="role-card">
      <div className="role-heading">
        <h4>{role.title}</h4>
        <span>{role.startDate} - {role.endDate}</span>
      </div>
      <p>{role.summary}</p>
      <AchievementList achievements={role.achievements} />
      <FocusAreas focusAreas={role.focusAreas} />
    </div>
  );
};

const AchievementList: React.FC<{ achievements: readonly string[] }> = ({ achievements }) => {
  return (
    <ul className="achievement-list">
      {achievements.map((achievement) => (
        <li key={achievement}>{achievement}</li>
      ))}
    </ul>
  );
};

const FocusAreas: React.FC<{ focusAreas: readonly string[] }> = ({ focusAreas }) => {
  return (
    <div className="focus-areas">
      {focusAreas.map((area) => (
        <span key={area}>{area}</span>
      ))}
    </div>
  );
};
