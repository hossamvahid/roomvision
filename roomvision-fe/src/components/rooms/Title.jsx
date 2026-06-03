import { Box, TextField, CircularProgress, Typography } from "@mui/material";
import { useState } from "react";
import createRoom from "../../services/room/createRoom";

const monoFont    = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT      = "oklch(0.840 0.060 214)";
const ACCENT_DIM  = "oklch(0.840 0.060 214 / 0.16)";

const inputSx = {
  "& .MuiInputBase-root": {
    fontFamily: displayFont, fontSize: "14px",
    color: "oklch(0.960 0.004 248)",
    background: "oklch(0.205 0.007 248)",
    borderRadius: "9px",
    border: "1px solid oklch(0.305 0.010 248)",
    transition: "border-color .15s ease, box-shadow .15s ease",
    "&:hover": { borderColor: "oklch(0.395 0.012 248)" },
    "&.Mui-focused": { borderColor: ACCENT, boxShadow: `0 0 0 3px ${ACCENT_DIM}` },
  },
  "& .MuiInputBase-input": { padding: "12px 13px", "&::placeholder": { color: "oklch(0.530 0.009 248)", opacity: 1 } },
  "& .MuiOutlinedInput-notchedOutline": { border: "none" },
};

function AddRoomModal({ open, onClose, onCreated }) {
  const [roomName, setRoomName] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading]   = useState(false);
  const [error, setError]       = useState("");

  const handleClose = () => { setRoomName(""); setPassword(""); setError(""); onClose(); };

  const handleSubmit = async () => {
    if (!roomName.trim()) { setError("Room name is required."); return; }
    if (!password.trim()) { setError("Password is required."); return; }
    setLoading(true); setError("");
    try {
      await createRoom(roomName.trim(), password);
      handleClose();
      onCreated();
    } catch (err) {
      setError(typeof err === "string" ? err : "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  if (!open) return null;

  return (
    <Box onClick={handleClose} sx={{ position: "fixed", inset: 0, zIndex: 200, background: "oklch(0.1 0.006 248 / 0.66)", backdropFilter: "blur(3px)", display: "grid", placeItems: "center", padding: "24px" }}>
      <Box onClick={(e) => e.stopPropagation()} sx={{ width: "100%", maxWidth: "420px", background: "oklch(0.195 0.007 248)", border: "1px solid oklch(0.395 0.012 248)", borderRadius: "16px", boxShadow: "0 30px 70px oklch(0 0 0 / 0.55)" }}>
        <Box sx={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between", gap: "16px", padding: "22px 24px 0" }}>
          <Box>
            <Box sx={{ fontFamily: displayFont, fontSize: "19px", fontWeight: 600, letterSpacing: "-0.01em", color: "oklch(0.960 0.004 248)" }}>Add room</Box>
            <Box sx={{ fontFamily: displayFont, fontSize: "13px", color: "oklch(0.700 0.008 248)", mt: "6px" }}>Enter a name and password for the new room.</Box>
          </Box>
          <Box component="button" onClick={handleClose} sx={{ width: "32px", height: "32px", flexShrink: 0, borderRadius: "8px", background: "oklch(0.205 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", color: "oklch(0.700 0.008 248)", cursor: "pointer", display: "grid", placeItems: "center", "&:hover": { color: "oklch(0.960 0.004 248)" } }}>
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M18 6 6 18M6 6l12 12"/></svg>
          </Box>
        </Box>
        <Box sx={{ padding: "20px 24px 24px", display: "flex", flexDirection: "column", gap: "14px" }}>
          <Box>
            <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", mb: "7px" }}>Room name</Box>
            <TextField placeholder="e.g. Conference Room A" value={roomName} onChange={(e) => setRoomName(e.target.value)} fullWidth sx={inputSx} />
          </Box>
          <Box>
            <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", mb: "7px" }}>Password</Box>
            <TextField type="password" placeholder="••••••••••••" value={password} onChange={(e) => setPassword(e.target.value)} onKeyDown={(e) => { if (e.key === "Enter") handleSubmit(); }} fullWidth sx={inputSx} />
          </Box>
          {error && <Typography sx={{ color: "#ff6b6b", fontFamily: monoFont, fontSize: "12px" }}>{error}</Typography>}
          <Box sx={{ display: "flex", justifyContent: "flex-end", gap: "10px", mt: "4px" }}>
            <Box component="button" onClick={handleClose} sx={{ background: "oklch(0.205 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", color: "oklch(0.700 0.008 248)", borderRadius: "9px", padding: "11px 16px", fontFamily: displayFont, fontSize: "14px", fontWeight: 500, cursor: "pointer", "&:hover": { color: "oklch(0.960 0.004 248)" } }}>Cancel</Box>
            <Box component="button" onClick={handleSubmit} disabled={loading} sx={{ display: "inline-flex", alignItems: "center", gap: "8px", background: "oklch(0.960 0.004 248)", color: "oklch(0.155 0.006 248)", border: "none", borderRadius: "9px", padding: "11px 16px", fontFamily: displayFont, fontSize: "14px", fontWeight: 600, cursor: "pointer", "&:hover": { boxShadow: `0 0 0 3px ${ACCENT_DIM}` }, "&:disabled": { opacity: 0.5 } }}>
              {loading ? <CircularProgress size={14} color="inherit" /> : "Create room"}
            </Box>
          </Box>
        </Box>
      </Box>
    </Box>
  );
}

export default function Title({ isAdmin, onRoomCreated }) {
  const [modalOpen, setModalOpen] = useState(false);

  return (
    <>
      <Box sx={{ display: "flex", alignItems: "flex-end", justifyContent: "space-between", gap: "24px", mb: "26px", flexWrap: "wrap" }}>
        <Box>
          <Box sx={{ fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.2em", textTransform: "uppercase", color: ACCENT, mb: "9px" }}>
            Live monitoring
          </Box>
          <Box component="h1" sx={{ margin: 0, fontFamily: displayFont, fontSize: "27px", fontWeight: 600, letterSpacing: "-0.02em", color: "oklch(0.960 0.004 248)" }}>
            Rooms
          </Box>
          <Box sx={{ fontFamily: displayFont, fontSize: "14px", color: "oklch(0.700 0.008 248)", mt: "7px" }}>
            Real-time occupancy across all monitored spaces.
          </Box>
        </Box>

        {isAdmin && (
          <Box component="button" onClick={() => setModalOpen(true)}
            sx={{ display: "inline-flex", alignItems: "center", gap: "8px", background: "oklch(0.960 0.004 248)", color: "oklch(0.155 0.006 248)", border: "none", borderRadius: "9px", padding: "10px 16px", fontFamily: displayFont, fontSize: "14px", fontWeight: 600, cursor: "pointer", transition: "box-shadow .15s ease, transform .12s ease", "&:hover": { boxShadow: `0 0 0 3px ${ACCENT_DIM}` }, "&:active": { transform: "translateY(1px)" } }}>
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M12 5v14M5 12h14"/></svg>
            Add room
          </Box>
        )}
      </Box>

      <AddRoomModal open={modalOpen} onClose={() => setModalOpen(false)} onCreated={onRoomCreated} />
    </>
  );
}
