import { Agent } from '@/types';
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  agent: Agent | null;
  setAuth: (agent: Agent) => void;
  clearAuth: () => void;
  isAuthenticated: () => boolean;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set, get) => ({
      token: null,
      agent: null,

      setAuth: (agent) => {
        set({ agent });
      },

      clearAuth: () => {
        set({ agent: null });
      },

      isAuthenticated: () => !!get().agent,
    }),
    { name: 'auth-storage' },
  ),
);
