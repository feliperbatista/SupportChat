import { Agent } from "@/types";
import { create } from "zustand";
import { persist } from "zustand/middleware";

interface AuthState {
  agent: Agent | null;

  setAgent: (agent: Agent) => void;
  clearAuth: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      agent: null,

      setAgent: (agent) =>
        set({ agent }),

      clearAuth: () =>
        set({ agent: null }),
    }),
    { name: 'auth-storage' },
  ),
);