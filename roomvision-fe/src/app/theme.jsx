import { createTheme } from "@mui/material/styles";

export const theme = createTheme({
  palette: {
    primary: {
      main: "#005461",
      contrastText: "#ffffff",
    },
    secondary: {
      main: "#018790",
    },
    info: {
      main: "#00B7B5",
    },
    background: {
      default: "#F4F4F4",
      paper: "#ffffff",
    },
    text: {
      primary: "#000000ff",
      secondary: "#ffffffff",
    },
    error: {
      main: "#c1121f",
    },
  },
});
