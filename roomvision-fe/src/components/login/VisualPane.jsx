import { Box, keyframes } from "@mui/material";

const ACCENT = "oklch(0.840 0.060 214)";
const ACCENT_DIM = "oklch(0.840 0.060 214 / 0.30)";

const lock = keyframes`
  0%, 100% { opacity: 0.55; }
  45%, 70%  { opacity: 1; }
`;

const sweep = keyframes`
  0%   { transform: translateY(-4vh); }
  100% { transform: translateY(104vh); }
`;

const blink = keyframes`
  0%, 100% { opacity: 1; }
  50%       { opacity: 0.25; }
`;

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';

const BOXES = [
  {
    top: "22%",
    left: "18%",
    width: 120,
    height: 150,
    tag: "ID·0421 · 98%",
    delay: "0s",
  },
  {
    top: "34%",
    left: "46%",
    width: 96,
    height: 122,
    tag: "ID·0419 · 95%",
    delay: "0.5s",
  },
  {
    top: "18%",
    left: "70%",
    width: 104,
    height: 132,
    tag: "ID·0417 · 97%",
    delay: "1.1s",
  },
  {
    top: "56%",
    left: "32%",
    width: 84,
    height: 106,
    tag: "SCAN · 71%",
    delay: "1.7s",
  },
];

const CORNERS = [
  { top: "-1px", left: "-1px", borderRight: "none", borderBottom: "none" },
  { top: "-1px", right: "-1px", borderLeft: "none", borderBottom: "none" },
  { bottom: "-1px", left: "-1px", borderRight: "none", borderTop: "none" },
  { bottom: "-1px", right: "-1px", borderLeft: "none", borderTop: "none" },
];

function DetBox({ top, left, width, height, tag, delay }) {
  return (
    <Box
      sx={{
        position: "absolute",
        top,
        left,
        width: `${width}px`,
        height: `${height}px`,
        border: `1px solid ${ACCENT_DIM}`,
        pointerEvents: "none",
        animation: `${lock} 5.5s ease-in-out infinite`,
        animationDelay: delay,
        "@media (prefers-reduced-motion: reduce)": {
          animation: "none",
          opacity: 0.7,
        },
      }}
    >
      {CORNERS.map((pos, i) => (
        <Box
          key={i}
          sx={{
            position: "absolute",
            width: "12px",
            height: "12px",
            border: `1.5px solid ${ACCENT}`,
            ...pos,
          }}
        />
      ))}
      <Box
        sx={{
          position: "absolute",
          top: "-1px",
          left: "-1px",
          transform: "translateY(-100%)",
          background: ACCENT,
          color: "oklch(0.16 0.02 248)",
          fontFamily: monoFont,
          fontSize: "9.5px",
          fontWeight: 600,
          letterSpacing: "0.06em",
          padding: "2px 6px",
          whiteSpace: "nowrap",
        }}
      >
        {tag}
      </Box>
    </Box>
  );
}

export default function VisualPane() {
  return (
    <Box sx={{ position: "absolute", inset: 0, width: "100%", height: "100%" }}>
      {/* Grid overlay */}
      <Box
        sx={{
          position: "absolute",
          inset: 0,
          pointerEvents: "none",
          backgroundImage: `
            linear-gradient(oklch(1 0 0 / 0.028) 1px, transparent 1px),
            linear-gradient(90deg, oklch(1 0 0 / 0.028) 1px, transparent 1px)
          `,
          backgroundSize: "46px 46px",
        }}
      />

      {/* Scrim */}
      <Box
        sx={{
          position: "absolute",
          inset: 0,
          pointerEvents: "none",
          background: `
            radial-gradient(120% 90% at 70% 20%, transparent 30%, oklch(0.135 0.006 248 / 0.55) 100%),
            linear-gradient(to top, oklch(0.12 0.006 248 / 0.92) 0%, oklch(0.12 0.006 248 / 0.15) 42%, transparent 70%)
          `,
        }}
      />

      {/* Detection boxes */}
      {BOXES.map((b) => (
        <DetBox key={b.tag} {...b} />
      ))}

      {/* Scan sweep */}
      <Box
        sx={{
          position: "absolute",
          left: 0,
          right: 0,
          height: "2px",
          background: `linear-gradient(90deg, transparent, ${ACCENT}, transparent)`,
          boxShadow: `0 0 18px 2px oklch(0.84 0.06 214 / 0.35)`,
          pointerEvents: "none",
          animation: `${sweep} 6s linear infinite`,
          "@media (prefers-reduced-motion: reduce)": { display: "none" },
        }}
      />

      {/* HUD top bar */}
      <Box
        sx={{
          position: "absolute",
          top: "28px",
          left: "32px",
          right: "32px",
          display: "flex",
          justifyContent: "space-between",
          alignItems: "center",
          fontFamily: monoFont,
          fontSize: "11px",
          letterSpacing: "0.1em",
          color: "oklch(0.700 0.008 248)",
          pointerEvents: "none",
        }}
      ></Box>

      {/* HUD bottom caption */}
      <Box
        sx={{
          position: "absolute",
          bottom: "36px",
          left: "36px",
          right: "36px",
          maxWidth: "560px",
          pointerEvents: "none",
        }}
      >
        <Box
          sx={{
            fontFamily: monoFont,
            fontSize: "11px",
            letterSpacing: "0.2em",
            textTransform: "uppercase",
            color: ACCENT,
            mb: "12px",
          }}
        >
          Full-room facial recognition
        </Box>
        <Box
          sx={{
            fontFamily: displayFont,
            color: "oklch(0.90 0.005 248)",
            fontSize: "16px",
            lineHeight: 1.65,
          }}
        >
          RoomVision is an advanced system designed for full-room facial
          recognition, capable of identifying and analyzing faces across an
          entire room in real time. It provides accurate detection, tracking,
          and insights to enhance security, access control, and smart
          environment management.
        </Box>
        <Box
          sx={{
            mt: "18px",
            display: "flex",
            gap: "26px",
            fontFamily: monoFont,
            fontSize: "11px",
            letterSpacing: "0.06em",
            color: "oklch(0.530 0.009 248)",
          }}
        ></Box>
      </Box>
    </Box>
  );
}
