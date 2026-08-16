import React, { useEffect } from 'react';
import { ClientLink } from '../../shared/navigation/ClientLink';
import { SiteBackLink } from '../../shared/navigation/SiteBackLink';
import { SiteHeader } from '../../shared/navigation/SiteHeader';
import {
  CASE_STUDY,
  findCaseStudyChapter,
  getCaseStudyChapterHref,
  getNeighboringChapters,
  REGULATORY_CASE_STUDY_PATH,
} from './caseStudyCatalog';
import type { CaseStudyChapter } from './caseStudyCatalog';
import { getCaseStudyChapterContent } from './caseStudyContent';
import { MarkdownDocument } from './MarkdownDocument';
import './caseStudies.css';

interface CaseStudyChapterPageProps {
  readonly chapterSlug: string;
}

export const CaseStudyChapterPage: React.FC<CaseStudyChapterPageProps> = ({ chapterSlug }) => {
  const chapter: CaseStudyChapter | undefined = findCaseStudyChapter(chapterSlug);
  useChapterTitle(chapter);
  return chapter === undefined ? <ChapterNotFound /> : <ChapterContent chapter={chapter} />;
};

const ChapterContent: React.FC<{ readonly chapter: CaseStudyChapter }> = ({ chapter }) => {
  const markdown: string = getCaseStudyChapterContent(chapter);
  return (
    <>
      <SiteHeader activeSection="case-studies" />
      <main className="case-study-reader">
        <div className="case-study-reader-shell">
          <ChapterSidebar activeChapter={chapter} />
          <article className="case-study-document">
            <ChapterHeader chapter={chapter} />
            <MarkdownDocument markdown={markdown} />
            <ChapterPager chapter={chapter} />
          </article>
        </div>
      </main>
    </>
  );
};

const ChapterHeader: React.FC<{ readonly chapter: CaseStudyChapter }> = ({ chapter }) => (
  <header className="case-study-reader-header">
    <p className="eyebrow">
      Chapter {chapter.order} of {CASE_STUDY.chapters.length} · {chapter.shortTitle}
    </p>
    <h1>{chapter.title}</h1>
    <p>{chapter.summary}</p>
  </header>
);

const ChapterSidebar: React.FC<{ readonly activeChapter: CaseStudyChapter }> = ({
  activeChapter,
}) => (
  <aside className="case-study-reader-nav" aria-label="Case-study chapters">
    <SiteBackLink href={REGULATORY_CASE_STUDY_PATH}>
      Regulatory Platform Modernization
    </SiteBackLink>
    <p className="case-study-reader-nav-heading">Thesis contents</p>
    <ol>
      {CASE_STUDY.chapters.map((chapter) => (
        <li key={chapter.slug}>
          <ClientLink
            href={getCaseStudyChapterHref(chapter)}
            aria-current={chapter === activeChapter ? 'page' : undefined}
          >
            <span>{String(chapter.order).padStart(2, '0')}</span>{chapter.shortTitle}
          </ClientLink>
        </li>
      ))}
    </ol>
    <SourceRevision />
  </aside>
);

const SourceRevision: React.FC = () => (
  <p className="case-study-source-revision">
    Source snapshot <a
      href={`${CASE_STUDY.sourceRepositoryUrl}/tree/${CASE_STUDY.sourceRevision}`}
      target="_blank"
      rel="noopener noreferrer"
    >{CASE_STUDY.sourceRevision.slice(0, 7)}</a>
  </p>
);

const ChapterPager: React.FC<{ readonly chapter: CaseStudyChapter }> = ({ chapter }) => {
  const { next, previous } = getNeighboringChapters(chapter);
  return (
    <nav className="case-study-pager" aria-label="Adjacent case-study chapters">
      {previous === undefined ? <span /> : <PagerLink chapter={previous} direction="Previous" />}
      {next === undefined ? <span /> : <PagerLink chapter={next} direction="Next" />}
    </nav>
  );
};

const PagerLink: React.FC<{
  readonly chapter: CaseStudyChapter;
  readonly direction: 'Next' | 'Previous';
}> = ({ chapter, direction }) => (
  <ClientLink href={getCaseStudyChapterHref(chapter)}>
    <span>{direction}</span><strong>{chapter.shortTitle}</strong>
  </ClientLink>
);

const ChapterNotFound: React.FC = () => (
  <>
    <SiteHeader activeSection="case-studies" />
    <main className="status-screen">
      <div className="error-panel">
        <h1>Case-study chapter not found</h1>
        <p>The requested chapter is not part of the published thesis.</p>
        <ClientLink className="primary-link" href={REGULATORY_CASE_STUDY_PATH}>
          View the case study
        </ClientLink>
      </div>
    </main>
  </>
);

const useChapterTitle = (chapter: CaseStudyChapter | undefined): void => {
  useEffect(() => {
    const previousTitle: string = document.title;
    document.title = chapter === undefined
      ? 'Case Study Not Found | Abhijeet Haval'
      : `${chapter.title} | Regulatory Platform Modernization`;
    return () => { document.title = previousTitle; };
  }, [chapter]);
};
