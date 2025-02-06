import { defineConfig } from "vite";

const libName = "discord-proxy-patch";
export default defineConfig({
    build: {
        lib: {
            name: libName,
            entry: "main.ts",
            fileName(format) {
                return `${libName}.${format}.jspre`
            },
        },
        minify: false,
        rollupOptions: {
            treeshake: true  // Ensure tree shaking is enabled
        }
    }
})
