import Image from 'next/image';

interface Props {
  name: string;
  imageUrl?: string;
  size?: 'sm' | 'md' | 'lg';
  className?: string;
}

const COLORS = [
  'bg-purple-600',
  'bg-pink-600',
  'bg-orange-500',
  'bg-teal-600',
  'bg-blue-600',
  'bg-indigo-600',
  'bg-rose-600',
];

function getColor(name: string) {
  const index = name.charCodeAt(0) % COLORS.length;
  return COLORS[index];
}

const sizes = {
  sm: 'w-8 h-8 text-xs',
  md: 'w-10 h-10 text-sm',
  lg: 'w-12 h-12 text-base',
};

export default function Avatar({
  name,
  imageUrl,
  size = 'md',
  className = '',
}: Props) {
  const initals = name
    .split(' ')
    .map((n) => n[0])
    .slice(0, 2)
    .join('')
    .toUpperCase();

  if (imageUrl) {
    return (
      <Image
        src={imageUrl}
        alt={name}
        className={`${sizes[size]} rounded-full object-cover shrink-0 ${className}`}
      />
    );
  }

  return (
    <div
      className={`${sizes[size]} ${getColor(name)} rounded-full flex items-center justify-center shrink-0 font-medium text-white ${className}`}
    >
      {initals}
    </div>
  );
}
