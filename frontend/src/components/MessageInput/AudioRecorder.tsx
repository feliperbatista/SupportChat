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
  const streamRef = useRef<MediaStream | null>(null);

  useEffect(() => {
    let aborted = false;

    const init = async () => {
      try {
        const stream = await navigator.mediaDevices.getUserMedia({
          audio: true,
        });

        if (aborted) {
          stream.getTracks().forEach((t) => t.stop());
          return;
        }

        streamRef.current = stream;

        const recorder = new MediaRecorder(stream, {
          mimeType: 'audio/webm; codecs=opus',
        });
        mediaRef.current = recorder;
        chunksRef.current = [];
        recorder.ondataavailable = (e) => {
          if (e.data.size > 0) chunksRef.current.push(e.data);
        };
        recorder.start();
      } catch (err) {
        console.error('Mic error:', err);
        onCancel();
      }
    };

    init();
    intervalRef.current = setInterval(() => setSeconds((s) => s + 1), 1000);

    return () => {
      aborted = true;
      if (intervalRef.current) clearInterval(intervalRef.current);

      if (mediaRef.current && mediaRef.current.state !== 'inactive') {
        mediaRef.current.stop();
      }
      stopTracks();
    };
  }, [onCancel]);

  function stopTracks() {
    if (streamRef.current) {
      streamRef.current.getTracks().forEach((t) => t.stop());
      streamRef.current = null;
    }

    navigator.mediaDevices.getUserMedia({ audio: true }).then((s) => {
      s.getTracks().forEach((t) => t.stop());
    });
  }

  function stopRecording(): Promise<Blob | null> {
    return new Promise((resolve) => {
      const recorder = mediaRef.current;

      if (!recorder || recorder.state === 'inactive') {
        stopTracks();
        return resolve(null);
      }

      recorder.onstop = async () => {
        const blob = new Blob(chunksRef.current, {
          type: 'audio/webm; codecs=opus',
        });
        stopTracks();
        resolve(blob);
      };

      recorder.stop();
      if (intervalRef.current) clearInterval(intervalRef.current);
    });
  }

  async function handleSend() {
    const blob = await stopRecording();
    if (!blob) return;
    setSending(true);

    try {
      const formData = new FormData();
      formData.append('file', blob, 'audio.webm');
      formData.append('content', 'Audio');
      formData.append('type', 'Audio');

      await api.post(`/api/conversation/${conversationId}/messages`, formData, {
        headers: { 'Content-Type': 'multipart/form-data' },
      });
      stopTracks();
      onSent();
    } catch (err) {
      console.error('Failed to send audio', err);
    } finally {
      setSending(false);
    }
  }

  function handleCancel() {
    if (mediaRef.current && mediaRef.current.state !== 'inactive') {
      mediaRef.current.stop();
    }
    stopTracks();
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
