import { Box } from "@mui/material";
import { useNavigate } from "react-router-dom";
import { useState } from "react";
import Information from "./Information";
import downloadScanReport from "../../services/room/downloadScanReport";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT = "oklch(0.840 0.060 214)";

export default function Title({ roomName, totalFaces, totalUnknown, scannedAt }) {
  const navigate = useNavigate();
  const [downloading, setDownloading] = useState(false);

  const handleDownload = async () => {
    setDownloading(true);
    try {
      await downloadScanReport(roomName);
    } catch {
      // silent fail
    } finally {
      setDownloading(false);
    }
  };

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

      {/* Room name + Download button */}
      <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", flexWrap: "wrap", gap: "12px" }}>
        <Box component="h1" sx={{ margin: 0, fontFamily: displayFont, fontSize: "27px", fontWeight: 600, letterSpacing: "-0.02em", color: "oklch(0.960 0.004 248)" }}>
          {roomName}
        </Box>
        <Box
          component="button"
          onClick={handleDownload}
          disabled={downloading}
          sx={{
            display: "inline-flex", alignItems: "center", gap: "7px",
            background: "none", border: "1px solid oklch(0.305 0.010 248)",
            borderRadius: "6px", cursor: downloading ? "not-allowed" : "pointer",
            fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.1em",
            textTransform: "uppercase", color: downloading ? "oklch(0.430 0.009 248)" : ACCENT,
            px: "12px", py: "7px", opacity: downloading ? 0.6 : 1,
            "&:hover": { borderColor: ACCENT, background: "oklch(0.160 0.007 248)" },
            transition: "all 0.15s ease",
          }}
        >
          <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
            <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
            <polyline points="7 10 12 15 17 10"/>
            <line x1="12" y1="15" x2="12" y2="3"/>
          </svg>
          {downloading ? "Se descarcă..." : "Descarcă PDF"}
        </Box>
      </Box>

      <Information totalFaces={totalFaces} totalUnknown={totalUnknown} scannedAt={scannedAt} />

      <Box sx={{ height: "1px", background: "oklch(0.305 0.010 248)", mt: "28px" }} />
    </Box>
  );
}
