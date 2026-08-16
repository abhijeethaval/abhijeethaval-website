import React from 'react';
import { ClientLink } from './ClientLink';

interface SiteBackLinkProps extends Omit<
  React.AnchorHTMLAttributes<HTMLAnchorElement>,
  'href'
> {
  readonly href: string;
}

export const SiteBackLink: React.FC<SiteBackLinkProps> = ({
  children,
  className,
  href,
  ...anchorProps
}) => {
  const linkClassName: string = className === undefined
    ? 'site-back-link'
    : `site-back-link ${className}`;
  return (
    <ClientLink {...anchorProps} className={linkClassName} href={href}>
      <span aria-hidden="true">←</span>{children}
    </ClientLink>
  );
};
