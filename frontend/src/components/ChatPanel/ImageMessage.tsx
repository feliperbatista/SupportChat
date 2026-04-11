'use client';

import { Message } from '@/types';
import { X } from 'lucide-react';
import Image from 'next/image';
import { useState } from 'react';

interface Props {
  message: Message;
  isSticker: boolean
}

export default function ImageMessage({ message, isSticker }: Props) {
  const [lightbox, setLightbox] = useState(false);

  if (!message.mediaUrl) {
    return (
      <div className='w-48 h-32 bg-wa-hover rounded-lg flex items-center justify-center text-wa-muted text-sm'>
        🖼️ Image
      </div>
    );
  }

  return (
    <>
      <div className='relative max-w-70 w-full'>
        <Image
          src={message.mediaUrl}
          alt='Image message'
          onClick={() => setLightbox(true)}
          width={isSticker ? 150 : 300}
          height={isSticker ? 150 : 300}
          className='max-w-70 max-h-80 rounded-lg object-cover cursor-pointer hover:opacity-90 transition-opacity'
        />
        {message.content && (
          <p className='text-wa-text text-sm mt-1'>{message.content}</p>
        )}
      </div>

      {lightbox && (
        <div
          className='fixed inset-0 bg-black/90 z-50 flex items-center justify-center p-4'
          onClick={() => setLightbox(false)}
        >
          <button
            className='absolute top-4 right-4 text-white hover:text-wa-muted'
            onClick={() => setLightbox(false)}
          >
            <X className='w-6 h-6' />
          </button>
          <Image
            src={message.mediaUrl}
            alt='Full size'
            width={800}
            height={800}
            className='max-w-full max-h-full object-contain rounded-lg'
            onClick={(e) => e.stopPropagation()}
          />
        </div>
      )}
    </>
  );
}
