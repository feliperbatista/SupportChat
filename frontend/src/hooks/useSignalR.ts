'use client';

import { getConnection, startConnection } from '@/services/signalr';
import { useConversationStore } from '@/store/conversationStore';
import { Conversation, Message } from '@/types';
import { useEffect } from 'react';

export function useSignalR() {
  const {
    addMessageToConversation,
    updateMessageStatus,
    addReaction,
    removeReaction,
    addToQueue,
    moveToMine,
    updateContact,
  } = useConversationStore();

  useEffect(() => {
    const conn = getConnection();

    conn.on('NewMessage', (message: Message) => {
      addMessageToConversation(message);
    });

    conn.on(
      'MessageStatusUpdated',
      ({
        whatsAppMessageId,
        status,
      }: {
        whatsAppMessageId: string;
        status: string;
      }) => {
        updateMessageStatus(whatsAppMessageId, status);
      },
    );

    conn.on(
      'ReactionReceived',
      ({
        messageId,
        emoji,
        from,
      }: {
        messageId: string;
        emoji: string;
        from: string;
      }) => {
        addReaction(messageId, emoji, from, false);
      },
    );

    conn.on(
      'ReactionRemoved',
      ({ messageId, from }: { messageId: string; from: string }) => {
        removeReaction(messageId, from);
      },
    );

    conn.on('ConversationQueued', (conversation: Conversation) => {
      addToQueue(conversation);
    });

    conn.on(
      'ConversationAssigned',
      ({ conversationId }: { conversationId: string }) => {
        moveToMine(conversationId);
      },
    );

    conn.on(
      'ContactUpdated',
      ({ contactId, name }: { contactId: string; name: string }) =>
        updateContact(contactId, name),
    );

    startConnection().catch((err) =>
      console.error('[SignalR] Failed to connect:', err),
    );

    return () => {
      conn.off('NewMessage');
      conn.off('MessageStatusUpdated');
      conn.off('ReactionReceived');
      conn.off('ReactionRemoved');
      conn.off('ConversationQueued');
      conn.off('ConversationAssigned');
    };
  }, [
    addMessageToConversation,
    updateMessageStatus,
    addReaction,
    removeReaction,
    addToQueue,
    moveToMine,
    updateContact
  ]);
}

export async function joinConversation(conversationId: string) {
  const conn = getConnection();
  await conn.invoke('JoinConversation', conversationId);
}

export async function leaveConversation(conversationId: string) {
  const conn = getConnection();
  await conn.invoke('LeaveConversation', conversationId);
}
