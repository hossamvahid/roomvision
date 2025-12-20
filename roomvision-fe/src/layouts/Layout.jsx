import React from "react";
import { Box } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Navbar } from "../components/navbar";

export default function Layout({ children }) {
  const theme = useTheme();

  return (
    <Box sx={{ display: "flex", minHeight: "100vh" }}>

      <Navbar />

      <Box
        sx={{
          marginLeft: "250px",
          width: "calc(100% - 250px)",
          bgcolor: theme.palette.background.default,
          overflowY: "auto",
        }}
      >
        {children}
      </Box>
    </Box>
  );
}
