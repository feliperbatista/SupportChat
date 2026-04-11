'use client';

import { Message } from '@/types';
import { format, isToday, isYesterday } from 'date-fns';
import { useEffect, useRef, useState } from 'react';
import Avatar from '../UI/Avatar';
import { MoreVertical, Search } from 'lucide-react';
import MessageBubble from './MessageBubble';
import MessageInput from '../MessageInput/MessageInput';
import ContactSidebar from './ContactSidebar';
import { useConversations } from '@/hooks/useConversations';

interface Props {
  conversationId: string;
}

function getDateLabel(dateStr: string) {
  const date = new Date(dateStr);
  if (isToday(date)) return 'Today';
  if (isYesterday(date)) return 'Yesterday';
  return format(date, 'MMMM d, yyyy');
}

function groupMessagesByDate(messages: Message[]) {
  const groups: { label: string; messages: Message[] }[] = [];
  let currentLabel = '';

  for (const msg of messages) {
    const label = getDateLabel(msg.createdAt);
    if (label !== currentLabel) {
      currentLabel = label;
      groups.push({ label, messages: [msg] });
    } else {
      groups[groups.length - 1].messages.push(msg);
    }
  }
  return groups;
}

export default function ChatPanel({ conversationId }: Props) {
  const bottomRef = useRef<HTMLDivElement>(null);
  const [showSidebar, setShowSidebar] = useState(false);

  const { myConversations, queue, messages } = useConversations();

  const conversation =
    myConversations.find((c) => c.id === conversationId) ||
    queue.find((c) => c.id === conversationId);

  const convMessages = messages[conversationId] ?? [];
  const groups = groupMessagesByDate(convMessages);

  useEffect(() => {
    bottomRef.current?.scrollIntoView({ behavior: 'smooth' });
  }, [convMessages.length]);

  if (!conversation) return null;

  return (
    <div className='flex h-full overflow-hidden'>
      <div className='flex flex-col flex-1 min-w-0'>
        <div className='flex items-center gap-3 px-4 h-15 bg-wa-panel border-b border-wa-border shrink-0'>
          <button
            onClick={() => setShowSidebar((v) => !v)}
            className='flex items-center gap-3 flex-1 min-w-0 hover:opacity-80 transition-opacity text-left'
          >
            <Avatar
              name={conversation.contact.name}
              imageUrl={conversation.contact.profilePictureUrl}
              size='md'
            />
            <div className='flex-1 min-w-0'>
              <p className='text-wa-text text-sm font-medium'>
                {conversation.contact.name}
              </p>
              <p className='text-wa-muted text-xs'>
                {conversation.contact.phoneNumber}
              </p>
            </div>
          </button>

          <div className='flex items-center gap-4 text-wa-muted'>
            <button className='hover:text-wa-text transition-colors'>
              <Search className='w-5 h-5' />
            </button>
            <button className='hover:text-wa-text transition-colors'>
              <MoreVertical className='w-5 h-5' />
            </button>
          </div>
        </div>

        <div
          className='flex-1 overflow-y-auto px-[8%] py-4 flex flex-col gap-1'
          style={{
            backgroundImage: `url("data:image/svg+xml,%3Csvg width='60' height='60' viewBox='0 0 60 60' xmlns='http://www.w3.org/2000/svg'%3E%3Cg fill='none' fill-rule='evenodd'%3E%3Cg fill='%23ffffff' fill-opacity='0.03'%3E%3Cpath d='M36 34v-4h-2v4h-4v2h4v4h2v-4h4v-2h-4zm0-30V0h-2v4h-4v2h4v4h2V6h4V4h-4zM6 34v-4H4v4H0v2h4v4h2v-4h4v-2H6zM6 4V0H4v4H0v2h4v4h2V6h4V4H6z'/%3E%3C/g%3E%3C/g%3E%3C/svg%3E")`,
          }}
        >
          {convMessages.length === 0 ? (
            <div className='flex-1 flex items-center justify-center'>
              <p className='text-wa-muted text-sm'>No messages yet</p>
            </div>
          ) : (
            groups.map((group) => (
              <div key={group.label}>
                <div className='flex items-center justify-center my-3'>
                  <span className='bg-wa-panel text-wa-muted text-xs px-3 py-1 rounded-full'>
                    {group.label}
                  </span>
                </div>

                <div className='flex flex-col gap-1'>
                  {group.messages.map((message) => (
                    <MessageBubble
                      key={message.id}
                      message={message}
                      conversationId={conversationId}
                    />
                  ))}
                </div>
              </div>
            ))
          )}
          <div ref={bottomRef} />
        </div>

        <div className='shrink-0'>
          <MessageInput conversationId={conversationId} />
        </div>
      </div>

      {showSidebar && (
        <ContactSidebar
          conversation={conversation}
          onClose={() => setShowSidebar(false)}
        />
      )}
    </div>
  );
}
