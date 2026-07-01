import React, { useEffect, useState } from 'react';
import { authApi } from '../auth/authApi';
import type { AuthenticatedUser, AuthSession, CurrentUserResponse } from '../auth/types';
import { ArticleStatusScreen } from '../articles/ArticleStatusScreen';
import { SiteHeader } from '../../shared/navigation/SiteHeader';
import { adminArticlesApi } from './adminArticlesApi';
import type {
  ArticleDraft,
  ArticleDraftSummary,
  CreateArticleDraftPayload,
  UpdateArticleDraftPayload,
} from './types';

interface DraftFormState {
  readonly id: string | null;
  readonly title: string;
  readonly slug: string;
  readonly summary: string;
  readonly mdxSource: string;
  readonly version: number | null;
  readonly isPublished: boolean;
}

const emptyForm: DraftFormState = {
  id: null,
  title: '',
  slug: '',
  summary: '',
  mdxSource: '# New article\n\nStart drafting here.',
  version: null,
  isPublished: false,
};

export const AdminArticlesPage: React.FC = () => {
  const [session, setSession] = useState<AuthSession>({ status: 'loading' });
  const [drafts, setDrafts] = useState<ReadonlyArray<ArticleDraftSummary>>([]);
  const [form, setForm] = useState<DraftFormState>(emptyForm);
  const [message, setMessage] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isBusy, setIsBusy] = useState<boolean>(false);

  useEffect(() => {
    void loadAdminState(setSession, setDrafts, setError);
  }, []);

  if (session.status === 'loading') {
    return <ArticleStatusScreen title="Loading admin" message="Checking session." />;
  }

  if (session.status !== 'authenticated' || !session.user.isAdmin) {
    return <AdminAccessScreen session={session} />;
  }

  return (
    <>
      <SiteHeader activeSection="none" />
      <main className="admin-page">
        <AdminHeader user={session.user} />
        <AdminWorkspace
          drafts={drafts}
          form={form}
          message={message}
          error={error}
          isBusy={isBusy}
          onDraftSelect={(id: string) => void selectDraft(id, setForm, setError, setMessage)}
          onNewDraft={() => resetDraft(setForm, setError, setMessage)}
          onFieldChange={setForm}
          onSave={() => void saveDraft(form, setForm, setDrafts, setMessage, setError, setIsBusy)}
          onPublish={() => void publishDraft(form, setForm, setDrafts, setMessage, setError, setIsBusy)}
        />
      </main>
    </>
  );
};

interface AdminAccessScreenProps {
  readonly session: AuthSession;
}

const AdminAccessScreen: React.FC<AdminAccessScreenProps> = ({ session }) => {
  const message: string = session.status === 'authenticated'
    ? 'Admin access is required.'
    : 'Sign in with an administrator account.';

  return (
    <>
      <SiteHeader activeSection="none" />
      <main className="status-screen">
        <div className="error-panel">
          <h1>Articles admin</h1>
          <p>{message}</p>
          <a className="primary-link" href={authApi.getGoogleLoginUrl()}>Sign in</a>
        </div>
      </main>
    </>
  );
};

interface AdminHeaderProps {
  readonly user: AuthenticatedUser;
}

const AdminHeader: React.FC<AdminHeaderProps> = ({ user }) => {
  return (
    <header className="admin-header section-band">
      <div className="section-shell">
        <p className="eyebrow">Articles admin</p>
        <h1>Draft and publish articles</h1>
        <p>{user.displayName}</p>
      </div>
    </header>
  );
};

interface AdminWorkspaceProps {
  readonly drafts: ReadonlyArray<ArticleDraftSummary>;
  readonly form: DraftFormState;
  readonly message: string | null;
  readonly error: string | null;
  readonly isBusy: boolean;
  readonly onDraftSelect: (id: string) => void;
  readonly onNewDraft: () => void;
  readonly onFieldChange: React.Dispatch<React.SetStateAction<DraftFormState>>;
  readonly onSave: () => void;
  readonly onPublish: () => void;
}

const AdminWorkspace: React.FC<AdminWorkspaceProps> = ({
  drafts,
  form,
  message,
  error,
  isBusy,
  onDraftSelect,
  onNewDraft,
  onFieldChange,
  onSave,
  onPublish,
}) => {
  return (
    <section className="admin-workspace section-band">
      <div className="admin-shell">
        <DraftList drafts={drafts} selectedId={form.id} onDraftSelect={onDraftSelect} />
        <DraftEditor
          form={form}
          message={message}
          error={error}
          isBusy={isBusy}
          onNewDraft={onNewDraft}
          onFieldChange={onFieldChange}
          onSave={onSave}
          onPublish={onPublish}
        />
      </div>
    </section>
  );
};

