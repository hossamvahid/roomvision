import { TextField } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Search } from "lucide-react";

export default function SearchBar({ value = "", onChange }) {
  const theme = useTheme();

  return (
    <TextField
      value={value}
      onChange={(e) => onChange && onChange(e.target.value)}
      placeholder="Search"
      variant="outlined"
      size="small"
      sx={{
        width: 250,
        alignSelf: "center",
        "& .MuiOutlinedInput-root": {
          borderRadius: "25px",
          backgroundColor: theme.palette.secondary.main,
          color: "white",
          "& fieldset": { borderColor: theme.palette.secondary.main },
          "&:hover fieldset": { borderColor: theme.palette.secondary.main },
          "&.Mui-focused fieldset": {
            borderColor: theme.palette.secondary.main,
          },
        },
        "& .MuiOutlinedInput-input": {
          color: "white",
          "&::placeholder": {
            color: "rgba(255,255,255,0.7)",
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
  );
}
