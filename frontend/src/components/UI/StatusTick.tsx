import { Check, CheckCheck, Clock } from 'lucide-react';

interface Props {
  status: 'Pending' | 'Sent' | 'Delivered' | 'Read' | 'Failed';
}

export default function StatusTick({ status }: Props) {
  switch (status.toLowerCase()) {
    case 'pending':
      return <Clock className='w-3.5 h-3.5 text-wa-muted' />;
    case 'sent':
      return <Check className='w-3.5 h-3.5 text-wa-muted' />;
    case 'delivered':
      return <Check className='w-3.5 h-3.5 text-wa-muted' />;
    case 'read':
      return <CheckCheck className='w-3.5 h-3.5 text-blue-500' />;
    case 'failed':
      return <span className='text-red-500 text-xs'>!</span>;
    default:
      return null;
  }
}
