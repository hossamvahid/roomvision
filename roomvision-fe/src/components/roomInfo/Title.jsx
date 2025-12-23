import { Box, Typography } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import Information from "./Information";
import SearchBar from "./SearchBar";

export default function Title({
  roomName,
  totalFaces,
  totalUnknown,
  scannedAt,
  searchTerm = "",
  setSearchTerm,
}) {
  const theme = useTheme();

  return (
    <Box
      sx={{
        display: "flex",
        flexWrap: "wrap",
        justifyContent: "space-between",
        alignItems: "flex-start",
        gap: 2,
        borderBottom: `3px solid ${theme.palette.secondary.main}`,
        pb: 2,
        mb: 4,
      }}
    >
      <Box sx={{ display: "flex", flexDirection: "column", gap: 2 }}>
        <Typography
          variant="h3"
          sx={{
            fontWeight: 600,
            color: theme.palette.secondary.main,
          }}
        >
          {roomName}
        </Typography>

        <Information
          totalFaces={totalFaces}
          totalUnknown={totalUnknown}
          scannedAt={scannedAt}
        />
      </Box>

      <SearchBar value={searchTerm} onChange={setSearchTerm} />
    </Box>
  );
}
