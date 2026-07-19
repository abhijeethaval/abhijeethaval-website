import React, { useState } from 'react';

type Boundary = 'core' | 'shell';

interface Responsibility {
  readonly correctBoundary: Boundary;
  readonly description: string;
  readonly id: string;
  readonly label: string;
}

const RESPONSIBILITIES: ReadonlyArray<Responsibility> = [
  {
    id: 'intent',
    label: 'Interpret ambiguous intent',
    description: 'Infer what the user is trying to achieve.',
    correctBoundary: 'shell',
  },
  {
    id: 'eligibility',
    label: 'Decide refund eligibility',
    description: 'Enforce the authoritative business policy.',
    correctBoundary: 'core',
  },
  {
    id: 'explain',
    label: 'Explain a decision',
    description: 'Adapt the explanation to context and audience.',
    correctBoundary: 'shell',
  },
  {
    id: 'transition',
    label: 'Commit state transition',
    description: 'Persist a valid, authorized domain change.',
    correctBoundary: 'core',
  },
  {
    id: 'investigate',
    label: 'Gather missing context',
    description: 'Navigate evidence and resolve uncertainty.',
    correctBoundary: 'shell',
  },
  {
    id: 'authorize',
    label: 'Authorize an operation',
    description: 'Apply identity, tenant, and permission rules.',
    correctBoundary: 'core',
  },
];

const INITIAL_ASSIGNMENTS: Readonly<Record<string, Boundary>> = {
  intent: 'shell',
  eligibility: 'shell',
  explain: 'shell',
  transition: 'shell',
  investigate: 'core',
  authorize: 'core',
};

export const BoundaryLabSection: React.FC = () => {
  const [assignments, setAssignments] =
    useState<Readonly<Record<string, Boundary>>>(INITIAL_ASSIGNMENTS);
  const riskCount: number = countBoundaryRisks(assignments);

  const assignResponsibility = (id: string, boundary: Boundary): void => {
    setAssignments((current) => ({ ...current, [id]: boundary }));
  };

  return (
    <section className="boundary-section">
      <BoundaryHeader riskCount={riskCount} />
      <div className="boundary-workspace thesis-shell">
        <ResponsibilityList assignments={assignments} onAssign={assignResponsibility} />
        <RiskPanel assignments={assignments} riskCount={riskCount} />
      </div>
    </section>
  );
};

const BoundaryHeader: React.FC<{ readonly riskCount: number }> = ({ riskCount }) => {
  return (
    <header className="boundary-header thesis-shell">
      <p>02 / Architecture boundary lab</p>
      <h2>Who gets to decide?</h2>
      <span>
        For each responsibility, choose whether dependable software should own the
        decision or whether adaptive AI should interpret the situation. The review
        updates as you go.
      </span>
      <div className="boundary-score">
        <span>Boundary leaks</span>
        <strong>{riskCount.toString().padStart(2, '0')}</strong>
      </div>
    </header>
  );
};

interface ResponsibilityListProps {
  readonly assignments: Readonly<Record<string, Boundary>>;
  readonly onAssign: (id: string, boundary: Boundary) => void;
}

const ResponsibilityList: React.FC<ResponsibilityListProps> = ({
  assignments,
  onAssign,
}) => {
  return (
    <div className="responsibility-list">
      <div className="boundary-column-head">
        <span>Responsibility</span><span>Place the authority</span>
      </div>
      {RESPONSIBILITIES.map((responsibility) => (
        <ResponsibilityRow
          key={responsibility.id}
          responsibility={responsibility}
          selected={assignments[responsibility.id] ?? 'core'}
          onAssign={onAssign}
        />
      ))}
    </div>
  );
};

interface ResponsibilityRowProps {
  readonly onAssign: (id: string, boundary: Boundary) => void;
  readonly responsibility: Responsibility;
  readonly selected: Boundary;
}

const ResponsibilityRow: React.FC<ResponsibilityRowProps> = ({
  onAssign,
  responsibility,
  selected,
}) => {
  const isCorrect: boolean = responsibility.correctBoundary === selected;
  return (
    <article className={`responsibility-row ${isCorrect ? 'is-correct' : 'has-risk'}`}>
      <div><strong>{responsibility.label}</strong><p>{responsibility.description}</p></div>
      <BoundaryChoice
        id={responsibility.id}
        selected={selected}
        onAssign={onAssign}
      />
    </article>
  );
};

interface BoundaryChoiceProps {
  readonly id: string;
  readonly onAssign: (id: string, boundary: Boundary) => void;
  readonly selected: Boundary;
}

const BoundaryChoice: React.FC<BoundaryChoiceProps> = ({ id, onAssign, selected }) => {
  return (
    <div className="boundary-choice">
      <button
        className={selected === 'core' ? 'is-selected' : ''}
        type="button"
        onClick={() => onAssign(id, 'core')}
      >
        Domain core
      </button>
      <button
        className={selected === 'shell' ? 'is-selected' : ''}
        type="button"
        onClick={() => onAssign(id, 'shell')}
      >
        Agentic shell
      </button>
    </div>
  );
};

interface RiskPanelProps {
  readonly assignments: Readonly<Record<string, Boundary>>;
  readonly riskCount: number;
}

const RiskPanel: React.FC<RiskPanelProps> = ({ assignments, riskCount }) => {
  const riskyItems: ReadonlyArray<Responsibility> = RESPONSIBILITIES.filter(
    (item) => assignments[item.id] !== item.correctBoundary,
  );
  return (
    <aside className="risk-panel" aria-live="polite">
      <p>Live architecture review</p>
      <h3>{getRiskHeadline(riskCount)}</h3>
      <span>{getRiskSummary(riskCount)}</span>
      <div className="risk-meter">
        <i style={{ width: `${(riskCount / RESPONSIBILITIES.length) * 100}%` }} />
      </div>
      <RiskList riskyItems={riskyItems} />
      {riskCount === 0 && (
        <blockquote>The agent reasons about the domain; it does not redefine it.</blockquote>
      )}
    </aside>
  );
};

const RiskList: React.FC<{
  readonly riskyItems: ReadonlyArray<Responsibility>;
}> = ({ riskyItems }) => {
  return (
    <ul>
      {riskyItems.map((item) => (
        <li key={item.id}>
          <strong>{item.label}</strong>
          <span>
            Belongs in the {item.correctBoundary === 'core' ? 'domain core' : 'agentic shell'}.
          </span>
        </li>
      ))}
    </ul>
  );
};

const countBoundaryRisks = (assignments: Readonly<Record<string, Boundary>>): number => {
  return RESPONSIBILITIES.filter((item) => assignments[item.id] !== item.correctBoundary).length;
};

const getRiskHeadline = (riskCount: number): string => {
  return riskCount === 0
    ? 'Authority is explicit.'
    : `${riskCount} boundary ${riskCount === 1 ? 'leak' : 'leaks'} detected.`;
};

const getRiskSummary = (riskCount: number): string => {
  return riskCount === 0
    ? 'Adaptive intelligence surrounds the business without becoming its source of truth.'
    : 'Probabilistic reasoning is making authoritative decisions—or deterministic code '
      + 'is trying to interpret ambiguity.';
};
