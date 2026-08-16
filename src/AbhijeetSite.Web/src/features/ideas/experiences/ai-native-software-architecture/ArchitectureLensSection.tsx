import React, { useState } from 'react';
import { SiteBackLink } from '../../../../shared/navigation/SiteBackLink';

type ArchitectureLens = 'ai-component' | 'enterprise-system' | 'enterprise-landscape';

interface LensContent {
  readonly description: string;
  readonly example: string;
  readonly formulaLeft: string;
  readonly formulaRight: string;
  readonly plainLanguage: string;
  readonly title: string;
}

interface LensOption {
  readonly id: ArchitectureLens;
  readonly label: string;
}

const LENS_OPTIONS: ReadonlyArray<LensOption> = [
  { id: 'ai-component', label: 'AI component' },
  { id: 'enterprise-system', label: 'Enterprise system' },
  { id: 'enterprise-landscape', label: 'Enterprise landscape' },
];

const LENS_CONTENT: Readonly<Record<ArchitectureLens, LensContent>> = {
  'ai-component': {
    title: 'Probabilistic core. Deterministic shell.',
    description: 'At component scope, model reasoning sits at the centre while '
      + 'permissions, tools, approvals, and retries constrain its execution.',
    plainLanguage: 'The model can explore possible answers. Ordinary code controls '
      + 'what it can access, which actions it can take, and when a human must approve.',
    example: 'A support agent may propose a refund, but its harness limits the tools '
      + 'it can call and requires approval before money moves.',
    formulaLeft: 'Model reasoning',
    formulaRight: 'Deterministic controls',
  },
  'enterprise-system': {
    title: 'Hard core. Soft shell.',
    description: 'At system scope, business authority sits in the domain core while '
      + 'adaptive intelligence interprets intent and navigates its capabilities.',
    plainLanguage: 'AI handles the messy human request. The business system still '
      + 'decides what is valid, permitted, and permanently recorded.',
    example: 'When a customer asks for a refund, the agent understands the request. '
      + 'The order system checks eligibility and commits the result.',
    formulaLeft: 'Domain authority',
    formulaRight: 'Adaptive intelligence',
  },
  'enterprise-landscape': {
    title: 'Pervasive intelligence. Distributed authority.',
    description: 'At enterprise scope, user-delegated agents reason across multiple '
      + 'systems. Each deterministic core still owns its facts, rules, and state.',
    plainLanguage: 'One agent can help a user across several applications without '
      + 'turning those applications into one unreliable source of truth.',
    example: 'To resolve a delayed shipment, an agent may coordinate CRM, ordering, '
      + 'payments, and operations. Each system remains authoritative for its own data.',
    formulaLeft: 'Delegated agency',
    formulaRight: 'Authoritative systems',
  },
};

export const ArchitectureLensSection: React.FC = () => {
  const [lens, setLens] = useState<ArchitectureLens>('enterprise-system');
  const content: LensContent = LENS_CONTENT[lens];

  return (
    <section className="lens-section">
      <LensHero />
      <ThesisPrimer />
      <div className="lens-workbench thesis-shell">
        <LensTabs selectedLens={lens} onSelect={setLens} />
        <div className="lens-stage">
          <LensDiagram key={lens} lens={lens} />
          <LensExplanation content={content} />
        </div>
      </div>
      <ThesisStatement />
    </section>
  );
};

const LensHero: React.FC = () => {
  return (
    <header className="lens-hero thesis-shell">
      <SiteBackLink href="/ideas">Ideas</SiteBackLink>
      <p>An interactive architecture thesis</p>
      <h1>Software Architecture in the AI Era</h1>
      <strong>Build a hard domain core. Surround it with a soft intelligence shell.</strong>
      <span>
        Generative AI expands what software can understand and coordinate. The domain
        model preserves what the business can trust.
      </span>
    </header>
  );
};

const ThesisPrimer: React.FC = () => {
  return (
    <section className="thesis-primer thesis-shell" aria-labelledby="plain-language-title">
      <div className="primer-copy">
        <p>The architectural argument</p>
        <h2 id="plain-language-title">
          The model can be flexible because the system remains firm.
        </h2>
        <span>
          Models are powerful at interpreting language, incomplete information, and novel
          situations. They are weak foundations for authority because their answers can vary.
          A durable domain core turns useful reasoning into safe, repeatable business action.
        </span>
      </div>
      <div className="primer-terms" aria-label="Key terms">
        <PrimerTerm
          label="Hard core"
          meaning={'Typed business concepts, invariants, permissions, and auditable '
            + 'state transitions.'}
        />
        <PrimerTerm
          label="Soft shell"
          meaning="AI that interprets intent, gathers context, investigates, and recommends action."
        />
        <PrimerTerm
          label="Governed boundary"
          meaning="Commands, policies, and approvals that validate reasoning before state changes."
        />
      </div>
    </section>
  );
};

