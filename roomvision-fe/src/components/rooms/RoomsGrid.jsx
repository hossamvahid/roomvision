import { Box, CircularProgress } from "@mui/material";
import { useState, useEffect } from "react";
import { useNavigate } from "react-router-dom";
import { formatDate } from "../../utils/date";
import { handleNextPage, handlePreviousPage } from "../../utils/pagination";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT = "oklch(0.840 0.060 214)";
const ACCENT_DIM = "oklch(0.840 0.060 214 / 0.16)";

const MINI_BOXES = [
  { top: "26%", left: "14%", width: 30, height: 38 },
  { top: "38%", left: "46%", width: 26, height: 34 },
  { top: "30%", left: "72%", width: 28, height: 36 },
];

const CORNERS = [
  { top: "-1px", left: "-1px",   borderRight: "none", borderBottom: "none" },
  { top: "-1px", right: "-1px",  borderLeft: "none",  borderBottom: "none" },
  { bottom: "-1px", left: "-1px",  borderRight: "none", borderTop: "none"  },
  { bottom: "-1px", right: "-1px", borderLeft: "none",  borderTop: "none"  },
];

function RoomCard({ room }) {
  const navigate = useNavigate();

  return (
    <Box
      onClick={() => navigate(`/room-info?room=${room.roomName}`)}
      sx={{
        cursor: "pointer",
        background: "oklch(0.195 0.007 248)",
        border: "1px solid oklch(0.305 0.010 248)",
        borderRadius: "12px",
        overflow: "hidden",
        transition: "border-color .18s ease, transform .18s ease",
        "&:hover": {
          borderColor: "oklch(0.395 0.012 248)",
          transform: "translateY(-2px)",
          "& .view-arrow": { color: ACCENT },
        },
      }}
    >
      {/* Camera feed area */}
      <Box sx={{
        position: "relative",
        height: "124px",
        backgroundImage: `
          linear-gradient(oklch(1 0 0 / 0.03) 1px, transparent 1px),
          linear-gradient(90deg, oklch(1 0 0 / 0.03) 1px, transparent 1px)
        `,
        backgroundSize: "26px 26px",
        bgcolor: "oklch(0.135 0.006 248)",
        borderBottom: "1px solid oklch(0.305 0.010 248)",
        overflow: "hidden",
        "&::after": {
          content: '""',
          position: "absolute",
          inset: 0,
          background: "linear-gradient(to top, oklch(0.135 0.006 248 / 0.7), transparent 60%)",
        },
      }}>
        {MINI_BOXES.map((mb, i) => (
          <Box key={i} sx={{ position: "absolute", top: mb.top, left: mb.left, width: `${mb.width}px`, height: `${mb.height}px`, border: `1px solid oklch(0.84 0.06 214 / 0.4)` }}>
            {CORNERS.map((pos, j) => (
              <Box key={j} sx={{ position: "absolute", width: "8px", height: "8px", border: `1.5px solid ${ACCENT}`, ...pos }} />
            ))}
          </Box>
        ))}
      </Box>

      {/* Card body */}
      <Box sx={{ padding: "18px" }}>
        <Box sx={{ fontFamily: displayFont, fontSize: "16px", fontWeight: 600, letterSpacing: "-0.01em", color: "oklch(0.960 0.004 248)", mb: "16px" }}>
          {room.roomName}
        </Box>

        <Box sx={{ display: "flex", alignItems: "baseline", gap: "10px", mb: "4px" }}>
          <Box sx={{ fontFamily: displayFont, fontSize: "38px", fontWeight: 600, letterSpacing: "-0.03em", lineHeight: 1, color: "oklch(0.960 0.004 248)", fontVariantNumeric: "tabular-nums" }}>
            {room.totalFaces}
          </Box>
          <Box sx={{ fontFamily: monoFont, fontSize: "10px", letterSpacing: "0.1em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)" }}>
            People
          </Box>
        </Box>

        {/* Footer */}
        <Box sx={{ display: "flex", alignItems: "center", justifyContent: "space-between", mt: "16px", pt: "14px", borderTop: "1px solid oklch(0.305 0.010 248)", fontFamily: monoFont, fontSize: "11px", color: "oklch(0.700 0.008 248)", letterSpacing: "0.02em" }}>
          <Box sx={{ display: "inline-flex", alignItems: "center", gap: "7px" }}>
            <svg width="12" height="12" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" style={{ color: "oklch(0.530 0.009 248)" }}>
              <path d="M21 12a9 9 0 1 1-3-6.7"/><path d="M21 4v5h-5"/>
            </svg>
            Scanned <Box component="b" sx={{ color: "oklch(0.960 0.004 248)", fontWeight: 500, ml: "4px" }}>{formatDate(room.scannedAt)}</Box>
          </Box>
          <Box className="view-arrow" sx={{ color: "oklch(0.530 0.009 248)", transition: "color .15s ease" }}>View →</Box>
        </Box>
      </Box>
    </Box>
  );
}

function PaginationBtn({ onClick, disabled, children }) {
  return (
    <Box component="button" onClick={onClick} disabled={disabled}
      sx={{ background: "oklch(0.195 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", color: disabled ? "oklch(0.395 0.012 248)" : "oklch(0.700 0.008 248)", fontFamily: displayFont, fontSize: "14px", fontWeight: 500, borderRadius: "9px", padding: "9px 18px", cursor: disabled ? "not-allowed" : "pointer", transition: "border-color .15s ease, color .15s ease", "&:not(:disabled):hover": { borderColor: "oklch(0.395 0.012 248)", color: "oklch(0.960 0.004 248)" } }}>
      {children}
    </Box>
  );
}

export default function RoomsGrid({ rooms, currentPage, totalPages, loading, setCurrentPage }) {
  if (loading) {
    return (
      <Box sx={{ display: "flex", justifyContent: "center", py: "64px" }}>
        <CircularProgress sx={{ color: ACCENT }} />
      </Box>
    );
  }

  return (
    <>
      <Box sx={{ display: "grid", gridTemplateColumns: "repeat(auto-fill, minmax(330px, 1fr))", gap: "18px", mb: "32px" }}>
        {rooms.map((room) => <RoomCard key={room.roomName} room={room} />)}
      </Box>

      {totalPages > 1 && (
        <Box sx={{ display: "flex", justifyContent: "center", alignItems: "center", gap: "16px" }}>
          <PaginationBtn onClick={() => handlePreviousPage({ setCurrentPage })} disabled={currentPage === 1}>Back</PaginationBtn>
          <Box sx={{ fontFamily: monoFont, fontSize: "12px", color: "oklch(0.530 0.009 248)", letterSpacing: "0.04em" }}>{currentPage} / {totalPages}</Box>
          <PaginationBtn onClick={() => handleNextPage({ currentPage, totalPages, setCurrentPage })} disabled={currentPage === totalPages}>Next</PaginationBtn>
        </Box>
      )}
    </>
  );
}
