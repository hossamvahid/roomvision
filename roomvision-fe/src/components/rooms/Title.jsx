import { Box } from "@mui/material";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';

export default function Title() {
  return (
    <Box sx={{ display: "flex", alignItems: "flex-end", justifyContent: "space-between", gap: "24px", mb: "26px", flexWrap: "wrap" }}>
      <Box>
        <Box sx={{ fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.2em", textTransform: "uppercase", color: "oklch(0.840 0.060 214)", mb: "9px" }}>
          Live monitoring
        </Box>
        <Box component="h1" sx={{ margin: 0, fontFamily: displayFont, fontSize: "27px", fontWeight: 600, letterSpacing: "-0.02em", color: "oklch(0.960 0.004 248)" }}>
          Rooms
        </Box>
        <Box sx={{ fontFamily: displayFont, fontSize: "14px", color: "oklch(0.700 0.008 248)", mt: "7px" }}>
          Real-time occupancy across all monitored spaces.
        </Box>
      </Box>
    </Box>
  );
}
