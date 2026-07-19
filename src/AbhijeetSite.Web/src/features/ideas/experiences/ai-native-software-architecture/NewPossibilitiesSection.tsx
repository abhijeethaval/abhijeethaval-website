import React from 'react';

interface Possibility {
  readonly before: string;
  readonly now: string;
  readonly title: string;
}

const POSSIBILITIES: ReadonlyArray<Possibility> = [
  {
    title: 'Intent-driven work',
    before: 'Every variation needed a predefined form, screen, or workflow.',
    now: 'An agent can understand the goal, gather missing context, and invoke typed capabilities.',
  },
  {
    title: 'Continuous semantic oversight',
    before: 'Humans could review only a sample of documents, events, and '
      + 'operational evidence.',
    now: 'AI can interpret the full stream while deterministic systems validate '
      + 'and record findings.',
  },
  {
    title: 'Delegated work across systems',
    before: 'People had to reconcile context and coordinate actions across '
      + 'disconnected applications.',
    now: 'Agents can work across governed APIs while each enterprise system retains its authority.',
  },
];

export const NewPossibilitiesSection: React.FC = () => {
  return (
    <section className="possibilities-section">
      <div className="thesis-shell">
        <header className="possibilities-heading">
          <p>Why AI changes the economics</p>
          <h2>Previously impractical use cases can now become dependable software.</h2>
          <span>
            Traditional software works best when every path can be specified in advance.
            Generative AI can interpret the ambiguous long tail. The hard domain core makes
            that interpretation safe to operationalize.
          </span>
        </header>
        <div className="possibilities-grid">
          {POSSIBILITIES.map((possibility) => (
            <PossibilityCard key={possibility.title} possibility={possibility} />
          ))}
        </div>
      </div>
    </section>
  );
};

const PossibilityCard: React.FC<{ readonly possibility: Possibility }> = ({ possibility }) => {
  return (
    <article className="possibility-card">
      <h3>{possibility.title}</h3>
      <div><strong>Before</strong><p>{possibility.before}</p></div>
      <div><strong>With a soft shell</strong><p>{possibility.now}</p></div>
    </article>
  );
};
