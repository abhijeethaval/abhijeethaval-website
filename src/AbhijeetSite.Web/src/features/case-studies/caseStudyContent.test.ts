import { describe, expect, it } from 'vitest';
import { findArchitectureDiagram } from './architectureDiagrams';
import { CASE_STUDY } from './caseStudyCatalog';
import { getCaseStudyChapterContent } from './caseStudyContent';

const DOMAIN_ARCHITECTURE_TITLE = '# Domain Architecture';
const MERMAID_BLOCK_PATTERN = /```mermaid\r?\n([\s\S]*?)```/g;

describe('caseStudyContent', () => {
  it('bundles non-empty Markdown for every catalog chapter', () => {
    const markdownDocuments: ReadonlyArray<string> = CASE_STUDY.chapters.map(
      getCaseStudyChapterContent,
    );

    expect(markdownDocuments).toHaveLength(CASE_STUDY.chapters.length);
    expect(markdownDocuments.every((markdown) => markdown.startsWith('# '))).toBe(true);
  });

  it('resolves content by the source filename recorded in the catalog', () => {
    const chapter = CASE_STUDY.chapters.find(({ slug }) => slug === 'domain-architecture');

    expect(chapter).toBeDefined();
    if (chapter === undefined) {
      return;
    }

    expect(getCaseStudyChapterContent(chapter).startsWith(DOMAIN_ARCHITECTURE_TITLE)).toBe(true);
  });

  it('has a reviewed self-hosted asset for every Mermaid block', () => {
    const mermaidSources: ReadonlyArray<string> = CASE_STUDY.chapters.flatMap((chapter) => {
      const markdown: string = getCaseStudyChapterContent(chapter);
      return Array.from(
        markdown.matchAll(MERMAID_BLOCK_PATTERN),
        (match) => match[1].trimEnd(),
      );
    });

    expect(mermaidSources).toHaveLength(5);
    expect(mermaidSources.every((source) => findArchitectureDiagram(source) !== undefined))
      .toBe(true);
  });
});
