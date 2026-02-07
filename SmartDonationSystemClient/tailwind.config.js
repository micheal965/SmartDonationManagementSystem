/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      fontFamily: {
        header: ["Poppins", "sans-serif"],
        content: ["Inter", "sans-serif"],
      },
    },
  },
  plugins: [],
};
