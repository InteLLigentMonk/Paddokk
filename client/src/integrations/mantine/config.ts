import { createTheme, Image } from "@mantine/core";
import type { ImageProps, MantineColorsTuple } from "@mantine/core";

export const myColor: MantineColorsTuple = [
  "#fffae0",
  "#fff3ca",
  "#ffe699",
  "#ffd863",
  "#ffcd36",
  "#ffc517",
  "#ffc102",
  "#e3aa00",
  "#ca9700",
  "#af8200",
];

export const theme = createTheme({
  colors: {
    myColor,
  },
  primaryColor: "myColor",
  primaryShade: { light: 6, dark: 7 },
  fontSizes: {
    d3: "2.75rem",
    d2: "3.5rem",
    d1: "4.75rem",
  },
  fontFamily: "Rajdhani, sans-serif",
  headings: {
    fontFamily: "BaseNeue, sans-serif",
    sizes: {
      h1: { fontSize: "1.5rem" },
    },
  },
  components: {
    Image: Image.extend({
      defaultProps: {
        loading: "lazy",
        decoding: "async",
      } as unknown as Partial<ImageProps>,
    }),
  },
});
