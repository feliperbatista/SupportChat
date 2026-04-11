'use client';

import { joinConversation, leaveConversation } from '@/hooks/useSignalR';
import { Conversation } from '@/types';
import { format, isToday, isYesterday } from 'date-fns';
import Avatar from '../UI/Avatar';
import { useConversations } from '@/hooks/useConversations';
import { useMessages } from '@/hooks/useMessages';

interface Props {
  conversation: Conversation;
  isQueue: boolean;
}

function formatTime(dateStr: string) {
  const date = new Date(dateStr);
  if (isToday(date)) return format(date, 'HH:mm');
  if (isYesterday(date)) return 'Yesterday';
  return format(date, 'dd/MM/yyyy');
}

function getPreview(conv: Conversation) {
  const msg = conv.lastMessage;
  if (!msg) return 'No messages yet';
  if (msg.type === 'Image') return '🖼️ Image';
  if (msg.type === 'Audio') return '🎵 Audio message';
  if (msg.type === 'Video') return '📹 Video';
  if (msg.type === 'Document') return '📄 Document';
  if (msg.type === 'Sticker') return '🎭 Sticker';
  return msg.content;
}

export default function ConversationItem({ conversation, isQueue }: Props) {
  const { assignConversation, activeConversationId, setActiveConversation } =
    useConversations();
  const { getMessages } = useMessages();

  const isActive = activeConversationId == conversation.id;

  async function handleClick() {
    if (activeConversationId && activeConversationId !== conversation.id)
      await leaveConversation(activeConversationId);

    if (isQueue) {
      await assignConversation(conversation.id);
    }

    setActiveConversation(conversation.id);

    await getMessages(conversation.id);

    await joinConversation(conversation.id);
  }

  const preview = getPreview(conversation);
  const time = conversation.lastMessage
    ? formatTime(conversation.lastMessage.createdAt)
    : formatTime(conversation.updatedAt);

  return (
    <button
      onClick={handleClick}
      className={`w-full flex items-center gap-3 px-4 py-3 border-b border-wa-border text-left transition-colors cursor-pointer ${isActive ? 'bg-wa-hover' : 'hover:bg-wa-hover'}`}
    >
      <Avatar
        name={conversation.contact.name}
        imageUrl={conversation.contact.profilePictureUrl}
        size='md'
      />
      <div className='flex-1 min-w-0'>
        <div className='flex items-center justify-between gap-2'>
          <span className='text-wa-text  text-sm font-medium truncate'>
            {conversation.contact.name}
          </span>
          <span className='text-wa-muted text-xs shrink-0'>{time}</span>
        </div>

        <div className='flex items-center justify-between gap-2 mt-0.5'>
          <span className='text-wa-muted text-xs truncate'>{preview}</span>
          <div className='flex items-center gap-1 shrink-0'>
            {isQueue && (
              <span className='text-xs bg-wa-teal/20 text-wa-teal px-1.5 py-0.5 rounded-full'>
                Queue
              </span>
            )}
          </div>
        </div>
      </div>
    </button>
  );
}
