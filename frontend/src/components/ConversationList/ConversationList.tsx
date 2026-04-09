'use client';

import api from '@/services/api';
import { useAuthStore } from '@/store/authStore';
import { useConversationStore } from '@/store/conversationStore';
import { useRouter } from 'next/navigation';
import { useState } from 'react';
import Avatar from '../UI/Avatar';
import { LogOut, Search } from 'lucide-react';
import ConversationItem from './ConversationItem';

type Tab = 'mine' | 'queue';

export default function ConversationList() {
  const router = useRouter();
  const agent = useAuthStore((s) => s.agent);
  const clearAuth = useAuthStore((s) => s.clearAuth);

  const { myConversations, queue } = useConversationStore();

  const [tab, setTab] = useState<Tab>('mine');
  const [search, setSearch] = useState('');

  const conversations = tab === 'mine' ? myConversations : queue;

  const filtered = conversations.filter(
    (c) =>
      c.contact.name.toLowerCase().includes(search.toLowerCase()) ||
      c.contact.phoneNumber.includes(search),
  );

  const sorted = filtered.sort(
    (a, b) =>
      (new Date(b.lastMessage?.createdAt || 0).getTime() || 0) -
      (new Date(a.lastMessage?.createdAt || 0).getTime() || 0),
  );

  async function handleLogout() {
    await api.post('/api/auth/logout');
    clearAuth();
    router.push('/login');
  }

  return (
    <div className='flex flex-col h-full bg-wa-panel'>
      <div className='flex items-center justify-between px-4 py-3 h-15'>
        {agent && (
          <Avatar name={agent.name} imageUrl={agent.avatarUrl} size='md' />
        )}
        <div className='flex items-center gap-3'>
          <span className='text-wa-text font-medium text-sm'>
            {agent?.name}
          </span>
          <button
            onClick={handleLogout}
            title='Logout'
            className='text-wa-muted hover:text-wa-text transition-colors'
          >
            <LogOut />
          </button>
        </div>
      </div>

      <div className='px-3 py-2'>
        <div className='flex items-center gap-2 bg-wa-search rounded-lg px-3 py-2'>
          <Search className='w-4 h-4 text-wa-muted shrink-0' />
          <input
            type='text'
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder='Search conversations'
            className='bg-transparent text-wa-text text-sm outline-none placeholder:text-wa-muted w-full'
          />
        </div>
      </div>

      <div className='flex border-b border-wa-border'>
        <button
          onClick={() => setTab('mine')}
          className={`flex-1 py-2.5 text-sm font-medium transition-colors border-b-2 ${tab === 'mine' ? 'border-wa-teal text-wa-teal' : 'border-transparent text-wa-muted hover:text-wa-text'}`}
        >
          My Chats{' '}
          {myConversations.length > 0 && (
            <span className='ml-1.5 text-xs'>{myConversations.length}</span>
          )}
        </button>
        <button
          onClick={() => setTab('queue')}
          className={`flex-1 py-2.5 text-sm font-medium transition-colors border-b-2 ${tab === 'queue' ? 'border-wa-teal text-wa-teal' : 'border-transparent text-wa-muted hover:text-wa-text'}`}
        >
          Queue{' '}
          {queue.length > 0 && (
            <span className='ml-2 bg-wa-teal text-white text-xs rounded-full px-1.5 py-0.5'>
              {queue.length}
            </span>
          )}
        </button>
      </div>

      <div className='flex-1 overflow-y-auto'>
        {sorted.length === 0 ? (
          <div className='flex flex-col items-center justify-center h-32 text-wa-muted text-sm'>
            {search
              ? 'No results found'
              : tab === 'mine'
                ? 'No active conversations'
                : 'Queue is empty'}
          </div>
        ) : (
          sorted.map((conv) => (
            <ConversationItem
              key={conv.id}
              conversation={conv}
              isQueue={tab === 'queue'}
            />
          ))
        )}
      </div>
    </div>
  );
}
