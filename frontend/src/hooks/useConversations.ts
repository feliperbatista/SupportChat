'use client';

import api from '@/services/api';
import { useConversationStore } from '@/store/conversationStore';
import { Conversation } from '@/types';
import { useCallback, useEffect, useState } from 'react';

export function useConversations() {
  const {
    queue,
    myConversations,
    setQueue,
    setMyConversations,
    activeConversationId,
    setActiveConversation,
    removeFromQueue,
    messages,
    updateContact,
  } = useConversationStore();

  const [loading, setLoading] = useState(false);

  const refresh = useCallback(async () => {
    setLoading(true);
    try {
      const [queueRes, minRes] = await Promise.all([
        api.get<Conversation[]>('/api/conversation/queue'),
        api.get<Conversation[]>('/api/conversation/mine'),
      ]);
      setQueue(queueRes.data);
      setMyConversations(minRes.data);
    } catch (error) {
      console.log(error);
    } finally {
      setLoading(false);
    }
  }, [setQueue, setMyConversations]);

  const assignConversation = useCallback(
    async (conversationId: string) => {
      const { data } = await api.patch<Conversation>(
        `/api/conversation/${conversationId}/assign`,
      );

      removeFromQueue(conversationId);
      setMyConversations([
        data,
        ...myConversations.filter((c) => c.id !== data.id),
      ]);
      setActiveConversation(conversationId);
      return data;
    },
    [
      myConversations,
      removeFromQueue,
      setMyConversations,
      setActiveConversation,
    ],
  );

  const updateContactName = useCallback(
    async (contactId: string, name: string) => {
      await api.patch(`/api/contact/${contactId}`, {
        name: name,
      });

      updateContact(contactId, name);
    },
    [updateContact],
  );

  useEffect(() => {
    refresh();
  }, [refresh]);

  const activeConversation =
    myConversations.find((c) => c.id === activeConversationId) ??
    queue.find((c) => c.id === activeConversationId) ??
    null;

  return {
    loading,
    queue,
    myConversations,
    activeConversationId,
    activeConversation,
    setActiveConversation,
    refresh,
    assignConversation,
    messages,
    updateContactName
  };
}
