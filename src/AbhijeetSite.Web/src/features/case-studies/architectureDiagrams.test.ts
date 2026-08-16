import { describe, expect, it } from 'vitest';
import { ARCHITECTURE_DIAGRAMS, findArchitectureDiagram } from './architectureDiagrams';

const DOMAIN_CONTEXT_MAP_SOURCE = `flowchart LR
    Product["Regulatory Product"]`;
const UNKNOWN_DIAGRAM_SOURCE = 'flowchart LR\n    A --> B';

describe('architectureDiagrams', () => {
  it('maps every reviewed Mermaid block to a unique self-hosted SVG', () => {
    const sources: ReadonlyArray<string> = ARCHITECTURE_DIAGRAMS.map(({ source }) => source);

    expect(ARCHITECTURE_DIAGRAMS).toHaveLength(5);
    expect(new Set(sources).size).toBe(ARCHITECTURE_DIAGRAMS.length);
  });

  it('resolves the domain context map from its stable source signature', () => {
    const diagram = findArchitectureDiagram(DOMAIN_CONTEXT_MAP_SOURCE);

    expect(diagram?.src).toBe(
      '/case-studies/regulatory-platform-modernization/diagrams/domain-context-map.svg',
    );
  });

  it('rejects Mermaid source without a reviewed asset', () => {
    expect(findArchitectureDiagram(UNKNOWN_DIAGRAM_SOURCE)).toBeUndefined();
  });
});
