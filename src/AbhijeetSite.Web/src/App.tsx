import React from 'react';
import { AdminArticlesPage } from './features/admin/AdminArticlesPage';
import { ArchitecturePage } from './features/architecture/ArchitecturePage';
import { HomePage } from './features/home/HomePage';
import { IdeaDetailPage } from './features/ideas/IdeaDetailPage';
import { IdeasPage } from './features/ideas/IdeasPage';
import { SiteHeader } from './shared/navigation/SiteHeader';

const HOME_PATH = '/';
const ARCHITECTURE_PATH = '/architecture';
const ADMIN_ARTICLES_PATH = '/admin/articles';
const IDEAS_PATH = '/ideas';
const LEGACY_ARTICLES_PATH = '/articles';
const IDEA_PATH_PREFIX = `${IDEAS_PATH}/`;
const LEGACY_ARTICLE_PATH_PREFIX = `${LEGACY_ARTICLES_PATH}/`;

function App(): React.ReactElement {
  return renderRoute(getRoutePath());
}

const renderRoute = (routePath: string): React.ReactElement => {
  if (routePath === HOME_PATH) {
    return <HomePage />;
  }

  if (routePath === ARCHITECTURE_PATH) {
    return <ArchitecturePage />;
  }

  if (routePath === ADMIN_ARTICLES_PATH) {
    return <AdminArticlesPage />;
  }

  if (routePath === IDEAS_PATH || routePath === LEGACY_ARTICLES_PATH) {
    return <IdeasPage />;
  }

  const ideaSlug: string | null = getIdeaSlug(routePath);
  return ideaSlug === null ? <NotFoundPage /> : <IdeaDetailPage slug={ideaSlug} />;
};

const getIdeaSlug = (routePath: string): string | null => {
  if (routePath.startsWith(IDEA_PATH_PREFIX)) {
    return getNonEmptySlug(routePath, IDEA_PATH_PREFIX);
  }

  if (routePath.startsWith(LEGACY_ARTICLE_PATH_PREFIX)) {
    return getNonEmptySlug(routePath, LEGACY_ARTICLE_PATH_PREFIX);
  }

  return null;
};

const getNonEmptySlug = (routePath: string, prefix: string): string | null => {
  const slug: string = routePath.slice(prefix.length);
  return slug.length === 0 ? null : slug;
};

const getRoutePath = (): string => {
  const pathName: string = window.location.pathname;

  if (pathName.length > HOME_PATH.length && pathName.endsWith('/')) {
    return pathName.slice(0, -1);
  }

  return pathName;
};

const NotFoundPage: React.FC = () => {
  return (
    <>
      <SiteHeader activeSection="none" />
      <main className="status-screen">
        <div className="error-panel">
          <h1>Page not found</h1>
          <p>The requested page does not exist.</p>
          <a className="primary-link" href="/">Go home</a>
        </div>
      </main>
    </>
  );
};

export default App;
