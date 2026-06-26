import React, { useEffect, useMemo, useState } from 'react';
import { ThemeContext, ThemeContextValue, ThemeName } from './ThemeContext';

const DEFAULT_THEME: ThemeName = 'light';
const DARK_THEME: ThemeName = 'dark';
const LIGHT_THEME: ThemeName = 'light';
const STORAGE_KEY = 'abhijeet-site-theme';
const LIGHT_THEME_COLOR = '#f3f2ef';
const DARK_THEME_COLOR = '#101827';

interface ThemeProviderProps {
  readonly children: React.ReactNode;
}

export const ThemeProvider: React.FC<ThemeProviderProps> = ({ children }) => {
  const [theme, setTheme] = useState<ThemeName>(readStoredTheme);

  useEffect(() => {
    applyTheme(theme);
    storeTheme(theme);
  }, [theme]);

  const value: ThemeContextValue = useMemo(
    () => ({ theme, toggleTheme: () => setTheme(getNextTheme) }),
    [theme],
  );

  return <ThemeContext.Provider value={value}>{children}</ThemeContext.Provider>;
};

const getNextTheme = (theme: ThemeName): ThemeName => {
  return theme === LIGHT_THEME ? DARK_THEME : LIGHT_THEME;
};

const readStoredTheme = (): ThemeName => {
  try {
    const storedTheme: string | null = window.localStorage.getItem(STORAGE_KEY);
    return isThemeName(storedTheme) ? storedTheme : DEFAULT_THEME;
  } catch (errorValue) {
    console.warn(getStorageErrorMessage('read', errorValue));
    return DEFAULT_THEME;
  }
};

const storeTheme = (theme: ThemeName): void => {
  try {
    window.localStorage.setItem(STORAGE_KEY, theme);
  } catch (errorValue) {
    console.warn(getStorageErrorMessage('save', errorValue));
  }
};

const applyTheme = (theme: ThemeName): void => {
  document.documentElement.dataset.theme = theme;
  updateThemeColor(theme);
};

const updateThemeColor = (theme: ThemeName): void => {
  const themeColor: HTMLMetaElement | null = document.querySelector('meta[name="theme-color"]');

  if (themeColor === null) {
    return;
  }

  themeColor.content = theme === DARK_THEME ? DARK_THEME_COLOR : LIGHT_THEME_COLOR;
};

const isThemeName = (value: string | null): value is ThemeName => {
  return value === DARK_THEME || value === LIGHT_THEME;
};

const getStorageErrorMessage = (operation: string, errorValue: unknown): string => {
  if (errorValue instanceof Error) {
    return `Unable to ${operation} theme preference: ${errorValue.message}`;
  }

  return `Unable to ${operation} theme preference because the browser returned an unknown error.`;
};
