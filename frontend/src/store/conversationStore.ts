import { create } from 'zustand';
import { Conversation, Message } from '@/types';

interface ConversationState {
  queue: Conversation[];
  myConversations: Conversation[];
  activeConversationId: string | null;
  messages: Record<string, Message[]>;

  setQueue: (conversations: Conversation[]) => void;
  setMyConversations: (conversations: Conversation[]) => void;
  setActiveConversation: (id: string | null) => void;
  setMessages: (conversationId: string, messages: Message[]) => void;

  addMessageToConversation: (message: Message) => void;
  updateMessageStatus: (whatsAppMessageId: string, status: string) => void;
  addReaction: (
    messageId: string,
    emoji: string,
    from: string,
    isFromAgent: boolean,
  ) => void;
  removeReaction: (messageId: string, from: string) => void;

  addToQueue: (conversation: Conversation) => void;
  moveToMine: (conversationId: string) => void;
  removeFromQueue: (conversationId: string) => void;

  updateContact: (contactId: string, name: string) => void;
}

export const useConversationStore = create<ConversationState>((set) => ({
  queue: [],
  myConversations: [],
  activeConversationId: null,
  messages: {},

  setQueue: (queue) => set({ queue }),
  setMyConversations: (myConversations) => set({ myConversations }),
  setActiveConversation: (id) => set({ activeConversationId: id }),
  setMessages: (conversationId, messages) =>
    set((state) => ({
      messages: { ...state.messages, [conversationId]: messages },
    })),

  addMessageToConversation: (message) =>
    set((state) => {
      const existing = state.messages[message.conversationId] ?? [];

      if (existing.some((m) => m.id === message.id)) return state;

      const optimisticIndex = existing.findIndex(
        (m) =>
          m.isFromAgent &&
          m.status === 'Pending' &&
          m.content === message.content &&
          m.type === message.type &&
          message.isFromAgent,
      );

      let updated: Message[];
      if (optimisticIndex !== -1) {
        updated = [...existing];
        updated[optimisticIndex] = message;
      } else {
        updated = [...existing, message];
      }

      const updateConv = (list: Conversation[]) =>
        list.map((c) =>
          c.id === message.conversationId
            ? { ...c, lastMessage: message, updatedAt: message.createdAt }
            : c,
        );

      return {
        messages: { ...state.messages, [message.conversationId]: updated },
        myConversations: updateConv(state.myConversations),
        queue: updateConv(state.queue),
      };
    }),

  updateMessageStatus: (whatsAppMessageId, status) =>
    set((state) => {
      const newMessages = { ...state.messages };
      for (const convId in newMessages) {
        newMessages[convId] = newMessages[convId].map((m) =>
          m.whatsAppMessageId === whatsAppMessageId
            ? { ...m, status: status as Message['status'] }
            : m,
        );
      }
      return { messages: newMessages };
    }),

  addReaction: (messageId, emoji, from, isFromAgent) =>
    set((state) => {
      const newMessages = { ...state.messages };
      for (const convId in newMessages) {
        newMessages[convId] = newMessages[convId].map((m) => {
          if (m.id !== messageId) return m;
          const alreadyExists = m.reactions.find(
            (r) => r.emoji === emoji && r.messageId === messageId,
          );
          if (alreadyExists) return m;
          return {
            ...m,
            reactions: [
              ...m.reactions,
              {
                id: crypto.randomUUID(),
                messageId,
                emoji,
                fromPhoneNumber: from,
                isFromAgent,
                createdAt: new Date(),
              },
            ],
          };
        });
      }
      return { messages: newMessages };
    }),

  removeReaction: (messageId, from) =>
    set((state) => {
      const newMessages = { ...state.messages };
      for (const convId in newMessages) {
        newMessages[convId] = newMessages[convId].map((m) =>
          m.id === messageId
            ? {
                ...m,
                reactions: m.reactions.filter(
                  (r) => r.fromPhoneNumber !== from,
                ),
              }
            : m,
        );
      }
      return { messages: newMessages };
    }),

  addToQueue: (conversation) =>
    set((state) => {
      const exists = state.queue.find((c) => c.id === conversation.id);
      if (exists) return state;
      return { queue: [conversation, ...state.queue] };
    }),

  moveToMine: (conversationId) =>
    set((state) => {
      const conv = state.queue.find((c) => c.id === conversationId);
      if (!conv) return state;
      return {
        queue: state.queue.filter((c) => c.id !== conversationId),
        myConversations: [
          { ...conv, isInQueue: false },
          ...state.myConversations,
        ],
      };
    }),

  removeFromQueue: (conversationId) =>
    set((state) => ({
      queue: state.queue.filter((c) => c.id !== conversationId),
    })),

  updateContact: (contactId, name) =>
    set((state) => {
      const updateContactList = (list: Conversation[]) =>
        list.map((c) => {
          if (c.contact.id !== contactId) return c;
          if (c.contact.name === name) return c;
          return { ...c, contact: { ...c.contact, name } };
        });

      return {
        queue: updateContactList(state.queue),
        myConversations: updateContactList(state.myConversations),
      };
    }),
}));
