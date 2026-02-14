/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
      colors: {
        "bright-blue": "#2d82b7",
        "dark-blue": "#07004d",
        "light-green": "#42e2b8",
        "soft-red": "#eb8a90",
        "light-gray": "#f2f2f7",
        "light-beige": "#f3dfbf",
      },
      keyframes: {
        stepPulse: {
          "0%, 100%": { transform: "scale(1)" },
          "50%": { transform: "scale(1.1)" },
        },
      },
      animation: {
        stepPulse: "stepPulse 1.2s ease-in-out infinite",
      },
      fontFamily: {
        header: ["Poppins", "sans-serif"],
        content: ["Inter", "sans-serif"],
      },
    },
  },
  plugins: [],
};
