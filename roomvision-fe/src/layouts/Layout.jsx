import { Box } from "@mui/material";
import { Navbar } from "../components/navbar";

export default function Layout({ children }) {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", minHeight: "100vh", bgcolor: "oklch(0.135 0.006 248)" }}>
      <Navbar />
      <Box sx={{ flex: 1 }}>
        {children}
      </Box>
    </Box>
  );
}
