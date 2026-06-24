import { defineConfig } from "vite";
import path from "path";

export default defineConfig({
  resolve: {
    alias: {
      "@dt": path.resolve(__dirname, "./src"),
    },
  },
});
