import React from 'react';
import { ArticleDetailPage } from '../articles/ArticleDetailPage';
import { findInteractiveIdea, InteractiveIdea } from './interactiveIdeas';

interface IdeaDetailPageProps {
  readonly slug: string;
}

export const IdeaDetailPage: React.FC<IdeaDetailPageProps> = ({ slug }) => {
  const interactiveIdea: InteractiveIdea | undefined = findInteractiveIdea(slug);

  if (interactiveIdea === undefined) {
    return <ArticleDetailPage slug={slug} />;
  }

  const InteractiveExperience: React.ComponentType = interactiveIdea.Component;
  return <InteractiveExperience />;
};
