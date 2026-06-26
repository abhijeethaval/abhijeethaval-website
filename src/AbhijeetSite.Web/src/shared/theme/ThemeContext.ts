import { createContext, useContext } from 'react';

export type ThemeName = 'dark' | 'light';

export interface ThemeContextValue {
  readonly theme: ThemeName;
  readonly toggleTheme: () => void;
}

const THEME_CONTEXT_ERROR = 'useTheme must be called inside ThemeProvider.';

export const ThemeContext = createContext<ThemeContextValue | null>(null);

export const useTheme = (): ThemeContextValue => {
  const value: ThemeContextValue | null = useContext(ThemeContext);

  if (value === null) {
    throw new Error(THEME_CONTEXT_ERROR);
  }

  return value;
};
