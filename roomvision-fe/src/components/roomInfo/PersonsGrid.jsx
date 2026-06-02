import { Box, CircularProgress } from "@mui/material";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT = "oklch(0.840 0.060 214)";

function PersonCard({ name }) {
  const isUnknown = name === "Unknown";
  return (
    <Box sx={{
      display: "flex",
      alignItems: "center",
      gap: "14px",
      background: isUnknown ? "oklch(0.680 0.150 28 / 0.10)" : "oklch(0.195 0.007 248)",
      border: `1px solid ${isUnknown ? "oklch(0.680 0.150 28 / 0.35)" : "oklch(0.305 0.010 248)"}`,
      borderRadius: "10px",
      padding: "14px 16px",
    }}>
      {/* Avatar */}
      <Box sx={{
        width: "36px", height: "36px", borderRadius: "50%", flexShrink: 0,
        background: isUnknown ? "oklch(0.680 0.150 28 / 0.20)" : "oklch(0.225 0.008 248)",
        border: `1px solid ${isUnknown ? "oklch(0.680 0.150 28 / 0.4)" : "oklch(0.395 0.012 248)"}`,
        display: "grid", placeItems: "center",
        fontFamily: monoFont, fontSize: "12px", fontWeight: 600,
        color: isUnknown ? "oklch(0.680 0.150 28)" : "oklch(0.700 0.008 248)",
      }}>
        {isUnknown ? "?" : name.slice(0, 2).toUpperCase()}
      </Box>
      <Box sx={{ fontFamily: displayFont, fontSize: "14px", fontWeight: 500, color: isUnknown ? "oklch(0.680 0.150 28)" : "oklch(0.960 0.004 248)" }}>
        {name}
      </Box>
      {isUnknown && (
        <Box sx={{ marginLeft: "auto", fontFamily: monoFont, fontSize: "9.5px", letterSpacing: "0.08em", textTransform: "uppercase", padding: "3px 8px", borderRadius: "100px", border: "1px solid oklch(0.680 0.150 28 / 0.4)", color: "oklch(0.680 0.150 28)" }}>
          Unknown
        </Box>
      )}
    </Box>
  );
}

export default function PersonsGrid({ faces, loading }) {
  if (loading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: "64px" }}>
        <CircularProgress sx={{ color: ACCENT }} />
      </Box>
    );
  }

  const known   = faces.filter((f) => f !== "Unknown");
  const unknown = faces.filter((f) => f === "Unknown");

  return (
    <Box>
      {known.length > 0 && (
        <Box sx={{ mb: "32px" }}>
          <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", mb: "14px" }}>
            Identified — {known.length}
          </Box>
          <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(260px, 1fr))", gap: "10px" }}>
            {known.map((name, i) => <PersonCard key={i} name={name} />)}
          </Box>
        </Box>
      )}

      {unknown.length > 0 && (
        <Box>
          <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", mb: "14px" }}>
            Unknown — {unknown.length}
          </Box>
          <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(260px, 1fr))", gap: "10px" }}>
            {unknown.map((name, i) => <PersonCard key={i} name={name} />)}
          </Box>
        </Box>
      )}

      {faces.length === 0 && (
        <Box sx={{ fontFamily: monoFont, fontSize: "12px", color: "oklch(0.530 0.009 248)", textAlign: "center", py: "48px" }}>
          No faces detected in this scan.
        </Box>
      )}
    </Box>
  );
}
