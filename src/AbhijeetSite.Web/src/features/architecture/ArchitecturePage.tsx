import React from 'react';
import { SiteHeader } from '../../shared/navigation/SiteHeader';

interface SystemNode {
  label: string;
  detail: string;
}

interface ArchitectureDecision {
  title: string;
  decision: string;
  tradeoff: string;
}

const SYSTEM_NODES: ReadonlyArray<SystemNode> = [
  { label: 'React web app', detail: 'Public profile and architecture UI served by Nginx.' },
  { label: 'ASP.NET Core API', detail: 'Minimal APIs expose profile data and future publishing workflows.' },
  { label: '.NET Aspire', detail: 'Local orchestration, service defaults, health checks, and telemetry wiring.' },
  { label: 'PostgreSQL', detail: 'Durable identity, article, comment, and publishing state.' },
  { label: 'Azure Container Apps', detail: 'Public web container proxies to an internal API container.' },
];

const MODULES: ReadonlyArray<SystemNode> = [
  { label: 'Profile', detail: 'Curated professional content shared by home and profile views.' },
  { label: 'Identity', detail: 'External login, users, sessions, and admin authorization.' },
  { label: 'Articles', detail: 'Draft source, validation, publishing transitions, and public reads.' },
  { label: 'Comments', detail: 'Authenticated submission, moderation state, and approved public output.' },
];

const DECISIONS: ReadonlyArray<ArchitectureDecision> = [
  {
    title: 'Modular monolith',
    decision: 'Keep deployment simple while making feature boundaries visible in code.',
    tradeoff: 'Avoids distributed-system overhead until behavior demands it.',
  },
  {
    title: 'Published read model',
    decision: 'Public article reads come from render-ready published records, not draft MDX.',
    tradeoff: 'Separates authoring risk from reader traffic and page rendering.',
  },
  {
    title: 'API-owned auth',
    decision: 'OAuth callbacks, cookies, and provider secrets stay inside the API boundary.',
    tradeoff: 'React stays simpler and never handles provider access tokens.',
  },
  {
    title: 'Relative API routing',
    decision: 'The browser calls `/api`; Vite and Nginx decide the upstream target.',
    tradeoff: 'Local and production routing stay aligned without exposing API host config.',
  },
];

const ROADMAP_ITEMS: ReadonlyArray<string> = [
  'External login with Google and LinkedIn',
  'Durable MDX article drafting and preview',
  'Draft-to-published article pipeline',
  'Authenticated comments with moderation',
];

export const ArchitecturePage: React.FC = () => {
  return (
    <>
      <SiteHeader activeSection="architecture" />
      <main>
        <ArchitectureHero />
        <RuntimeFlowSection />
        <ModuleBoundarySection />
        <DecisionSection />
        <RoadmapSection />
      </main>
    </>
  );
};

const ArchitectureHero: React.FC = () => {
  return (
    <section className="architecture-hero section-band">
      <div className="section-shell architecture-hero-grid">
        <div className="architecture-hero-copy">
          <p className="eyebrow">Architecture</p>
          <h1>How this website is built, operated, and evolving.</h1>
          <p>
            This site is a production portfolio and a working product surface: a React
            frontend, ASP.NET Core API, Aspire orchestration, PostgreSQL persistence,
            and Azure Container Apps deployment.
          </p>
        </div>
        <SystemMap />
      </div>
    </section>
  );
};

const SystemMap: React.FC = () => {
  return (
    <div className="system-map" aria-label="Website architecture system map">
      {SYSTEM_NODES.map((node) => (
        <article className="system-node" key={node.label}>
          <span>{node.label}</span>
          <p>{node.detail}</p>
        </article>
      ))}
    </div>
  );
};

const RuntimeFlowSection: React.FC = () => {
  return (
    <section className="architecture-section section-band">
      <div className="section-shell architecture-two-column">
        <SectionIntro
          eyebrow="Runtime Flow"
          title="A public web container fronts an internal API."
        />
        <ol className="runtime-flow">
          <li>Browser requests the React app from the public Web container.</li>
          <li>Nginx serves static assets and proxies `/api` requests.</li>
          <li>The internal ASP.NET Core API resolves profile and publishing data.</li>
          <li>PostgreSQL persists identity, article, comment, and publishing state.</li>
        </ol>
      </div>
    </section>
  );
};

const ModuleBoundarySection: React.FC = () => {
  return (
    <section className="architecture-section architecture-section-muted section-band">
      <div className="section-shell architecture-two-column">
        <SectionIntro
          eyebrow="Module Boundaries"
          title="Feature slices keep product behavior owned by the right module."
        />
        <div className="module-grid">
          {MODULES.map((module) => (
            <article className="module-card" key={module.label}>
              <h3>{module.label}</h3>
              <p>{module.detail}</p>
            </article>
          ))}
        </div>
      </div>
    </section>
  );
};

const DecisionSection: React.FC = () => {
  return (
    <section className="architecture-section section-band">
      <div className="section-shell">
        <SectionIntro
          eyebrow="Decisions"
          title="The important tradeoffs are explicit and testable."
        />
        <div className="decision-grid">
          {DECISIONS.map((decision) => (
            <DecisionCard decision={decision} key={decision.title} />
          ))}
        </div>
      </div>
    </section>
  );
};

interface DecisionCardProps {
  decision: ArchitectureDecision;
}

const DecisionCard: React.FC<DecisionCardProps> = ({ decision }) => {
  return (
    <article className="decision-card">
      <h3>{decision.title}</h3>
      <p>{decision.decision}</p>
      <span>{decision.tradeoff}</span>
    </article>
  );
};

const RoadmapSection: React.FC = () => {
  return (
    <section className="architecture-section architecture-section-muted section-band">
      <div className="section-shell architecture-two-column">
        <SectionIntro
          eyebrow="Roadmap"
          title="The next slices turn the portfolio into a publishing platform."
        />
        <ul className="roadmap-list">
          {ROADMAP_ITEMS.map((item) => (
            <li key={item}>{item}</li>
          ))}
        </ul>
      </div>
    </section>
  );
};

interface SectionIntroProps {
  eyebrow: string;
  title: string;
}

const SectionIntro: React.FC<SectionIntroProps> = ({ eyebrow, title }) => {
  return (
    <div className="section-heading architecture-heading">
      <p className="eyebrow">{eyebrow}</p>
      <h2>{title}</h2>
    </div>
  );
};
