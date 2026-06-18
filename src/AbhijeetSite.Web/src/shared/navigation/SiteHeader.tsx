import React from 'react';

type ActiveSection = 'architecture' | 'none' | 'profile';

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
    { href: `${homePrefix}#education`, label: 'Education', isActive: false },
  ];
};
