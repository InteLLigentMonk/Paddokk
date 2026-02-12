import { createTheme } from "@mantine/core";
import type { MantineColorsTuple } from "@mantine/core";

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
});
