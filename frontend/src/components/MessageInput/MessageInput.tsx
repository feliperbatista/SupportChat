'use client';

import { useAuthStore } from '@/store/authStore';
import { useConversationStore } from '@/store/conversationStore';
import { Message } from '@/types';
import { FileIcon, Mic, Paperclip, Send, Smile, X } from 'lucide-react';
import React, { ChangeEvent, useRef, useState } from 'react';
import AudioRecorder from './AudioRecorder';
import { useMessages } from '@/hooks/useMessages';
import {
  FilePreview,
  formatBytes,
  getFileType,
  getMessageType,
} from '@/utils/file';
import Image from 'next/image';

interface Props {
  conversationId: string;
}

export default function MessageInput({ conversationId }: Props) {
  const agent = useAuthStore((s) => s.agent);
  const { sendMessage } = useMessages();
  const addMessage = useConversationStore((s) => s.addMessageToConversation);

  const [text, setText] = useState('');
  const [sending, setSending] = useState(false);
  const [recording, setRecording] = useState(false);
  const [preview, setPreview] = useState<FilePreview | null>(null);
  const [progress, setProgress] = useState(0);
  const textareaRef = useRef<HTMLTextAreaElement>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

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
    resetTextArea();

    try {
      await sendMessage({
        conversationId: conversationId,
        content: optimistic.content,
        type: 'Text',
      });
    } catch (err) {
      console.error('Failed to send message', err);
    } finally {
      setSending(false);
    }
  }

  async function sendFile() {
    if (!preview || sending) return;

    setSending(true);
    setProgress(0);

    const optimistic: Message = {
      id: crypto.randomUUID(),
      conversationId,
      content: text.trim(),
      type: getMessageType(preview.type),
      status: 'Pending',
      isFromAgent: true,
      sentByAgentId: agent?.id,
      sentByAgentName: agent?.name,
      createdAt: new Date().toISOString(),
      reactions: [],
    };

    addMessage(optimistic);

    await sendMessage({
      conversationId,
      content: text.trim() ?? '',
      type: getMessageType(preview.type),
      file: preview.file,
      onProgress: setProgress,
    });

    setPreview(null);
    setText('');
    setProgress(0);
    setSending(false);
  }

  function handleKeyDown(e: React.KeyboardEvent<HTMLTextAreaElement>) {
    if (e.key === 'Enter' && !e.shiftKey) {
      e.preventDefault();
      if (preview) sendFile();
      else sendText();
    }
  }

  function handleFileChange(e: ChangeEvent<HTMLInputElement>) {
    const file = e.target.files?.[0];
    if (!file) return;

    const type = getFileType(file);
    const url =
      type === 'image' || type === 'video' ? URL.createObjectURL(file) : '';

    setPreview({ file, url, type });
    e.target.value = '';
  }

  function clearPreview() {
    if (preview?.url) URL.revokeObjectURL(preview.url);
    setPreview(null);
    setProgress(0);
  }

  function handleChange(e: React.ChangeEvent<HTMLTextAreaElement>) {
    setText(e.target.value);
    autoResize();
  }

  function autoResize() {
    const ta = textareaRef.current;
    if (!ta) return;
    ta.style.height = 'auto';
    ta.style.height = `${Math.min(ta.scrollHeight, 120)}px`;
  }

  function resetTextArea() {
    const ta = textareaRef.current;
    if (!ta) return;
    ta.style.height = 'auto';
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
    <div className='bg-wa-panel border-t border-wa-border'>
      {preview && (
        <div className='px-4 pt-3 pb-2'>
          <div
            className='
            relative flex items-center gap-3
            bg-wa-hover rounded-xl p-3
            border border-wa-border
          '
          >
            <button
              onClick={clearPreview}
              disabled={sending}
              className='
                absolute -top-2 -right-2 w-5 h-5 rounded-full
                bg-wa-muted text-wa-bg flex items-center justify-center
                hover:bg-wa-text transition-colors
              '
            >
              <X className='w-3 h-3' />
            </button>

            {/* Thumbnail or icon */}
            {preview.type === 'image' && preview.url ? (
              <Image
                src={preview.url}
                alt='Preview'
                height={16}
                width={16}
                className='w-16 h-16 rounded-lg object-cover shrink-0'
              />
            ) : preview.type === 'video' && preview.url ? (
              <video
                src={preview.url}
                className='w-16 h-16 rounded-lg object-cover shrink-0'
              />
            ) : (
              <div
                className='
                w-16 h-16 rounded-lg bg-wa-panel flex items-center
                justify-center shrink-0 text-wa-teal
              '
              >
                <FileIcon type={preview.type} />
              </div>
            )}

            <div className='flex-1 min-w-0'>
              <p className='text-wa-text text-sm font-medium truncate'>
                {preview.file.name}
              </p>
              <p className='text-wa-muted text-xs mt-0.5'>
                {formatBytes(preview.file.size)}
              </p>

              {sending && (
                <div className='mt-2'>
                  <div className='h-1 bg-wa-border rounded-full overflow-hidden'>
                    <div
                      className='h-full bg-wa-teal rounded-full transition-all duration-300'
                      style={{ width: `${progress}%` }}
                    />
                  </div>
                  <p className='text-wa-muted text-xs mt-1'>{progress}%</p>
                </div>
              )}
            </div>
          </div>
        </div>
      )}

      <div className='flex items-end gap-2 px-3 py-2'>
        <button
          className='text-wa-muted hover:text-wa-text transition-colors p-2 shrink-0'
          title='Emoji'
        >
          <Smile className='w-5 h-5' />
        </button>

        <div className='relative shrink-0'>
          <input
            ref={fileInputRef}
            type='file'
            accept='image/*,video/*,audio/*,.pdf,.doc,.docx,.xls,.xlsx,.zip,.txt'
            onChange={handleFileChange}
            className='hidden'
          />
          <button
            onClick={() => fileInputRef.current?.click()}
            disabled={sending}
            className='
              text-wa-muted hover:text-wa-text transition-colors p-2
              disabled:opacity-50
            '
            title='Attach file'
          >
            <Paperclip className='w-5 h-5' />
          </button>
        </div>

        <textarea
          ref={textareaRef}
          value={text}
          onChange={handleChange}
          onKeyDown={handleKeyDown}
          placeholder={preview ? 'Add a caption...' : 'Type a message'}
          rows={1}
          disabled={sending}
          className='
            flex-1 bg-wa-input text-wa-text text-sm rounded-lg
            px-4 py-2.5 outline-none resize-none
            placeholder:text-wa-muted min-h-10.5 max-h-30
            leading-relaxed disabled:opacity-50
          '
        />

        {text.trim() || preview ? (
          <button
            onClick={preview ? sendFile : sendText}
            disabled={sending}
            className='
              w-10 h-10 rounded-full bg-wa-teal flex items-center
              justify-center shrink-0 hover:bg-teal-500
              transition-colors disabled:opacity-50 disabled:cursor-not-allowed
            '
            title='Send'
          >
            {sending ? (
              <div
                className='w-4 h-4 border-2 border-white border-t-transparent
                              rounded-full animate-spin'
              />
            ) : (
              <Send className='w-4 h-4 text-white' />
            )}
          </button>
        ) : (
          <button
            onClick={() => setRecording(true)}
            disabled={sending}
            className='
              w-10 h-10 rounded-full bg-wa-teal flex items-center
              justify-center shrink-0 hover:bg-teal-500
              transition-colors disabled:opacity-50
            '
            title='Record audio'
          >
            <Mic className='w-4 h-4 text-white' />
          </button>
        )}
      </div>
    </div>
  );
}
