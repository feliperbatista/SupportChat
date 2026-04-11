import { Conversation } from '@/types';
import { X } from 'lucide-react';
import Avatar from '../UI/Avatar';

interface Props {
  conversation: Conversation;
  onClose: () => void;
}

export default function ContactSidebar({ conversation, onClose }: Props) {
  const { contact } = conversation;

  return (
    <div className='w-85 min-w-85 h-full flex flex-col bg-wa-panel border border-wa-border animate-in slide-in-from-right duration-200'>
      <div className='flex items-center gap-3 px-4 h-15 border-b border-wa-border shrink-0'>
        <button
          onClick={onClose}
          className='text-wa-muted hover:text-wa-text transition-colors'
        >
          <X className='w-5 h-5' />
        </button>
        <span className='text-wa-text font-medium text-sm'>Contact Info</span>
      </div>

      <div className='flex-1 overflow-y-auto'>
        <div className='flex flex-col items-center gap-3 py-8 px-6 border-b border-wa-border'>
          <Avatar
            name={contact.name}
            imageUrl={contact.profilePictureUrl}
            size='xl'
          />
          <div className='text-center'>
            <h2 className='text-wa-text font-semibold text-lg'>
              {contact.name}
            </h2>
            <p className='text-wa-muted text-sm mt-0.5'>
              {contact.phoneNumber}
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
