'use client';

import { useState, useRef } from 'react';
import { Play, Pause } from 'lucide-react';
import { Message } from '@/types';

interface Props {
  message: Message;
}

export default function AudioMessage({ message }: Props) {
  const audioRef = useRef<HTMLAudioElement>(null);
  const [playing, setPlaying] = useState(false);
  const [progress, setProgress] = useState(0);
  const [duration, setDuration] = useState(0);

  function togglePlay() {
    const audio = audioRef.current;
    if (!audio) return;
    if (playing) {
      audio.pause();
    } else {
      audio.play();
    }
    setPlaying(!playing);
  }

  function handleTimeUpdate() {
    const audio = audioRef.current;
    if (!audio) return;
    setProgress((audio.currentTime / audio.duration) * 100);
  }

  function handleLoadedMetadata() {
    const audio = audioRef.current;
    if (!audio) return;

    if (!Number.isFinite(audio.duration)) {
      audio.currentTime = 1e101;

      audio.ontimeupdate = function () {
        audio.ontimeupdate = null;
        audio.currentTime = 0;
        setDuration(audio.duration);
      };
    } else {
      setDuration(audio.duration);
    }
  }

  function handleEnded() {
    setPlaying(false);
    setProgress(0);
  }

  function handleSeek(e: React.ChangeEvent<HTMLInputElement>) {
    const audio = audioRef.current;
    if (!audio) return;
    const newTime = (Number(e.target.value) / 100) * audio.duration;
    audio.currentTime = newTime;
    setProgress(Number(e.target.value));
  }

  function formatDuration(s: number) {
    if (!Number.isFinite(s) || s <= 0) return '0:00';

    const m = Math.floor(s / 60);
    const sec = Math.floor(s % 60);

    return `${m}:${sec.toString().padStart(2, '0')}`;
  }

  return (
    <div className='flex items-center gap-2 w-50'>
      {message.mediaUrl && (
        <audio
          key={message.mediaUrl}
          ref={audioRef}
          src={message.mediaUrl}
          onTimeUpdate={handleTimeUpdate}
          onLoadedMetadata={handleLoadedMetadata}
          onEnded={handleEnded}
        />
      )}

      <button
        onClick={togglePlay}
        className='
          w-9 h-9 rounded-full bg-wa-teal flex items-center
          justify-center shrink-0 hover:bg-teal-500 transition-colors
        '
      >
        {playing ? (
          <Pause className='w-4 h-4 text-white' />
        ) : (
          <Play className='w-4 h-4 text-white ml-0.5' />
        )}
      </button>

      <div className='flex-1 flex flex-col gap-1'>
        <input
          type='range'
          min={0}
          max={100}
          value={progress}
          onChange={handleSeek}
          className='w-full h-1 accent-wa-teal cursor-pointer'
        />
        <span className='text-[11px] text-wa-muted'>
          {formatDuration(duration)}
        </span>
      </div>
    </div>
  );
}
