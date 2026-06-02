import { Box } from "@mui/material";
import { formatDate } from "../../utils/date";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT = "oklch(0.840 0.060 214)";

function Stat({ label, value }) {
  return (
    <Box sx={{ display: "flex", flexDirection: "column", gap: "6px" }}>
      <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)" }}>{label}</Box>
      <Box sx={{ fontFamily: displayFont, fontSize: "30px", fontWeight: 600, letterSpacing: "-0.02em", lineHeight: 1, color: "oklch(0.960 0.004 248)", fontVariantNumeric: "tabular-nums" }}>{value}</Box>
    </Box>
  );
}

export default function Information({ totalFaces, totalUnknown, scannedAt }) {
  return (
    <Box sx={{ display: "flex", alignItems: "flex-start", gap: "40px", flexWrap: "wrap", mt: "20px" }}>
      <Stat label="Total people" value={totalFaces} />
      <Stat label="Unknown" value={totalUnknown} />
      <Box sx={{ display: "flex", flexDirection: "column", gap: "6px" }}>
        <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)" }}>Last scanned</Box>
        <Box sx={{ fontFamily: monoFont, fontSize: "14px", color: "oklch(0.700 0.008 248)", letterSpacing: "0.02em", mt: "6px" }}>{formatDate(scannedAt)}</Box>
      </Box>
    </Box>
  );
}
