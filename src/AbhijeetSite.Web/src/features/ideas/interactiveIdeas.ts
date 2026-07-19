import { ComponentType } from 'react';
import { AiNativeSoftwareArchitectureExperience } from
  './experiences/ai-native-software-architecture/AiNativeSoftwareArchitectureExperience';

export interface InteractiveIdea {
  readonly Component: ComponentType;
  readonly format: 'Interactive thesis';
  readonly publishedAt: string;
  readonly slug: string;
  readonly summary: string;
  readonly title: string;
}

export const FEATURED_INTERACTIVE_IDEA: InteractiveIdea = {
  Component: AiNativeSoftwareArchitectureExperience,
  format: 'Interactive thesis',
  publishedAt: '2026-07-19',
  slug: 'ai-native-software-architecture',
  summary: 'Why enterprise AI needs a strongly typed, deterministic domain core—and '
    + 'how a soft intelligence shell makes previously impractical software possible.',
  title: 'Software Architecture in the AI Era',
};

export const INTERACTIVE_IDEAS: ReadonlyArray<InteractiveIdea> = [
  FEATURED_INTERACTIVE_IDEA,
];

export const findInteractiveIdea = (slug: string): InteractiveIdea | undefined => {
  return INTERACTIVE_IDEAS.find((idea) => idea.slug === slug);
};

export const isInteractiveIdeaSlug = (slug: string): boolean => {
  return findInteractiveIdea(slug) !== undefined;
};
