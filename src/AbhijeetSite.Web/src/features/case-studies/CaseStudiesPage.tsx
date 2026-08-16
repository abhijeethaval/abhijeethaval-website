import React, { useEffect } from 'react';
import { ClientLink } from '../../shared/navigation/ClientLink';
import { SiteHeader } from '../../shared/navigation/SiteHeader';
import {
  CASE_STUDY,
  getCaseStudyChapterHref,
} from './caseStudyCatalog';
import type { CaseStudyChapter } from './caseStudyCatalog';
import './caseStudies.css';

const PAGE_TITLE = 'Regulatory Platform Modernization | Abhijeet Haval';

export const CaseStudiesPage: React.FC = () => {
  usePageTitle(PAGE_TITLE);
  return (
    <>
      <SiteHeader activeSection="case-studies" />
      <main className="case-studies-page">
        <CaseStudyHero />
        <TransformationThesis />
        <ChapterCatalog />
      </main>
    </>
  );
};

const CaseStudyHero: React.FC = () => {
  const firstChapter: CaseStudyChapter = CASE_STUDY.chapters[0];
  return (
    <section className="case-study-hero section-band">
      <div className="section-shell case-study-hero-grid">
        <div className="case-study-hero-copy">
          <p className="eyebrow">Architecture case study</p>
          <h1>{CASE_STUDY.title}</h1>
          <p>{CASE_STUDY.summary}</p>
          <div className="case-study-actions">
            <ClientLink
              className="case-study-primary-action"
              href={getCaseStudyChapterHref(firstChapter)}
            >
              Start the thesis <span aria-hidden="true">→</span>
            </ClientLink>
            <a href={CASE_STUDY.sourceRepositoryUrl} target="_blank" rel="noopener noreferrer">
              View source repository
            </a>
          </div>
          <p className="case-study-disclosure">
            Fictional scenario · synthetic targets · no client or employer claims
          </p>
        </div>
        <TransformationMap />
      </div>
    </section>
  );
};

const TransformationMap: React.FC = () => {
  return (
    <div className="transformation-map" aria-label="Architecture transformation thesis">
      <div className="transformation-state transformation-state-fragmented">
        <span>Current pressure</span>
        <strong>Customer-specific variants</strong>
        <small>Forks · duplicated rules · release drag</small>
      </div>
      <div className="transformation-arrow" aria-hidden="true">↓</div>
      <div className="transformation-core">
        <span>Transformation seam</span>
        <strong>Bounded capabilities</strong>
        <div><small>Contracts</small><small>Configuration</small><small>Extensions</small></div>
      </div>
      <div className="transformation-arrow" aria-hidden="true">↓</div>
      <div className="transformation-state transformation-state-target">
        <span>Target posture</span>
        <strong>Governed shared core</strong>
        <small>Incremental adoption · explicit variation</small>
      </div>
    </div>
  );
};

const TransformationThesis: React.FC = () => {
  return (
    <section className="case-study-thesis section-band">
      <div className="section-shell case-study-thesis-grid">
        <div className="section-heading">
          <p className="eyebrow">The argument</p>
          <h2>Converge behavior without pretending every jurisdiction is the same.</h2>
        </div>
        <div className="case-study-principles">
          <Principle number="01" title="Model the domain">
            Put stable business capabilities behind explicit bounded-context contracts.
          </Principle>
          <Principle number="02" title="Govern variation">
            Separate product configuration and extensions from customer-specific executable code.
          </Principle>
          <Principle number="03" title="Migrate by tracer">
            Prove one end-to-end capability, then make the second adoption the reuse test.
          </Principle>
        </div>
      </div>
    </section>
  );
};

const Principle: React.FC<{
  readonly children: React.ReactNode;
  readonly number: string;
  readonly title: string;
}> = ({ children, number, title }) => (
  <article className="case-study-principle">
    <span>{number}</span><h3>{title}</h3><p>{children}</p>
  </article>
);

const ChapterCatalog: React.FC = () => {
  return (
    <section className="case-study-catalog section-band" id="chapters">
      <div className="section-shell">
        <div className="case-study-catalog-heading">
          <div className="section-heading">
            <p className="eyebrow">Read the work</p>
            <h2>Ten thesis chapters and a canonical-language appendix.</h2>
          </div>
          <p>Each chapter preserves the reviewed Markdown from the public source revision.</p>
        </div>
        <div className="case-study-chapter-grid">
          {CASE_STUDY.chapters.map((chapter) => (
            <ChapterCard chapter={chapter} key={chapter.slug} />
          ))}
        </div>
      </div>
    </section>
  );
};

const ChapterCard: React.FC<{ readonly chapter: CaseStudyChapter }> = ({ chapter }) => (
  <article className="case-study-chapter-card">
    <span>{String(chapter.order).padStart(2, '0')}</span>
    <h3><ClientLink href={getCaseStudyChapterHref(chapter)}>{chapter.title}</ClientLink></h3>
    <p>{chapter.summary}</p>
    <ClientLink className="case-study-card-action" href={getCaseStudyChapterHref(chapter)}>
      Read chapter <span aria-hidden="true">→</span>
    </ClientLink>
  </article>
);

const usePageTitle = (title: string): void => {
  useEffect(() => {
    const previousTitle: string = document.title;
    document.title = title;
    return () => { document.title = previousTitle; };
  }, [title]);
};