interface DraftListProps {
  readonly drafts: ReadonlyArray<ArticleDraftSummary>;
  readonly selectedId: string | null;
  readonly onDraftSelect: (id: string) => void;
}

const DraftList: React.FC<DraftListProps> = ({ drafts, selectedId, onDraftSelect }) => {
  return (
    <aside className="admin-draft-list" aria-label="Article drafts">
      {drafts.map((draft: ArticleDraftSummary) => (
        <button
          className="admin-draft-row"
          type="button"
          key={draft.id}
          aria-current={draft.id === selectedId ? 'true' : undefined}
          onClick={() => onDraftSelect(draft.id)}
        >
          <span>{draft.title}</span>
          <small>{draft.status} - v{draft.version}</small>
        </button>
      ))}
      {drafts.length === 0 ? <p className="admin-empty">No drafts yet.</p> : null}
    </aside>
  );
};

interface DraftEditorProps {
  readonly form: DraftFormState;
  readonly message: string | null;
  readonly error: string | null;
  readonly isBusy: boolean;
  readonly onNewDraft: () => void;
  readonly onFieldChange: React.Dispatch<React.SetStateAction<DraftFormState>>;
  readonly onSave: () => void;
  readonly onPublish: () => void;
}

const DraftEditor: React.FC<DraftEditorProps> = ({
  form,
  message,
  error,
  isBusy,
  onNewDraft,
  onFieldChange,
  onSave,
  onPublish,
}) => {
  return (
    <section className="admin-editor" aria-label="Draft editor">
      <EditorToolbar form={form} isBusy={isBusy} onNewDraft={onNewDraft} onSave={onSave} onPublish={onPublish} />
      <label>
        Title
        <input value={form.title} onChange={(event) => updateField(onFieldChange, 'title', event.target.value)} />
      </label>
      <label>
        Slug
        <input
          value={form.slug}
          disabled={form.isPublished}
          onChange={(event) => updateField(onFieldChange, 'slug', event.target.value)}
        />
      </label>
      <label>
        Summary
        <textarea
          rows={3}
          value={form.summary}
          onChange={(event) => updateField(onFieldChange, 'summary', event.target.value)}
        />
      </label>
      <label>
        Source
        <textarea
          className="admin-source"
          rows={18}
          value={form.mdxSource}
          onChange={(event) => updateField(onFieldChange, 'mdxSource', event.target.value)}
        />
      </label>
      {message !== null ? <p className="admin-message">{message}</p> : null}
      {error !== null ? <p className="admin-error">{error}</p> : null}
    </section>
  );
};

interface EditorToolbarProps {
  readonly form: DraftFormState;
  readonly isBusy: boolean;
  readonly onNewDraft: () => void;
  readonly onSave: () => void;
  readonly onPublish: () => void;
}

const EditorToolbar: React.FC<EditorToolbarProps> = ({
  form,
  isBusy,
  onNewDraft,
  onSave,
  onPublish,
}) => {
  return (
    <div className="admin-toolbar">
      <div>
        <strong>{form.id === null ? 'New draft' : `Version ${form.version ?? 0}`}</strong>
        <span>{form.isPublished ? 'Published' : 'Draft'}</span>
      </div>
      <button type="button" onClick={onNewDraft} disabled={isBusy}>New</button>
      <button type="button" onClick={onSave} disabled={isBusy}>Save</button>
      <button type="button" onClick={onPublish} disabled={isBusy || form.id === null}>Publish</button>
    </div>
  );
};

const loadAdminState = async (
  setSession: React.Dispatch<React.SetStateAction<AuthSession>>,
  setDrafts: React.Dispatch<React.SetStateAction<ReadonlyArray<ArticleDraftSummary>>>,
  setError: React.Dispatch<React.SetStateAction<string | null>>,
): Promise<void> => {
  try {
    const response: CurrentUserResponse = await authApi.getCurrentUser();
    const nextSession: AuthSession = toAuthSession(response);
    setSession(nextSession);
    if (nextSession.status === 'authenticated' && nextSession.user.isAdmin) {
      setDrafts(await adminArticlesApi.getDrafts());
    }
  } catch (errorValue) {
    setSession({ status: 'error' });
    setError(getErrorMessage(errorValue));
  }
};

