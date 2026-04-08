'use client';

import api from '@/services/api';
import { Send, Trash2 } from 'lucide-react';
import { useEffect, useRef, useState } from 'react';

interface Props {
  conversationId: string;
  onCancel: () => void;
  onSent: () => void;
}

export default function AudioRecorder({
  conversationId,
  onCancel,
  onSent,
}: Props) {
  const [seconds, setSeconds] = useState(0);
  const [sending, setSending] = useState(false);
  const mediaRef = useRef<MediaRecorder | null>(null);
  const chunksRef = useRef<Blob[]>([]);
  const intervalRef = useRef<NodeJS.Timeout | null>(null);

  useEffect(() => {
    startRecording();
    intervalRef.current = setInterval(() => setSeconds((s) => s + 1), 1000);
    return () => {
      stopRecording();
      if (intervalRef.current) clearInterval(intervalRef.current);
    };
  });

  async function startRecording() {
    try {
      const stream = await navigator.mediaDevices.getUserMedia({ audio: true });
      const recorder = new MediaRecorder(stream);
      mediaRef.current = recorder;
      chunksRef.current = [];
      recorder.ondataavailable = (e) => chunksRef.current.push(e.data);
      recorder.start();
    } catch (err) {
      console.error('Microphone access denied', err);
      onCancel();
    }
  }

  function stopRecording(): Blob | null {
    const recorder = mediaRef.current;
    if (!recorder) return null;
    recorder.stop();
    recorder.stream.getTracks().forEach((t) => t.stop());
    if (intervalRef.current) clearInterval(intervalRef.current);
    return new Blob(chunksRef.current, { type: 'audio/webm' });
  }

  async function handleSend() {
    const blob = stopRecording();
    if (!blob) return;
    setSending(true);

    try {
      const formData = new FormData();
      formData.append('file', blob, 'audio.webm');
      formData.append('conversationId', conversationId);

      await api.post(
        `/api/conversation/${conversationId}/messages/audio`,
        formData,
        { headers: { 'Content-Type': 'multipart/form-data' } },
      );
      onSent();
    } catch (err) {
      console.error('Failed to send audio', err);
    } finally {
      setSending(false);
    }
  }

  function handleCancel() {
    stopRecording();
    onCancel();
  }

  function formatTime(s: number) {
    const m = Math.floor(s / 60);
    const sec = s % 60;
    return `${m}:${sec.toString().padStart(2, '0')}`;
  }

  return (
    <div className='flex items-center gap-3 px-4 py-3 bg-wa-panel border-t border-wa-border'>
      <button
        onClick={handleCancel}
        className='text-wa-muted hover:text-red-400 transition-colors p-2'
      >
        <Trash2 className='w-5 h-5' />
      </button>

      <div className='flex-1 flex items-center gap-3 bg-wa-input rounded-full px-4 py-2'>
        <span className='w-2 h-2 rounded-full bg-red-500 animate-pulse' />
        <span className='text-wa-muted text-sm'>{formatTime(seconds)}</span>
        <div className='flex-1 flex items-center gap-0.5'>
          {Array.from({ length: 30 }).map((_, i) => (
            <div
              key={i}
              className='flex-1 bg-wa-teal rounded-full opacity-60'
              style={{
                height: `${Math.random() * 16 + 4}px`,
                animationDelay: `${i * 50}ms`,
              }}
            ></div>
          ))}
        </div>
      </div>

      <button
        onClick={handleSend}
        disabled={sending}
        className='w-10 h-10 rounded-full bg-wa-teal flex items-center justify-center hover:bg-teal-500 transition-colors disabled:opacity-50'
      >
        <Send className='w-4 h-4 text-white' />
      </button>
    </div>
  );
}
