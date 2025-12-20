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

        <TextField
          placeholder="search"
          variant="outlined"
          size="small"
          sx={{
            width: 250,
            "& .MuiOutlinedInput-root": {
              borderRadius: "25px",
              backgroundColor: theme.palette.secondary.main,
              color: "white",
              "& fieldset": {
                borderColor: theme.palette.secondary.main,
              },
              "&:hover fieldset": {
                borderColor: theme.palette.secondary.main,
              },
              "&.Mui-focused fieldset": {
                borderColor: theme.palette.secondary.main,
              },
            },
            "& .MuiOutlinedInput-input": {
              color: "white",
              "&::placeholder": {
                color: "rgba(255, 255, 255, 0.7)",
                opacity: 1,
              },
            },
          }}
          InputProps={{
            endAdornment: (
              <Search size={20} style={{ color: "white", marginRight: 8 }} />
            ),
          }}
        />
      </Box>
    </>
  );
}
