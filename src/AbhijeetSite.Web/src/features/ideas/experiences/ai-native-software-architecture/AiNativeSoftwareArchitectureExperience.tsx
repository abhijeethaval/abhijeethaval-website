import React from 'react';
import { SiteHeader } from '../../../../shared/navigation/SiteHeader';
import { ArchitectureLensSection } from './ArchitectureLensSection';
import { BoundaryLabSection } from './BoundaryLabSection';
import { NewPossibilitiesSection } from './NewPossibilitiesSection';
import './aiNativeSoftwareArchitecture.css';

export const AiNativeSoftwareArchitectureExperience: React.FC = () => {
  return (
    <>
      <SiteHeader activeSection="ideas" />
      <main className="interactive-thesis-page">
        <ArchitectureLensSection />
        <NewPossibilitiesSection />
        <ThesisBridge />
        <BoundaryLabSection />
      </main>
    </>
  );
};

const ThesisBridge: React.FC = () => {
  return (
    <section className="thesis-bridge">
      <div>
        <span>The architecture consequence</span>
        <strong>Now place the boundary yourself.</strong>
      </div>
      <p>
        Let the agent interpret the situation. Let the domain model decide what is valid,
        permitted, and committed. The exercise below makes that distinction concrete.
      </p>
    </section>
  );
};
