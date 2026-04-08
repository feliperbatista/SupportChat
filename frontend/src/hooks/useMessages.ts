'use client';

import api from '@/services/api';
import { useConversationStore } from '@/store/conversationStore';
import { Message } from '@/types';
import { useCallback } from 'react';

type SendPayload = {
  conversationId: string;
  content: string;
  type?: Message['type'];
  file?: File | null;
};

export function useMessages() {
  const { messages, setMessages } = useConversationStore();

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
    async ({ conversationId, content, type = 'Text', file }: SendPayload) => {
      const formData = new FormData();
      formData.append('content', content);
      formData.append('type', type);

      if (file) formData.append('file', file);

      const { data } = await api.post<Message>(
        `/api/conversation/${conversationId}/messages`,
        formData,
      );

      return data;
    },
    [],
  );

  return { messages, getMessages, sendMessage };
}
