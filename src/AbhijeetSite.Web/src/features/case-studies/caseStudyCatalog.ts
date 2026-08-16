export const CASE_STUDIES_PATH = '/case-studies';
export const REGULATORY_CASE_STUDY_PATH =
  `${CASE_STUDIES_PATH}/regulatory-platform-modernization`;
const CASE_STUDY_CHAPTER_PATH_PREFIX = `${REGULATORY_CASE_STUDY_PATH}/`;

export interface CaseStudyChapter {
  readonly order: number;
  readonly shortTitle: string;
  readonly slug: string;
  readonly sourceFile: string;
  readonly summary: string;
  readonly title: string;
}

export interface CaseStudy {
  readonly chapters: ReadonlyArray<CaseStudyChapter>;
  readonly sourceRepositoryUrl: string;
  readonly sourceRevision: string;
  readonly summary: string;
  readonly title: string;
}

export interface NeighboringChapters {
  readonly next: CaseStudyChapter | undefined;
  readonly previous: CaseStudyChapter | undefined;
}

const createChapter = (
  order: number,
  title: string,
  shortTitle: string,
  slug: string,
  sourceFile: string,
  summary: string,
): CaseStudyChapter => ({ order, title, shortTitle, slug, sourceFile, summary });

export const CASE_STUDY: CaseStudy = {
  title: 'Regulatory Platform Modernization',
  summary: 'A domain-led strategy for converging a fragmented product family without a rewrite.',
  sourceRepositoryUrl: 'https://github.com/abhijeethaval/Regulatory-Case-Arch-Case-Study',
  sourceRevision: 'bb029cba7b60e4474308538a0f2f2ba983b4f0a2',
  chapters: [
    createChapter(1, 'Case-Study Premise', 'Premise', 'case-study-premise',
      '01-case-study-premise.md',
      'Fictional constraints, synthetic outcomes, transformation thesis, and guardrails.'),
    createChapter(2, 'Decision and Assumption Log', 'Decisions', 'decision-log',
      '02-decision-log.md',
      'Architecture decisions, consequences, rejected alternatives, and validation hypotheses.'),
    createChapter(3, 'Case-Study Outline', 'Outline', 'case-study-outline',
      '03-case-study-outline.md',
      'The public narrative, supporting artifacts, and the argument that connects them.'),
    createChapter(4, 'Domain Architecture', 'Domain map', 'domain-architecture',
      '04-domain-architecture.md',
      'Bounded contexts, ownership, contracts, orchestration, and consistency boundaries.'),
    createChapter(5, 'Bounded-Context Catalog', 'Context catalog',
      'bounded-context-catalog', '05-context-catalog.md',
      'Context responsibilities, relationships, aggregate candidates, and tracer scoring.'),
    createChapter(6, 'Customization Governance Framework', 'Governed variation',
      'customization-governance', '06-customization-governance.md',
      'A decision model for core, configuration, extension, integration, and exceptions.'),
    createChapter(7, 'Current-State Platform Assessment', 'Assessment',
      'current-state-assessment', '07-current-state-assessment.md',
      'Evidence collection, divergence measures, portfolio hypotheses, and assessment gates.'),
    createChapter(8, 'Shared-Core Migration Roadmap', 'Migration', 'migration-roadmap',
      '08-migration-roadmap.md',
      'Incremental coexistence, tracer sequencing, dependencies, risks, and synthetic milestones.'),
    createChapter(9, 'Platform Engineering Foundations', 'Engineering',
      'platform-engineering-foundations', '09-platform-engineering-foundations.md',
      'Golden paths, delivery controls, observability, API governance, and adoption strategy.'),
    createChapter(10, 'Architecture Leadership and Operating Model', 'Operating model',
      'architecture-operating-model', '10-operating-model.md',
      'Decision rights, team interactions, governance cadence, and executive scorecards.'),
    createChapter(11, 'Context and Canonical Language', 'Language', 'canonical-language',
      'CONTEXT.md',
      'The neutral domain vocabulary and scope rules used throughout the thesis.'),
  ],
};

export const findCaseStudyChapter = (slug: string): CaseStudyChapter | undefined => {
  return CASE_STUDY.chapters.find((chapter) => chapter.slug === slug);
};

export const getCaseStudyChapterHref = (chapter: CaseStudyChapter): string => {
  return `${REGULATORY_CASE_STUDY_PATH}/${chapter.slug}`;
};

export const getCaseStudyChapterSlug = (routePath: string): string | null => {
  if (!routePath.startsWith(CASE_STUDY_CHAPTER_PATH_PREFIX)) {
    return null;
  }

  const slug: string = routePath.slice(CASE_STUDY_CHAPTER_PATH_PREFIX.length);
  return slug.length > 0 && !slug.includes('/') ? slug : null;
};

export const getNeighboringChapters = (chapter: CaseStudyChapter): NeighboringChapters => {
  const chapterIndex: number = CASE_STUDY.chapters.indexOf(chapter);
  return {
    previous: chapterIndex > 0 ? CASE_STUDY.chapters[chapterIndex - 1] : undefined,
    next: chapterIndex < CASE_STUDY.chapters.length - 1
      ? CASE_STUDY.chapters[chapterIndex + 1]
      : undefined,
  };
};
