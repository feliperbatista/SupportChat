'use client';

import api from '@/services/api';
import { useConversationStore } from '@/store/conversationStore';
import { Message } from '@/types';
import { useCallback } from 'react';

type MessagePayload = {
  conversationId: string;
  content: string;
  type?: Message['type'];
  file?: File | null;
  onProgress?: (e: number) => void;
};

type ReactionPayload = {
  conversationId: string;
  messageId: string;
  emoji: string;
  from: string;
};

export function useMessages() {
  const { messages, setMessages, addReaction } = useConversationStore();

  const getMessages = useCallback(
    async (conversationId: string) => {
      const { data } = await api.get<Message[]>(
        `/api/conversation/${conversationId}/messages`,
      );
      setMessages(conversationId, data);
      return data;
    },
    [setMessages],
  );

  const sendMessage = useCallback(
    async ({
      conversationId,
      content,
      type = 'Text',
      file,
      onProgress,
    }: MessagePayload) => {
      const formData = new FormData();
      formData.append('content', content);
      formData.append('type', type);

      if (file) formData.append('file', file);

      try {
        const { data } = await api.post<Message>(
          `/api/conversation/${conversationId}/messages`,
          formData,
          {
            onUploadProgress: onProgress
              ? (e) => {
                  if (e.total) {
                    onProgress(Math.round((e.loaded / e.total) * 100));
                  }
                }
              : undefined,
          },
        );
        return data;
      } catch (err) {
        console.error('Error sending message:', err);
      }
    },
    [],
  );

  const sendReaction = useCallback(
    async ({ conversationId, messageId, emoji, from }: ReactionPayload) => {
      try {
        await api.post(
          `/api/conversation/${conversationId}/messages/${messageId}/react`,
          { emoji },
        );
        addReaction(messageId, emoji, from, true);
      } catch (err) {
        console.error('Failed to send reaction', err);
      }
    },
    [addReaction],
  );

  return { messages, getMessages, sendMessage, sendReaction };
}
