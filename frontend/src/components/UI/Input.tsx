interface Props {
  type: string;
  value: string;
  onChange: (e: React.ChangeEvent<HTMLInputElement>) => void
  placeholder: string
  required: boolean
}

export default function Input({type, value, onChange, placeholder, required}: Props) {
  return (
    <div className='flex flex-col gap-1.5'>
      <label className='text-sm text-wa-muted'>Email</label>
      <input
        type={type}
        value={value}
        onChange={onChange}
        placeholder={placeholder}
        required={required}
        className='bg-wa-input text-wa-text rounded-lg px-4 py-3 text-sm outline-none border border-transparent focus:border-wa-teal transition-colors placeholder:text-wa-muted'
      />
    </div>
  );
}
