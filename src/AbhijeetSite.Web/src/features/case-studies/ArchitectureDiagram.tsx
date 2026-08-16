import React from 'react';
import { findArchitectureDiagram } from './architectureDiagrams';
import type { ArchitectureDiagramAsset } from './architectureDiagrams';

interface ArchitectureDiagramProps {
  readonly source: string;
}

export const ArchitectureDiagram: React.FC<ArchitectureDiagramProps> = ({ source }) => {
  const diagram: ArchitectureDiagramAsset | undefined = findArchitectureDiagram(source);
  return diagram === undefined
    ? <MissingDiagram source={source} />
    : <ReviewedDiagram diagram={diagram} />;
};

const ReviewedDiagram: React.FC<{
  readonly diagram: ArchitectureDiagramAsset;
}> = ({ diagram }) => (
  <figure className="case-study-diagram">
    <div className="case-study-diagram-viewport">
      <img alt={diagram.alt} loading="lazy" src={diagram.src} />
    </div>
    <figcaption>{diagram.alt}</figcaption>
  </figure>
);

const MissingDiagram: React.FC<{ readonly source: string }> = ({ source }) => (
  <div className="case-study-diagram-error" role="alert">
    <p>This diagram has no reviewed publication asset. The source is shown below.</p>
    <pre><code>{source}</code></pre>
  </div>
);
