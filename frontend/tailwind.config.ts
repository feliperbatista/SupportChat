import type { Config } from 'tailwindcss'

const config: Config = {
  content: [
    './src/pages/**/*.{js,ts,jsx,tsx,mdx}',
    './src/components/**/*.{js,ts,jsx,tsx,mdx}',
    './src/app/**/*.{js,ts,jsx,tsx,mdx}',
  ],
  theme: {
    extend: {
      colors: {
        wa: {
          bg:         '#111b21',
          panel:      '#202c33',
          hover:      '#2a3942',
          bubble_in:  '#202c33',
          bubble_out: '#005c4b',
          teal:       '#00a884',
          text:       '#e9edef',
          muted:      '#8696a0',
          border:     '#2a3942',
          input:      '#2a3942',
          search:     '#1c2b33',
          green:      '#25d366',
          red:        '#f15c6d',
        }
      },
      fontFamily: {
        sans: ['Inter', 'system-ui', 'sans-serif'],
      }
    },
  },
  plugins: [],
}

export default config