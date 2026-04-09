'use client';

import ChatLayout from '@/components/ChatLayout';
import { useAuthStore } from '@/store/authStore';
import { useRouter } from 'next/navigation';
import { useEffect } from 'react';

export default function Home() {
  const router = useRouter();
  const agent = useAuthStore((s) => s.agent);
  const isAuthenticated = !!agent 

  useEffect(() => {
    if (!isAuthenticated) {
      router.push('/login');
    }
  }, [isAuthenticated, router]);

  if (!isAuthenticated) return null;

  return <ChatLayout />;
}
