import { Box } from "@mui/material";
import { useNavigate } from "react-router-dom";
import Information from "./Information";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT = "oklch(0.840 0.060 214)";
const ACCENT_DIM = "oklch(0.840 0.060 214 / 0.16)";

export default function Title({ roomName, totalFaces, totalUnknown, scannedAt }) {
  const navigate = useNavigate();

  return (
    <Box sx={{ mb: "32px" }}>
      {/* Back link */}
      <Box component="button" onClick={() => navigate("/rooms")}
        sx={{ display: "inline-flex", alignItems: "center", gap: "7px", background: "none", border: "none", cursor: "pointer", fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.1em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", mb: "20px", padding: 0, "&:hover": { color: ACCENT } }}>
        <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M19 12H5"/><path d="M12 19l-7-7 7-7"/></svg>
        Rooms
      </Box>

      {/* Eyebrow */}
      <Box sx={{ fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.2em", textTransform: "uppercase", color: ACCENT, mb: "9px" }}>
        Room detail
      </Box>

      {/* Room name */}
      <Box component="h1" sx={{ margin: 0, fontFamily: displayFont, fontSize: "27px", fontWeight: 600, letterSpacing: "-0.02em", color: "oklch(0.960 0.004 248)" }}>
        {roomName}
      </Box>

      <Information totalFaces={totalFaces} totalUnknown={totalUnknown} scannedAt={scannedAt} />

      <Box sx={{ height: "1px", background: "oklch(0.305 0.010 248)", mt: "28px" }} />
    </Box>
  );
}