interface PrimerTermProps {
  readonly label: string;
  readonly meaning: string;
}

const PrimerTerm: React.FC<PrimerTermProps> = ({ label, meaning }) => {
  return (
    <article>
      <strong>{label}</strong>
      <span>{meaning}</span>
    </article>
  );
};

interface LensTabsProps {
  readonly onSelect: (lens: ArchitectureLens) => void;
  readonly selectedLens: ArchitectureLens;
}

const LensTabs: React.FC<LensTabsProps> = ({ onSelect, selectedLens }) => {
  return (
    <div className="lens-tabs" role="tablist" aria-label="Architecture scope">
      <span>Choose the frame</span>
      {LENS_OPTIONS.map((option) => (
        <button
          aria-selected={selectedLens === option.id}
          className={selectedLens === option.id ? 'is-active' : ''}
          id={`lens-tab-${option.id}`}
          key={option.id}
          role="tab"
          type="button"
          onClick={() => onSelect(option.id)}
        >
          {option.label}
        </button>
      ))}
    </div>
  );
};

const LensDiagram: React.FC<{ readonly lens: ArchitectureLens }> = ({ lens }) => {
  return (
    <div
      aria-labelledby={`lens-tab-${lens}`}
      className="diagram-slot diagram-transition"
      role="tabpanel"
    >
      {renderDiagram(lens)}
    </div>
  );
};

const renderDiagram = (lens: ArchitectureLens): React.ReactElement => {
  switch (lens) {
    case 'ai-component':
      return <OrbDiagram core="Probabilistic model" shell="Deterministic harness" />;
    case 'enterprise-landscape':
      return <EnterpriseLandscapeDiagram />;
    default:
      return <OrbDiagram core="Deterministic domain" shell="Probabilistic intelligence" />;
  }
};

interface OrbDiagramProps {
  readonly core: string;
  readonly shell: string;
}

const OrbDiagram: React.FC<OrbDiagramProps> = ({ core, shell }) => {
  const isDomainCore: boolean = core === 'Deterministic domain';
  return (
    <div className={`architecture-orb ${isDomainCore ? 'is-system' : 'is-component'}`}>
      <span className="orb-shell-label">{shell}</span>
      <div className="execution-boundary">
        <span>Governed execution boundary</span>
        <div className="orb-core">
          <strong>{core}</strong>
          <small>Source of {isDomainCore ? 'authority' : 'reasoning'}</small>
        </div>
      </div>
    </div>
  );
};

const EnterpriseLandscapeDiagram: React.FC = () => {
  return (
    <div className="enterprise-landscape-diagram">
      <span className="intelligence-fabric-label">Continuous intelligence fabric</span>
      <div className="delegated-intent">
        <small>User intent</small><strong>Agents acting with delegated authority</strong>
      </div>
      <div className="agent-ribbon" aria-label="Agent responsibilities">
        <span>Interpret</span><span>Coordinate</span><span>Monitor</span>
      </div>
      <div className="governed-tools-label">Governed tools, identity, and policy</div>
      <div className="enterprise-core-grid">
        <SystemCore name="CRM" authority="Customer truth" />
        <SystemCore name="ERP" authority="Financial truth" />
        <SystemCore name="Operations" authority="Execution state" />
        <SystemCore name="Identity" authority="Access policy" />
      </div>
    </div>
  );
};

interface SystemCoreProps {
  readonly authority: string;
  readonly name: string;
}

const SystemCore: React.FC<SystemCoreProps> = ({ authority, name }) => {
  return (
    <article className="enterprise-core">
      <strong>{name}</strong><small>{authority}</small>
    </article>
  );
};

const LensExplanation: React.FC<{ readonly content: LensContent }> = ({ content }) => {
  return (
    <article className="lens-explanation" aria-live="polite">
      <p>01 / Change the frame</p>
      <h2>{content.title}</h2>
      <span>{content.description}</span>
      <div className="lens-plain-language">
        <strong>In plain English</strong>
        <p>{content.plainLanguage}</p>
      </div>
      <div className="lens-example">
        <strong>For example</strong>
        <p>{content.example}</p>
      </div>
      <div className="lens-equation">
        <span>{content.formulaLeft}</span><b>+</b><span>{content.formulaRight}</span>
        <b>=</b><strong>Enterprise capability</strong>
      </div>
    </article>
  );
};

const ThesisStatement: React.FC = () => {
  return (
    <div className="thesis-statement thesis-shell">
      <p>The durable boundary</p>
      <blockquote>
        “The agent reasons about the domain; it does not redefine the domain.”
      </blockquote>
      <div>
        <span>Authority → domain cores</span>
        <span>Intelligence → agentic fabric</span>
        <span>Execution → governed boundaries</span>
      </div>
    </div>
  );
};
