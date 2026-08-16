import { describe, expect, it } from 'vitest';
import {
  CASE_STUDY,
  findCaseStudyChapter,
  getCaseStudyChapterSlug,
  getCaseStudyChapterHref,
  getNeighboringChapters,
} from './caseStudyCatalog';

const DOMAIN_ARCHITECTURE_SLUG = 'domain-architecture';
const UNKNOWN_CHAPTER_SLUG = 'missing-chapter';

describe('caseStudyCatalog', () => {
  it('publishes every thesis document under a unique route', () => {
    const slugs: ReadonlyArray<string> = CASE_STUDY.chapters.map((chapter) => chapter.slug);
    const sourceFiles: ReadonlyArray<string> = CASE_STUDY.chapters.map(
      (chapter) => chapter.sourceFile,
    );

    expect(new Set(slugs).size).toBe(CASE_STUDY.chapters.length);
    expect(new Set(sourceFiles).size).toBe(CASE_STUDY.chapters.length);
    expect(CASE_STUDY.chapters.map((chapter) => chapter.order))
      .toEqual(CASE_STUDY.chapters.map((_, index) => index + 1));
  });

  it('resolves a known chapter and rejects an unknown route', () => {
    const chapter = findCaseStudyChapter(DOMAIN_ARCHITECTURE_SLUG);

    expect(chapter?.title).toBe('Domain Architecture');
    expect(findCaseStudyChapter(UNKNOWN_CHAPTER_SLUG)).toBeUndefined();
  });

  it('builds durable chapter routes from the case-study slug', () => {
    const chapter = findCaseStudyChapter(DOMAIN_ARCHITECTURE_SLUG);

    expect(chapter).toBeDefined();
    if (chapter === undefined) {
      return;
    }

    expect(getCaseStudyChapterHref(chapter))
      .toBe('/case-studies/regulatory-platform-modernization/domain-architecture');
  });

  it('parses only direct chapter routes under the regulatory case study', () => {
    const chapterPath =
      '/case-studies/regulatory-platform-modernization/domain-architecture';

    expect(getCaseStudyChapterSlug(chapterPath)).toBe(DOMAIN_ARCHITECTURE_SLUG);
    expect(getCaseStudyChapterSlug('/case-studies/regulatory-platform-modernization')).toBeNull();
    expect(getCaseStudyChapterSlug('/unrelated/path/that-is-longer-than-the-prefix')).toBeNull();
    expect(getCaseStudyChapterSlug(`${chapterPath}/nested`)).toBeNull();
  });

  it('returns adjacent chapters at the middle and edges of the thesis', () => {
    const firstChapter = CASE_STUDY.chapters[0];
    const middleChapter = findCaseStudyChapter(DOMAIN_ARCHITECTURE_SLUG);
    const lastChapter = CASE_STUDY.chapters[CASE_STUDY.chapters.length - 1];

    expect(firstChapter).toBeDefined();
    expect(middleChapter).toBeDefined();
    expect(lastChapter).toBeDefined();
    if (firstChapter === undefined || middleChapter === undefined || lastChapter === undefined) {
      return;
    }

    expect(getNeighboringChapters(firstChapter).previous).toBeUndefined();
    expect(getNeighboringChapters(middleChapter).previous?.slug).toBe('case-study-outline');
    expect(getNeighboringChapters(middleChapter).next?.slug).toBe('bounded-context-catalog');
    expect(getNeighboringChapters(lastChapter).next).toBeUndefined();
  });
});
