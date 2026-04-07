import { Agent } from '@/types';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  agent: Agent | null;
  setAuth: (token: string, agent: Agent) => void;
  clearAuth: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      agent: null,

      setAuth: (token, agent) => {
        localStorage.setItem('token', token);
        set({ token, agent });
      },

      clearAuth: () => {
        localStorage.removeItem('token');
        set({ token: null, agent: null });
      },

      isAuthenticated: () => !!get().token,
    }),
    { name: 'auth-storage' },
  ),
);
