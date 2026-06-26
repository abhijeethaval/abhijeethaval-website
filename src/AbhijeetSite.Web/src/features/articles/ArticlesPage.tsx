import React, { useEffect, useState } from 'react';
import { SiteHeader } from '../../shared/navigation/SiteHeader';
import { ArticleStatusScreen } from './ArticleStatusScreen';
import { articlesApi } from './articlesApi';
import { PublishedArticleSummary } from './types';

export const ArticlesPage: React.FC = () => {
  const [articles, setArticles] = useState<ReadonlyArray<PublishedArticleSummary>>([]);
  const [isLoading, setIsLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const loadArticles = async (): Promise<void> => {
    try {
      setIsLoading(true);
      setError(null);
      setArticles(await articlesApi.getPublishedArticles());
    } catch (errorValue) {
      setError(getErrorMessage(errorValue));
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    void loadArticles();
  }, []);

  if (isLoading) {
    return <ArticleStatusScreen title="Loading articles" message="Reading the published catalog." />;
  }

  if (error !== null) {
    return <ArticleStatusScreen title="Articles unavailable" message={error} onRetry={() => void loadArticles()} />;
  }

  return <ArticlesContent articles={articles} />;
};

interface ArticlesContentProps {
  readonly articles: ReadonlyArray<PublishedArticleSummary>;
}

const ArticlesContent: React.FC<ArticlesContentProps> = ({ articles }) => {
  return (
    <>
      <SiteHeader activeSection="articles" />
      <main>
        <section className="articles-hero section-band">
          <div className="section-shell articles-heading">
            <p className="eyebrow">Articles</p>
            <h1>Architecture, AI systems, and production engineering.</h1>
            <p>Long-form notes grounded in systems I have designed and shipped.</p>
          </div>
        </section>
        <ArticleList articles={articles} />
      </main>
    </>
  );
};

const ArticleList: React.FC<ArticlesContentProps> = ({ articles }) => {
  if (articles.length === 0) {
    return <p className="empty-articles">No published articles are available.</p>;
  }

  return (
    <section className="articles-list-section section-band">
      <div className="section-shell articles-list">
        {articles.map((article) => <ArticleCard article={article} key={article.slug} />)}
      </div>
    </section>
  );
};

interface ArticleCardProps {
  readonly article: PublishedArticleSummary;
}

const ArticleCard: React.FC<ArticleCardProps> = ({ article }) => {
  return (
    <article className="article-card">
      <time dateTime={article.publishedAt}>{formatArticleDate(article.publishedAt)}</time>
      <h2><a href={`/articles/${article.slug}`}>{article.title}</a></h2>
      <p>{article.summary}</p>
      <a className="article-read-link" href={`/articles/${article.slug}`}>Read article</a>
    </article>
  );
};

const formatArticleDate = (value: string): string => {
  return new Intl.DateTimeFormat('en', { dateStyle: 'long' }).format(new Date(value));
};

const getErrorMessage = (errorValue: unknown): string => {
  return errorValue instanceof Error
    ? `Failed to load published articles: ${errorValue.message}`
    : 'Failed to load published articles due to an unknown client error.';
};
