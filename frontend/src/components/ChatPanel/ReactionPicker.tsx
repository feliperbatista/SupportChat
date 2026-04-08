'use client';

const EMOJIS = ['👍', '❤️', '😂', '😮', '😢', '🙏', '🔥', '👏'];

interface Props {
  onReact: (emoji: string) => void;
}

export default function ReactionPicker({ onReact }: Props) {
  return (
    <div className='flex gap-1 bg-wa-panel border border-wa-border rounded-full px-3 py-2 shadow-xl animate-in fade-in zoom-in-95 duration-100'>
      {EMOJIS.map((emoji) => (
        <button
          key={emoji}
          onClick={(e) => {
            e.stopPropagation();
            onReact(emoji);
          }}
          className='text-xl hover:scale-125 transition-transform w-8 h-8 flex items-center justify-center rounded-full hover:bg-wa-hover'
        >
          {emoji}
        </button>
      ))}
    </div>
  );
}
