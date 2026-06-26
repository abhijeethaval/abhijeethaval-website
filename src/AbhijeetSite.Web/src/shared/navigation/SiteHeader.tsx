import React from 'react';
import { useTheme } from '../theme/ThemeContext';

type ActiveSection = 'architecture' | 'articles' | 'none' | 'profile';

interface SiteHeaderProps {
  activeSection: ActiveSection;
}

interface NavigationLink {
  href: string;
  label: string;
  isActive: boolean;
}

export const SiteHeader: React.FC<SiteHeaderProps> = ({ activeSection }) => {
  const links: ReadonlyArray<NavigationLink> = getNavigationLinks(activeSection);
  const homeHref: string = activeSection === 'profile' ? '#home' : '/';
  const { theme, toggleTheme } = useTheme();

  return (
    <header className="site-header">
      <a className="site-mark" href={homeHref} aria-label="Abhijeet Haval home">AH</a>
      <nav aria-label="Primary navigation">
        {links.map((link) => (
          <a
            key={link.label}
            href={link.href}
            aria-current={link.isActive ? 'page' : undefined}
          >
            {link.label}
          </a>
        ))}
      </nav>
      <button
        className="theme-toggle"
        type="button"
        aria-label={`Switch to ${theme === 'light' ? 'dark' : 'light'} theme`}
        aria-pressed={theme === 'dark'}
        onClick={toggleTheme}
      >
        {theme === 'light' ? 'Dark' : 'Light'}
      </button>
    </header>
  );
};

const getNavigationLinks = (activeSection: ActiveSection): ReadonlyArray<NavigationLink> => {
  const homePrefix: string = activeSection === 'profile' ? '' : '/';

  return [
    { href: `${homePrefix}#profile`, label: 'Profile', isActive: activeSection === 'profile' },
    { href: `${homePrefix}#experience`, label: 'Experience', isActive: false },
    {
      href: '/architecture',
      label: 'Architecture',
      isActive: activeSection === 'architecture',
    },
    { href: '/articles', label: 'Articles', isActive: activeSection === 'articles' },
    { href: `${homePrefix}#education`, label: 'Education', isActive: false },
  ];
};
