'use client';

import { useSignalR } from '@/hooks/useSignalR';
import api from '@/services/api';
import { useAuthStore } from '@/store/authStore';
import { useConversationStore } from '@/store/conversationStore';
import { Conversation } from '@/types';
import { useEffect } from 'react';
import ConversationList from './ConversationList/ConversationList';
import ChatPanel from './ChatPanel/ChatPanel';

export default function ChatLayout() {
  useSignalR();

  const agent = useAuthStore((s) => s.agent);
  const { setQueue, setMyConversations, activeConversationId } =
    useConversationStore();

  useEffect(() => {
    async function load() {
      try {
        const [queueRes, mineRes] = await Promise.all([
          api.get<Conversation[]>('/api/conversation/queue'),
          api.get<Conversation[]>('/api/conversation/mine'),
        ]);
        setQueue(queueRes.data);
        setMyConversations(mineRes.data);
      } catch (err) {
        console.log('Failed to load conversations', err);
      }
    }
    load();
  }, [setQueue, setMyConversations]);

  return (
    <div className='flex h-screen w-screen overflow-hidden bg-wa-bg'>
      <div className='w-95 min-w-95 flex flex-col border-r border-wa-border'>
        <ConversationList />
      </div>

      <div className='flex-1 flex flex-col min-w-0'>
        {activeConversationId ? (
          <ChatPanel conversationId={activeConversationId} />
        ) : (
          <EmptyState agentName={agent?.name} />
        )}
      </div>
    </div>
  );
}

function EmptyState({ agentName }: { agentName?: string }) {
  return (
    <div className='flex-1 flex flex-col items-center justify-center gap-4 text-wa-muted'>
      <div className='w-20 h-20 rounded-full bg-wa-panel flex items-center justify-center'>
        <span className='text-4xl'>💬</span>
      </div>
      <div className='text-center'>
        <p className='text-wa-text text-lg font-medium'>
          Welcome{agentName ? `, ${agentName}` : ''}
        </p>
        <p className='text-sm mt-1'>
          Select a conversation or pick one from the queue
        </p>
      </div>
    </div>
  );
}
