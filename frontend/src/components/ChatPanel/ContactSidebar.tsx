import { Conversation } from '@/types';
import { Edit, X } from 'lucide-react';
import Avatar from '../UI/Avatar';
import { useState } from 'react';
import EditContatctForm from './EditContatctForm';

interface Props {
  conversation: Conversation;
  onClose: () => void;
}

export default function ContactSidebar({ conversation, onClose }: Props) {
  const { contact } = conversation;
  const [showEditForm, setShowEditForm] = useState(false);

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

      <div className='flex flex-col items-center'>
        <div className='flex flex-col items-center w-full gap-3 py-8 px-6 border-b border-wa-border'>
          <Avatar
            name={contact.name}
            imageUrl={contact.profilePictureUrl}
            size='xl'
          />
          <div className='flex flex-col items-center text-center'>
            <h2 className='text-wa-text font-semibold text-lg'>
              {contact.name}
            </h2>
            <p className='text-wa-muted text-sm mt-0.5 mb-1.5'>
              {contact.phoneNumber}
            </p>
            <button
              onClick={() => setShowEditForm(!showEditForm)}
              className='flex items-center text-wa-text hover:cursor-pointer px-1.5 py-0.5 bg-wa-teal hover:opacity-80 rounded-full'
            >
              <Edit className='h-4 w-4 mr-1' />
              <span className='text-xs'>Edit</span>
            </button>
          </div>
        </div>
        {showEditForm && <EditContatctForm contact={contact} onClose={() => setShowEditForm(false)} />}
      </div>
    </div>
  );
}
