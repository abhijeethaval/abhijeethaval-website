export const CLIENT_NAVIGATION_EVENT = 'app:client-navigation';

export const navigateToRoute = (href: string): void => {
  const currentHref: string = `${window.location.pathname}${window.location.search}${window.location.hash}`;
  if (currentHref === href) {
    return;
  }

  window.history.pushState(null, '', href);
  window.dispatchEvent(new Event(CLIENT_NAVIGATION_EVENT));
  window.scrollTo({ behavior: 'auto', left: 0, top: 0 });
};
