import { Contact } from '@/types';
import Input from '../UI/Input';
import React, { useState } from 'react';
import { useConversations } from '@/hooks/useConversations';
import { Loader2 } from 'lucide-react';

interface Props {
  contact: Contact;
  onClose: () => void;
}

export default function EditContatctForm({ contact, onClose }: Props) {
  const [name, setName] = useState(contact.name);
  const [loading, setLoading] = useState(false);
  const { updateContactName } = useConversations();

  const handleSubmit = async (e: React.SubmitEvent) => {
    e.preventDefault();
    setLoading(true);
    await updateContactName(contact.id, name);
    setLoading(false);
    onClose();
  };

  return (
    <div className='w-3/4 flex items-center my-3'>
      <form onSubmit={handleSubmit} className='flex gap-3 w-full flex-col'>
        <Input
          type='text'
          value={name}
          label='Name'
          placeholder={contact.name}
          required={true}
          onChange={(e) => setName(e.target.value)}
        />
        <button
          type='submit'
          disabled={loading}
          className='bg-wa-teal hover:bg-teal-500 disabled:opacity-50
                         disabled:cursor-not-allowed text-white font-medium
                         rounded-lg py-3 transition-colors flex items-center
                         justify-center gap-2'
        >
          {loading && <Loader2 className='w-4 h-4 animate-spin' />}
          {loading ? 'Updating...' : 'Update'}
        </button>
      </form>
    </div>
  );
}
