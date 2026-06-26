import React, { useEffect, useState } from 'react';
import { SiteHeader } from '../../shared/navigation/SiteHeader';
import { ArticleStatusScreen } from './ArticleStatusScreen';
import { articlesApi } from './articlesApi';
import { PublishedArticle } from './types';

interface ArticleDetailPageProps {
  readonly slug: string;
}

export const ArticleDetailPage: React.FC<ArticleDetailPageProps> = ({ slug }) => {
  const [state, setState] = useState<ArticlePageState>(INITIAL_STATE);

  useEffect(() => {
    void loadArticle(slug, setState);
  }, [slug]);

  if (state.isLoading) {
    return <ArticleStatusScreen title="Loading article" message="Reading the published revision." />;
  }

  if (state.error !== null || state.article === null) {
    return <ArticleStatusScreen title="Article unavailable" message={state.error ?? 'Article was not returned.'} onRetry={() => void loadArticle(slug, setState)} />;
  }

  return <ArticleContent article={state.article} />;
};

interface ArticlePageState {
  readonly article: PublishedArticle | null;
  readonly error: string | null;
  readonly isLoading: boolean;
}

const INITIAL_STATE: ArticlePageState = {
  article: null,
  error: null,
  isLoading: true,
};

const loadArticle = async (
  slug: string,
  setState: React.Dispatch<React.SetStateAction<ArticlePageState>>,
): Promise<void> => {
  setState(INITIAL_STATE);
  try {
    const article: PublishedArticle = await articlesApi.getPublishedArticle(slug);
    setState({ article, error: null, isLoading: false });
  } catch (errorValue) {
    setState({ article: null, error: getErrorMessage(errorValue), isLoading: false });
  }
};

interface ArticleContentProps {
  readonly article: PublishedArticle;
}

const ArticleContent: React.FC<ArticleContentProps> = ({ article }) => {
  return (
    <>
      <SiteHeader activeSection="articles" />
      <main className="article-page">
        <header className="article-header section-band">
          <div className="article-shell">
            <a className="article-back-link" href="/articles">Back to articles</a>
            <p className="eyebrow">Published {formatArticleDate(article.publishedAt)}</p>
            <h1>{article.title}</h1>
            <p className="article-summary">{article.summary}</p>
          </div>
        </header>
        <section className="article-body-section section-band">
          <div
            className="article-body article-shell"
            dangerouslySetInnerHTML={{ __html: article.renderedHtml }}
          />
        </section>
      </main>
    </>
  );
};

const formatArticleDate = (value: string): string => {
  return new Intl.DateTimeFormat('en', { dateStyle: 'long' }).format(new Date(value));
};

const getErrorMessage = (errorValue: unknown): string => {
  return errorValue instanceof Error
    ? `Failed to load the published article: ${errorValue.message}`
    : 'Failed to load the published article due to an unknown client error.';
};
