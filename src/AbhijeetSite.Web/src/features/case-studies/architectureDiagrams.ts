const DIAGRAM_BASE_PATH =
  '/case-studies/regulatory-platform-modernization/diagrams';

export interface ArchitectureDiagramAsset {
  readonly alt: string;
  readonly source: string;
  readonly src: string;
}

const createDiagram = (
  source: string,
  fileName: string,
  alt: string,
): ArchitectureDiagramAsset => ({
  alt,
  source,
  src: `${DIAGRAM_BASE_PATH}/${fileName}`,
});

export const ARCHITECTURE_DIAGRAMS: ReadonlyArray<ArchitectureDiagramAsset> = [
  createDiagram(
    'flowchart LR\n    Product["Regulatory Product"]',
    'domain-context-map.svg',
    'Context map showing the Regulatory Platform bounded contexts and their relationships.',
  ),
  createDiagram(
    'sequenceDiagram\n    participant O as Regulatory Process',
    'regulatory-process-sequence.svg',
    'Sequence showing a regulatory process coordinating domain capabilities.',
  ),
  createDiagram(
    'flowchart TD\n    A["Describe requirement in domain language"]',
    'customization-decision-flow.svg',
    'Decision flow for classifying core, configuration, extension, integration, and custom work.',
  ),
  createDiagram(
    'flowchart LR\n    A["Source"]',
    'delivery-pipeline.svg',
    'Delivery pipeline from source validation through deployment and operational verification.',
  ),
  createDiagram(
    'flowchart TD\n    A["Requirement or technical decision"]',
    'architecture-decision-workflow.svg',
    'Architecture decision workflow based on decision tier and cross-context impact.',
  ),
];

export const findArchitectureDiagram = (
  markdownSource: string,
): ArchitectureDiagramAsset | undefined => {
  return ARCHITECTURE_DIAGRAMS.find(({ source }) => markdownSource.startsWith(source));
};
