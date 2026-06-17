import React from 'react';
import { Education } from '../types';

interface EducationSectionProps {
  educations: readonly Education[];
}

export const EducationSection: React.FC<EducationSectionProps> = ({ educations }) => {
  return (
    <section id="education" className="education-section section-band">
      <div className="section-shell two-column">
        <div>
          <p className="eyebrow">Education</p>
          <h2>Engineering foundation with early cross-functional interests.</h2>
        </div>
        <div className="education-list">
          {educations.map((education) => (
            <EducationCard key={education.institution} education={education} />
          ))}
        </div>
      </div>
    </section>
  );
};

const EducationCard: React.FC<{ education: Education }> = ({ education }) => {
  return (
    <article className="education-card">
      <span>{education.years}</span>
      <h3>{education.institution}</h3>
      <p>{education.credential}</p>
      <ActivityList activities={education.activities} />
    </article>
  );
};

const ActivityList: React.FC<{ activities: readonly string[] }> = ({ activities }) => {
  return (
    <div className="activity-list">
      {activities.map((activity) => (
        <span key={activity}>{activity}</span>
      ))}
    </div>
  );
};
