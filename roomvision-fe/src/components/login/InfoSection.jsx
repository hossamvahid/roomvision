import { Box } from "@mui/material";

const ACCENT = "oklch(0.840 0.060 214)";

export default function BrandLockup() {
  return (
    <Box sx={{ display: "flex", alignItems: "center", gap: "12px" }}>
      {/* Detection-frame logo mark */}
      <Box
        aria-hidden="true"
        sx={{
          position: "relative",
          width: "30px",
          height: "30px",
          flexShrink: 0,
        }}
      >
        {/* Corner brackets */}
        {[
          { top: 0, left: 0, borderRight: "none", borderBottom: "none" },
          { top: 0, right: 0, borderLeft: "none", borderBottom: "none" },
          { bottom: 0, left: 0, borderRight: "none", borderTop: "none" },
          { bottom: 0, right: 0, borderLeft: "none", borderTop: "none" },
        ].map((pos, i) => (
          <Box
            key={i}
            sx={{
              position: "absolute",
              width: "9px",
              height: "9px",
              border: `1.5px solid ${ACCENT}`,
              ...pos,
            }}
          />
        ))}
        {/* Center dot */}
        <Box
          sx={{
            position: "absolute",
            inset: 0,
            margin: "auto",
            width: "7px",
            height: "7px",
            borderRadius: "50%",
            background: ACCENT,
            boxShadow: `0 0 10px ${ACCENT}`,
          }}
        />
      </Box>

      {/* Wordmark */}
      <Box
        sx={{
          fontFamily: '"Space Grotesk", sans-serif',
          fontWeight: 600,
          fontSize: "19px",
          letterSpacing: "-0.01em",
          color: "oklch(0.960 0.004 248)",
        }}
      >
        Room
        <Box component="span" sx={{ color: "oklch(0.700 0.008 248)", fontWeight: 400 }}>
          Vision
        </Box>
      </Box>
    </Box>
  );
}
