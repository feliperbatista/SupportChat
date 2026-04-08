'use client';

import { useState } from 'react';
import { useRouter } from 'next/navigation';
import { MessageSquare, Loader2 } from 'lucide-react';
import api from '@/services/api';
import { useAuthStore } from '@/store/authStore';
import { Agent } from '@/types';

export default function LoginPage() {
  const router = useRouter();
  const setAgent = useAuthStore((s) => s.setAgent);

  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  async function handleLogin(e: React.FormEvent) {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      const { data } = await api.post<Agent>('/api/auth/login', {
        email,
        password,
      });

      setAgent(data);
      router.push('/');
      // eslint-disable-next-line @typescript-eslint/no-explicit-any
    } catch (err: any) {
      setError(err.response?.data?.error ?? 'Invalid email or password.');
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className='min-h-screen bg-wa-bg flex items-center justify-center p-4'>
      <div className='w-full max-w-md'>
        <div className='flex flex-col mb-2 items-center gap-3'>
          <div className='w-16 h-16 bg-wa-teal rounded-full flex items-center justify-center'>
            <MessageSquare className='w-8 h-8 text-white' />
          </div>
          <h1 className='text-2xl font-semibold text-wa-text'>Support Chat</h1>
          <p className='text-wa-muted text-sm'>Sign in to your agent account</p>
        </div>

        <div className='bg-wa-panel rounded-xl p-8 shadow-xl'>
          <form onSubmit={handleLogin} className='flex flex-col gap-5'>
            <div className='flex flex-col gap-1.5'>
              <label className='text-sm text-wa-muted'>Email</label>
              <input
                type='email'
                value={email}
                onChange={(e) => setEmail(e.target.value)}
                placeholder='agent@company.com'
                required
                className='bg-wa-input text-wa-text rounded-lg px-4 py-3 text-sm
                           outline-none border border-transparent
                           focus:border-wa-teal transition-colors
                           placeholder:text-wa-muted'
              />
            </div>

            <div className='flex flex-col gap-1.5'>
              <label className='text-sm text-wa-muted'>Password</label>
              <input
                type='password'
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                placeholder='••••••••'
                required
                className='bg-wa-input text-wa-text rounded-lg px-4 py-3 text-sm
                           outline-none border border-transparent
                           focus:border-wa-teal transition-colors
                           placeholder:text-wa-muted'
              />
            </div>

            {error && (
              <div className='bg-red-500/10 border border-red-500/20 rounded-lg px-4 py-3'>
                <p className='text-wa-red text-sm text-center'>{error}</p>
              </div>
            )}

            <button
              type='submit'
              disabled={loading}
              className='bg-wa-teal hover:bg-teal-500 disabled:opacity-50
                         disabled:cursor-not-allowed text-white font-medium
                         rounded-lg py-3 transition-colors flex items-center
                         justify-center gap-2'
            >
              {loading && <Loader2 className='w-4 h-4 animate-spin' />}
              {loading ? 'Signing in...' : 'Sign in'}
            </button>
          </form>
        </div>
      </div>
    </div>
  );
}
