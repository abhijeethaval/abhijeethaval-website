import { defineConfig } from 'vite'
import react from '@vitejs/plugin-react'

// Declare process structure for TypeScript to avoid adding @types/node dependency
declare const process: {
  env: {
    PORT?: string;
    VITE_API_URL?: string;
    [key: string]: string | undefined;
  };
};

// https://vite.dev/config/
export default defineConfig({
  plugins: [react()],
  server: {
    port: process.env.PORT ? parseInt(process.env.PORT) : 5173,
    strictPort: true,
    host: true,
    proxy: {
      '/api': {
        target: process.env.VITE_API_URL || 'http://localhost:5297',
        changeOrigin: false,
        secure: false,
      }
    }
  }
})
