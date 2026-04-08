'use client';

import api from '@/services/api';
import { useAuthStore } from '@/store/authStore';
import { useConversationStore } from '@/store/conversationStore';
import { Message } from '@/types';
import { Mic, Paperclip, Send, Smile } from 'lucide-react';
import React, { useRef, useState } from 'react';
import AudioRecorder from './AudioRecorder';

interface Props {
  conversationId: string;
}

export default function MessageInput({ conversationId }: Props) {
  const agent = useAuthStore((s) => s.agent);
  const addMessage = useConversationStore((s) => s.addMessageToConversation);

  const [text, setText] = useState('');
  const [sending, setSending] = useState(false);
  const [recording, setRecording] = useState(false);
  const textareaRef = useRef<HTMLTextAreaElement>(null);

  async function sendText() {
    if (!text.trim() || sending) return;
    setSending(true);

    const optimistic: Message = {
      id: crypto.randomUUID(),
      conversationId,
      content: text.trim(),
      type: 'Text',
      status: 'Pending',
      isFromAgent: true,
      sentByAgentId: agent?.id,
      sentByAgentName: agent?.name,
      createdAt: new Date().toISOString(),
      reactions: [],
    };

    addMessage(optimistic);
    setText('');

    try {
      const formData = new FormData();
      formData.append('content', optimistic.content);
      formData.append('type', 'Text');

      await api.post(`/api/conversation/${conversationId}/messages`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
    } catch (err) {
      console.error('Failed to send message', err);
    } finally {
      setSending(false);
    }
  }

  function handleKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      sendText();
    }
  }

  function handleChange(e: React.ChangeEvent<HTMLTextAreaElement>) {
    setText(e.target.value);
    const ta = textareaRef.current;
    if (ta) {
      ta.style.height = 'auto';
      ta.style.height = `${Math.min(ta.scrollHeight, 120)}px`;
    }
  }

  if (recording) {
    return (
      <AudioRecorder
        conversationId={conversationId}
        onCancel={() => setRecording(false)}
        onSent={() => setRecording(false)}
      />
    );
  }

  return (
    <div className='flex items-end gap-2 px-3 py-2 bg-wa-panel border-t border-wa-border'>
      <button className='text-wa-muted hover:text-wa-text transition-colors p-2'>
        <Smile className='w-5 h-5' />
      </button>

      <button className='text-wa-muted hover:text-wa-text transition-colors p-2'>
        <Paperclip className='w-5 h-5' />
      </button>

      <textarea
        ref={textareaRef}
        value={text}
        onChange={handleChange}
        onKeyDown={handleKeyDown}
        placeholder='Type a message'
        rows={1}
        className='flex-1 bg-wa-input text-wa-text text-sm rounded-lg px-4 py-2.5 outline-none resize-none placeholder:text-wa-muted min-h-10.5 max-h-30 leading-relaxed'
      />

      {text.trim() ? (
        <button
          onClick={sendText}
          disabled={sending}
          className='w-10 h-10 rounded-full bg-wa-teal flex items-center justify-center shrink-0 hover:bg-teal-500 transition-colors disabled:opacity-50'
        >
          <Send className='w-4 h-4 text-white' />
        </button>
      ) : (
        <button
          onClick={() => setRecording(true)}
          className='w-10 h-10 rounded-full bg-wa-teal flex items-center justify-center shrink-0 hover:bg-teal-500 transition-colors'
        >
          <Mic className='w-4 h-4 text-white' />
        </button>
      )}
    </div>
  );
}
