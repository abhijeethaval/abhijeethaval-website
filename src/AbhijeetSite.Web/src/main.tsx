import { StrictMode } from 'react';
import { createRoot } from 'react-dom/client';
import { ThemeProvider } from './shared/theme/ThemeProvider';
import { initializeBrowserTelemetry } from './shared/telemetry/browserTelemetry';
import './index.css';
import App from './App.tsx';

const ROOT_ELEMENT_ID = 'root';
void initializeBrowserTelemetry().catch(() => {
  console.error('Browser telemetry initialization failed. Verify runtime telemetry configuration.');
});
const rootElement: HTMLElement | null = document.getElementById(ROOT_ELEMENT_ID);

if (rootElement === null) {
  throw new Error(`Unable to start React app: #${ROOT_ELEMENT_ID} was not found.`);
}

createRoot(rootElement).render(
  <StrictMode>
    <ThemeProvider>
      <App />
    </ThemeProvider>
  </StrictMode>,
);
