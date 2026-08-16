import React from 'react';
import { navigateToRoute } from './clientNavigation';

interface ClientLinkProps extends React.AnchorHTMLAttributes<HTMLAnchorElement> {
  readonly href: string;
}

export const ClientLink: React.FC<ClientLinkProps> = ({
  download,
  href,
  onClick,
  target,
  ...anchorProps
}) => {
  const handleClick = (event: React.MouseEvent<HTMLAnchorElement>): void => {
    onClick?.(event);
    if (event.defaultPrevented || !isClientNavigation(event, download, target)) {
      return;
    }

    event.preventDefault();
    navigateToRoute(href);
  };

  return (
    <a {...anchorProps} download={download} href={href} onClick={handleClick} target={target} />
  );
};

const isClientNavigation = (
  event: React.MouseEvent<HTMLAnchorElement>,
  download: ClientLinkProps['download'],
  target: ClientLinkProps['target'],
): boolean => {
  return event.button === 0
    && !event.altKey
    && !event.ctrlKey
    && !event.metaKey
    && !event.shiftKey
    && download === undefined
    && (target === undefined || target === '_self');
};