const selectDraft = async (
  id: string,
  setForm: React.Dispatch<React.SetStateAction<DraftFormState>>,
  setError: React.Dispatch<React.SetStateAction<string | null>>,
  setMessage: React.Dispatch<React.SetStateAction<string | null>>,
): Promise<void> => {
  setError(null);
  setMessage(null);
  try {
    setForm(toForm(await adminArticlesApi.getDraft(id)));
  } catch (errorValue) {
    setError(getErrorMessage(errorValue));
  }
};

const resetDraft = (
  setForm: React.Dispatch<React.SetStateAction<DraftFormState>>,
  setError: React.Dispatch<React.SetStateAction<string | null>>,
  setMessage: React.Dispatch<React.SetStateAction<string | null>>,
): void => {
  setForm(emptyForm);
  setError(null);
  setMessage(null);
};

const saveDraft = async (
  form: DraftFormState,
  setForm: React.Dispatch<React.SetStateAction<DraftFormState>>,
  setDrafts: React.Dispatch<React.SetStateAction<ReadonlyArray<ArticleDraftSummary>>>,
  setMessage: React.Dispatch<React.SetStateAction<string | null>>,
  setError: React.Dispatch<React.SetStateAction<string | null>>,
  setIsBusy: React.Dispatch<React.SetStateAction<boolean>>,
): Promise<void> => {
  await runDraftAction(setMessage, setError, setIsBusy, async () => {
    const savedDraft: ArticleDraft = form.id === null
      ? await adminArticlesApi.createDraft(toCreatePayload(form))
      : await adminArticlesApi.updateDraft(form.id, toUpdatePayload(form));
    setForm(toForm(savedDraft));
    setDrafts(await adminArticlesApi.getDrafts());
    setMessage('Draft saved.');
  });
};

const publishDraft = async (
  form: DraftFormState,
  setForm: React.Dispatch<React.SetStateAction<DraftFormState>>,
  setDrafts: React.Dispatch<React.SetStateAction<ReadonlyArray<ArticleDraftSummary>>>,
  setMessage: React.Dispatch<React.SetStateAction<string | null>>,
  setError: React.Dispatch<React.SetStateAction<string | null>>,
  setIsBusy: React.Dispatch<React.SetStateAction<boolean>>,
): Promise<void> => {
  await runDraftAction(setMessage, setError, setIsBusy, async () => {
    if (form.id === null || form.version === null) {
      throw new Error('Save the draft before publishing.');
    }

    await adminArticlesApi.publishDraft(form.id, { expectedVersion: form.version });
    setForm(toForm(await adminArticlesApi.getDraft(form.id)));
    setDrafts(await adminArticlesApi.getDrafts());
    setMessage('Draft published.');
  });
};

const runDraftAction = async (
  setMessage: React.Dispatch<React.SetStateAction<string | null>>,
  setError: React.Dispatch<React.SetStateAction<string | null>>,
  setIsBusy: React.Dispatch<React.SetStateAction<boolean>>,
  action: () => Promise<void>,
): Promise<void> => {
  setIsBusy(true);
  setError(null);
  setMessage(null);
  try {
    await action();
  } catch (errorValue) {
    setError(getErrorMessage(errorValue));
  } finally {
    setIsBusy(false);
  }
};

const updateField = (
  setForm: React.Dispatch<React.SetStateAction<DraftFormState>>,
  key: keyof Pick<DraftFormState, 'title' | 'slug' | 'summary' | 'mdxSource'>,
  value: string,
): void => {
  setForm((current: DraftFormState) => ({ ...current, [key]: value }));
};

const toAuthSession = (response: CurrentUserResponse): AuthSession => {
  if (response.isAuthenticated && response.user !== null) {
    return { status: 'authenticated', user: response.user };
  }

  return { status: 'anonymous' };
};

const toForm = (draft: ArticleDraft): DraftFormState => {
  return {
    id: draft.id,
    title: draft.title,
    slug: draft.slug,
    summary: draft.summary,
    mdxSource: draft.mdxSource,
    version: draft.version,
    isPublished: draft.isPublished,
  };
};

const toCreatePayload = (form: DraftFormState): CreateArticleDraftPayload => {
  return {
    title: form.title,
    slug: form.slug,
    summary: form.summary,
    mdxSource: form.mdxSource,
  };
};

const toUpdatePayload = (form: DraftFormState): UpdateArticleDraftPayload => {
  if (form.version === null) {
    throw new Error('Draft version is missing. Reload the draft before saving.');
  }

  return {
    ...toCreatePayload(form),
    expectedVersion: form.version,
  };
};

const getErrorMessage = (errorValue: unknown): string => {
  return errorValue instanceof Error
    ? errorValue.message
    : 'The article admin action failed due to an unknown client error.';
};
