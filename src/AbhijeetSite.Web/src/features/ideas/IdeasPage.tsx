import React, { useEffect, useState } from 'react';
import { articlesApi } from '../articles/articlesApi';
import { PublishedArticleSummary } from '../articles/types';
import { SiteHeader } from '../../shared/navigation/SiteHeader';
import {
  FEATURED_INTERACTIVE_IDEA,
  InteractiveIdea,
  isInteractiveIdeaSlug,
} from './interactiveIdeas';
import './ideasPage.css';

interface PublishedIdeasState {
  readonly articles: ReadonlyArray<PublishedArticleSummary>;
  readonly error: string | null;
  readonly isLoading: boolean;
}

const INITIAL_STATE: PublishedIdeasState = {
  articles: [],
  error: null,
  isLoading: true,
};

export const IdeasPage: React.FC = () => {
  const [state, setState] = useState<PublishedIdeasState>(INITIAL_STATE);

  useEffect(() => {
    void loadPublishedIdeas(setState);
  }, []);

  return (
    <>
      <SiteHeader activeSection="ideas" />
      <main className="ideas-page">
        <IdeasHero />
        <FeaturedIdea idea={FEATURED_INTERACTIVE_IDEA} />
        <WrittenIdeasSection state={state} onRetry={() => void loadPublishedIdeas(setState)} />
      </main>
    </>
  );
};

const IdeasHero: React.FC = () => {
  return (
    <section className="ideas-hero section-band">
      <div className="section-shell ideas-heading">
        <p className="eyebrow">Ideas</p>
        <h1>Architecture you can explore, not just read.</h1>
        <p>
          Each idea starts with a practical question, explains the architecture in plain
          language, and lets you explore the consequences. No AI background required.
        </p>
      </div>
    </section>
  );
};

const FeaturedIdea: React.FC<{ readonly idea: InteractiveIdea }> = ({ idea }) => {
  return (
    <section className="featured-idea-section section-band">
      <div className="section-shell">
        <article className="featured-idea-card">
          <div className="featured-idea-copy">
            <p>{idea.format} · Featured</p>
            <h2>{idea.title}</h2>
            <span>{idea.summary}</span>
            <a href={`/ideas/${idea.slug}`}>Explore the thesis <b>→</b></a>
          </div>
          <FeaturedIdeaVisual />
        </article>
      </div>
    </section>
  );
};

const FeaturedIdeaVisual: React.FC = () => {
  return (
    <div className="featured-idea-visual" aria-label="Hard domain core and intelligence shell">
      <span>Probabilistic intelligence</span>
      <div><strong>Deterministic domain</strong><small>Source of authority</small></div>
      <p>Change the frame</p>
    </div>
  );
};

interface WrittenIdeasSectionProps {
  readonly onRetry: () => void;
  readonly state: PublishedIdeasState;
}

const WrittenIdeasSection: React.FC<WrittenIdeasSectionProps> = ({ onRetry, state }) => {
  const articles: ReadonlyArray<PublishedArticleSummary> = state.articles.filter(
    (article) => !isInteractiveIdeaSlug(article.slug),
  );

  return (
    <section className="written-ideas-section section-band">
      <div className="section-shell">
        <div className="written-ideas-heading">
          <div>
            <p className="eyebrow">Build notes</p>
            <span>Concrete decisions from building the site itself.</span>
          </div>
          <h2>From an architecture idea to working software.</h2>
        </div>
        <PublishedIdeasState state={state} articles={articles} onRetry={onRetry} />
      </div>
    </section>
  );
};

interface PublishedIdeasStateProps {
  readonly articles: ReadonlyArray<PublishedArticleSummary>;
  readonly onRetry: () => void;
  readonly state: PublishedIdeasState;
}

const PublishedIdeasState: React.FC<PublishedIdeasStateProps> = ({
  articles,
  onRetry,
  state,
}) => {
  if (state.isLoading) {
    return <p className="ideas-catalog-status">Loading published writing…</p>;
  }

  if (state.error !== null) {
    return <PublishedIdeasError message={state.error} onRetry={onRetry} />;
  }

  return <PublishedIdeasList articles={articles} />;
};

const PublishedIdeasError: React.FC<{
  readonly message: string;
  readonly onRetry: () => void;
}> = ({ message, onRetry }) => {
  return (
    <div className="ideas-catalog-error">
      <p>{message}</p>
      <button type="button" onClick={onRetry}>Retry published writing</button>
    </div>
  );
};

const PublishedIdeasList: React.FC<{
  readonly articles: ReadonlyArray<PublishedArticleSummary>;
}> = ({ articles }) => {
  if (articles.length === 0) {
    return <p className="ideas-catalog-status">More written explorations are coming.</p>;
  }

  return (
    <div className="published-ideas-list">
      {articles.map((article) => <PublishedIdeaCard article={article} key={article.slug} />)}
    </div>
  );
};

const PublishedIdeaCard: React.FC<{
  readonly article: PublishedArticleSummary;
}> = ({ article }) => {
  return (
    <article className="published-idea-card">
      <div>
        <p>Build note · {formatIdeaDate(article.publishedAt)}</p>
        <h3><a href={`/ideas/${article.slug}`}>{article.title}</a></h3>
        <span>{article.summary}</span>
      </div>
      <a href={`/ideas/${article.slug}`} aria-label={`Read ${article.title}`}>Read <b>→</b></a>
    </article>
  );
};

const loadPublishedIdeas = async (
  setState: React.Dispatch<React.SetStateAction<PublishedIdeasState>>,
): Promise<void> => {
  setState((current) => ({ ...current, error: null, isLoading: true }));
  try {
    const articles: ReadonlyArray<PublishedArticleSummary> =
      await articlesApi.getPublishedArticles();
    setState({ articles, error: null, isLoading: false });
  } catch (errorValue) {
    setState({ articles: [], error: getErrorMessage(errorValue), isLoading: false });
  }
};

const formatIdeaDate = (value: string): string => {
  return new Intl.DateTimeFormat('en', { dateStyle: 'medium' }).format(new Date(value));
};

const getErrorMessage = (errorValue: unknown): string => {
  return errorValue instanceof Error
    ? `Published writing is unavailable: ${errorValue.message}`
    : 'Published writing is unavailable due to an unknown client error.';
};
