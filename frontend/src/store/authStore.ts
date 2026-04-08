import { Agent } from "@/types";
import { create } from "zustand";
import { persist } from "zustand/middleware";

interface AuthState {
  agent: Agent | null;
  isAuthenticated: boolean;

  setAgent: (agent: Agent) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      agent: null,
      isAuthenticated: false,

      setAgent: (agent) =>
        set({ agent, isAuthenticated: true }),

      clearAuth: () =>
        set({ agent: null, isAuthenticated: false }),
    }),
    { name: 'auth-storage' },
  ),
);