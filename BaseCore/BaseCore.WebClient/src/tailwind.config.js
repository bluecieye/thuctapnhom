import forms from '@tailwindcss/forms';

/** @type {import('tailwindcss').Config} */
export default {
  content: ['./index.html', './src/**/*.{js,jsx,ts,tsx}'],
  theme: {
    extend: {
      fontFamily: {
        sans: ['Lora', 'Georgia', 'Cambria', 'Times New Roman', 'serif'],
        serif: ['"Playfair Display"', 'Georgia', 'Cambria', 'serif'],
        display: ['"Playfair Display"', 'Georgia', 'Cambria', 'serif'],
      },
      boxShadow: {
        soft: '0 10px 30px rgba(15, 23, 42, 0.08)',
      },
    },
  },
  plugins: [forms],
};
