import { Box, Typography, TextField } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Search } from "lucide-react";

export default function Title() {
  const theme = useTheme();

  return (
    <>
      <Box
        sx={{
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          mb: 4,
        }}
      >
        <Typography
          variant="h3"
          sx={{
            fontWeight: 600,
            color: theme.palette.secondary.main,
            borderBottom: `3px solid ${theme.palette.secondary.main}`,
            pb: 1,
            display: "inline-block",
          }}
        >
          Rooms
        </Typography>
      </Box>
    </>
  );
}
