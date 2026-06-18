import React from 'react';
import { ArchitecturePage } from './features/architecture/ArchitecturePage';
import { HomePage } from './features/home/HomePage';
import { SiteHeader } from './shared/navigation/SiteHeader';

const HOME_PATH = '/';
const ARCHITECTURE_PATH = '/architecture';

function App(): React.ReactElement {
  const routePath: string = getRoutePath();

  if (routePath === HOME_PATH) {
    return <HomePage />;
  }

  if (routePath === ARCHITECTURE_PATH) {
    return <ArchitecturePage />;
  }

  return <NotFoundPage />;
}

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
