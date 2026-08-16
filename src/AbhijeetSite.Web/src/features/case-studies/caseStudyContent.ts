import premiseMarkdown from './content/regulatory-platform/01-case-study-premise.md?raw';
import decisionLogMarkdown from './content/regulatory-platform/02-decision-log.md?raw';
import outlineMarkdown from './content/regulatory-platform/03-case-study-outline.md?raw';
import domainArchitectureMarkdown from './content/regulatory-platform/04-domain-architecture.md?raw';
import contextCatalogMarkdown from './content/regulatory-platform/05-context-catalog.md?raw';
import customizationGovernanceMarkdown from
  './content/regulatory-platform/06-customization-governance.md?raw';
import currentStateMarkdown from './content/regulatory-platform/07-current-state-assessment.md?raw';
import migrationRoadmapMarkdown from './content/regulatory-platform/08-migration-roadmap.md?raw';
import platformEngineeringMarkdown from
  './content/regulatory-platform/09-platform-engineering-foundations.md?raw';
import operatingModelMarkdown from './content/regulatory-platform/10-operating-model.md?raw';
import canonicalLanguageMarkdown from './content/regulatory-platform/CONTEXT.md?raw';
import type { CaseStudyChapter } from './caseStudyCatalog';

const CONTENT_BY_SOURCE_FILE: Readonly<Partial<Record<string, string>>> = {
  '01-case-study-premise.md': premiseMarkdown,
  '02-decision-log.md': decisionLogMarkdown,
  '03-case-study-outline.md': outlineMarkdown,
  '04-domain-architecture.md': domainArchitectureMarkdown,
  '05-context-catalog.md': contextCatalogMarkdown,
  '06-customization-governance.md': customizationGovernanceMarkdown,
  '07-current-state-assessment.md': currentStateMarkdown,
  '08-migration-roadmap.md': migrationRoadmapMarkdown,
  '09-platform-engineering-foundations.md': platformEngineeringMarkdown,
  '10-operating-model.md': operatingModelMarkdown,
  'CONTEXT.md': canonicalLanguageMarkdown,
};

export class CaseStudyContentError extends Error {
  public constructor(sourceFile: string) {
    super(`Case-study content is missing for source file "${sourceFile}".`);
    this.name = 'CaseStudyContentError';
  }
}

export const getCaseStudyChapterContent = (chapter: CaseStudyChapter): string => {
  const markdown: string | undefined = CONTENT_BY_SOURCE_FILE[chapter.sourceFile];
  if (markdown === undefined) {
    throw new CaseStudyContentError(chapter.sourceFile);
  }

  return markdown;
};
