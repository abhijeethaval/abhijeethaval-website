import React, { useEffect, useState } from 'react';
import { homeApi } from './homeApi';
import { HomeSummary } from './types';

export const HomePage: React.FC = () => {
  const [data, setData] = useState<HomeSummary | null>(null);
  const [loading, setLoading] = useState<boolean>(true);
  const [error, setError] = useState<string | null>(null);

  const fetchSummary = async () => {
    try {
      setLoading(true);
      setError(null);
      const summary = await homeApi.getSummary();
      setData(summary);
    } catch (err) {
      console.error(err);
      setError('Failed to load portfolio summary. Ensure the backend API is running.');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchSummary();
  }, []);

  return (
    <div className="home-container">
      <div className="gradient-bg"></div>
      
      <div className="content-wrapper">
        {loading && (
          <div className="loading-card glass-panel">
            <div className="shimmer shimmer-avatar animate-pulse"></div>
            <div className="shimmer shimmer-title animate-pulse"></div>
            <div className="shimmer shimmer-body animate-pulse"></div>
          </div>
        )}

        {error && (
          <div className="error-card glass-panel fade-in">
            <div className="error-icon">⚡</div>
            <h2>Connection Error</h2>
            <p>{error}</p>
            <button className="btn btn-primary" onClick={fetchSummary}>
              Retry Connection
            </button>
          </div>
        )}

        {!loading && !error && data && (
          <main className="profile-card glass-panel fade-in">
            <div className="profile-header">
              <div className="avatar-placeholder">
                {data.name.split(' ').map(n => n[0]).join('')}
              </div>
              <div className="profile-title">
                <h1 className="name-heading">{data.name}</h1>
              </div>
            </div>
            
            <div className="divider"></div>
            
            <div className="profile-body">
              <p className="summary-text">{data.summary}</p>
            </div>

            <div className="profile-footer">
              {data.headline
                .split(/[|,]/)
                .map(item => item.trim())
                .filter(Boolean)
                .map((badge, index) => (
                  <span key={index} className="badge">
                    {badge}
                  </span>
                ))}
            </div>
          </main>
        )}
      </div>
    </div>
  );
};
