/** @type {import('tailwindcss').Config} */
module.exports = {
  content: ["./src/**/*.{html,ts}"],
  theme: {
    extend: {
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
