import React from 'react';
import ReactMarkdown from 'react-markdown';
import type { Components } from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { ArchitectureDiagram } from './ArchitectureDiagram';

interface MarkdownDocumentProps {
  readonly markdown: string;
}

const MARKDOWN_COMPONENTS: Components = {
  a: ({ children, href }) => <MarkdownLink href={href}>{children}</MarkdownLink>,
  code: ({ children, className }) => (
    <MarkdownCode className={className}>{children}</MarkdownCode>
  ),
  h1: () => null,
};

export const MarkdownDocument: React.FC<MarkdownDocumentProps> = ({ markdown }) => {
  return (
    <ReactMarkdown components={MARKDOWN_COMPONENTS} remarkPlugins={[remarkGfm]}>
      {markdown}
    </ReactMarkdown>
  );
};

const MarkdownLink: React.FC<{
  readonly children: React.ReactNode;
  readonly href: string | undefined;
}> = ({ children, href }) => {
  const isExternal: boolean = href?.startsWith('https://') === true;
  return (
    <a
      href={href}
      rel={isExternal ? 'noopener noreferrer' : undefined}
      target={isExternal ? '_blank' : undefined}
    >
      {children}
    </a>
  );
};

const MarkdownCode: React.FC<{
  readonly children: React.ReactNode;
  readonly className: string | undefined;
}> = ({ children, className }) => {
  const source: string = String(children).replace(/\n$/, '');
  return className === 'language-mermaid'
    ? <ArchitectureDiagram source={source} />
    : <code className={className}>{children}</code>;
};
