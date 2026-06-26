import React from 'react';
import { SiteHeader } from '../../shared/navigation/SiteHeader';

interface ArticleStatusScreenProps {
  readonly title: string;
  readonly message: string;
  readonly onRetry?: () => void;
}

export const ArticleStatusScreen: React.FC<ArticleStatusScreenProps> = ({
  title,
  message,
  onRetry,
}) => {
  return (
    <>
      <SiteHeader activeSection="articles" />
      <main className="status-screen">
        <div className="error-panel">
          <h1>{title}</h1>
          <p>{message}</p>
          {onRetry === undefined ? null : (
            <button type="button" onClick={onRetry}>Retry</button>
          )}
        </div>
      </main>
    </>
  );
};
