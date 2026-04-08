interface Props {
  count: number;
}

export default function Badge({ count }: Props) {
  if (count === 0) return null;
  return (
    <span className='bg-wa-teal text-white text-xs font-medium rounded-full min-w-4.5 h-4.5 px-1 flex items-center justify-center shrink-0'>
      {count > 99 ? '99+' : count}
    </span>
  );
}
