import { Box, CircularProgress, TextField, Typography } from "@mui/material";
import { useRef, useState } from "react";
import createPerson from "../../services/person/createPerson";

const monoFont    = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';
const ACCENT      = "oklch(0.840 0.060 214)";
const ACCENT_DIM  = "oklch(0.840 0.060 214 / 0.16)";

const inputSx = {
  "& .MuiInputBase-root": {
    fontFamily: displayFont,
    fontSize: "14px",
    color: "oklch(0.960 0.004 248)",
    background: "oklch(0.205 0.007 248)",
    borderRadius: "9px",
    border: "1px solid oklch(0.305 0.010 248)",
    transition: "border-color .15s ease, box-shadow .15s ease",
    "&:hover": { borderColor: "oklch(0.395 0.012 248)" },
    "&.Mui-focused": { borderColor: ACCENT, boxShadow: `0 0 0 3px ${ACCENT_DIM}` },
  },
  "& .MuiInputBase-input": {
    padding: "12px 13px",
    "&::placeholder": { color: "oklch(0.530 0.009 248)", opacity: 1 },
  },
  "& .MuiOutlinedInput-notchedOutline": { border: "none" },
};

function FieldLabel({ children }) {
  return (
    <Box
      component="label"
      sx={{
        fontFamily: monoFont,
        fontSize: "10.5px",
        letterSpacing: "0.12em",
        textTransform: "uppercase",
        color: "oklch(0.530 0.009 248)",
        display: "block",
        mb: "7px",
      }}
    >
      {children}
    </Box>
  );
}

export default function AddPersonModal({ open, onClose, onCreated }) {
  const [name, setName]       = useState("");
  const [file, setFile]       = useState(null);
  const [preview, setPreview] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState("");
  const inputRef              = useRef(null);

  const handleClose = () => {
    setName("");
    setFile(null);
    setPreview(null);
    setError("");
    onClose();
  };

  const handleFileChange = (e) => {
    const selected = e.target.files[0];
    if (!selected) return;
    setFile(selected);
    setPreview(URL.createObjectURL(selected));
    setError("");
  };

  const handleDrop = (e) => {
    e.preventDefault();
    const dropped = e.dataTransfer.files[0];
    if (!dropped) return;
    setFile(dropped);
    setPreview(URL.createObjectURL(dropped));
    setError("");
  };

  const handleSubmit = async () => {
    if (!name.trim()) { setError("Name is required."); return; }
    if (!file)        { setError("Please select a photo."); return; }
    setLoading(true);
    setError("");
    try {
      await createPerson(name.trim(), file);
      handleClose();
      if (onCreated) onCreated();
    } catch (err) {
      const msg = err?.response?.data?.Error;
      setError(msg || "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  if (!open) return null;

  return (
    <Box
      onClick={handleClose}
      sx={{
        position: "fixed",
        inset: 0,
        zIndex: 200,
        background: "oklch(0.1 0.006 248 / 0.66)",
        backdropFilter: "blur(3px)",
        display: "grid",
        placeItems: "center",
        padding: "24px",
      }}
    >
      <Box
        onClick={(e) => e.stopPropagation()}
        sx={{
          width: "100%",
          maxWidth: "460px",
          background: "oklch(0.195 0.007 248)",
          border: "1px solid oklch(0.395 0.012 248)",
          borderRadius: "16px",
          boxShadow: "0 30px 70px oklch(0 0 0 / 0.55)",
          maxHeight: "calc(100vh - 48px)",
          overflowY: "auto",
        }}
      >
        {/* Header */}
        <Box
          sx={{
            display: "flex",
            alignItems: "flex-start",
            justifyContent: "space-between",
            gap: "16px",
            padding: "22px 24px 0",
          }}
        >
          <Box>
            <Box
              sx={{
                fontFamily: displayFont,
                fontSize: "19px",
                fontWeight: 600,
                letterSpacing: "-0.01em",
                color: "oklch(0.960 0.004 248)",
              }}
            >
              Add person
            </Box>
            <Box
              sx={{
                fontFamily: displayFont,
                fontSize: "13px",
                color: "oklch(0.700 0.008 248)",
                mt: "6px",
              }}
            >
              Enter a name and upload a clear photo of the person's face.
            </Box>
          </Box>

          {/* Close button */}
          <Box
            component="button"
            onClick={handleClose}
            sx={{
              width: "32px",
              height: "32px",
              flexShrink: 0,
              borderRadius: "8px",
              background: "oklch(0.205 0.007 248)",
              border: "1px solid oklch(0.305 0.010 248)",
              color: "oklch(0.700 0.008 248)",
              cursor: "pointer",
              display: "grid",
              placeItems: "center",
              "&:hover": { color: "oklch(0.960 0.004 248)", borderColor: "oklch(0.395 0.012 248)" },
            }}
          >
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
              <path d="M18 6 6 18M6 6l12 12" />
            </svg>
          </Box>
        </Box>

        {/* Body */}
        <Box sx={{ padding: "20px 24px 24px", display: "flex", flexDirection: "column", gap: "16px" }}>

          {/* Name field */}
          <Box>
            <FieldLabel>Full name</FieldLabel>
            <TextField
              placeholder="e.g. Maria Ionescu"
              value={name}
              onChange={(e) => setName(e.target.value)}
              fullWidth
              sx={inputSx}
            />
          </Box>

          {/* Photo upload */}
          <Box>
            <FieldLabel>Photo</FieldLabel>

            {preview ? (
              /* Preview of selected photo */
              <Box sx={{ position: "relative" }}>
                <Box
                  component="img"
                  src={preview}
                  alt="preview"
                  sx={{
                    width: "100%",
                    height: "220px",
                    objectFit: "cover",
                    borderRadius: "10px",
                    border: `1px solid ${ACCENT}`,
                    display: "block",
                  }}
                />
                {/* Change photo button */}
                <Box
                  component="button"
                  onClick={() => inputRef.current?.click()}
                  sx={{
                    position: "absolute",
                    bottom: "10px",
                    right: "10px",
                    background: "oklch(0.155 0.006 248 / 0.85)",
                    border: "1px solid oklch(0.395 0.012 248)",
                    borderRadius: "8px",
                    color: "oklch(0.960 0.004 248)",
                    fontFamily: displayFont,
                    fontSize: "12px",
                    fontWeight: 500,
                    padding: "7px 12px",
                    cursor: "pointer",
                    backdropFilter: "blur(6px)",
                    "&:hover": { borderColor: ACCENT },
                  }}
                >
                  Change photo
                </Box>
              </Box>
            ) : (
              /* Drop zone */
              <Box
                onDrop={handleDrop}
                onDragOver={(e) => e.preventDefault()}
                onClick={() => inputRef.current?.click()}
                sx={{
                  height: "160px",
                  borderRadius: "10px",
                  border: "2px dashed oklch(0.305 0.010 248)",
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                  justifyContent: "center",
                  gap: "10px",
                  cursor: "pointer",
                  transition: "border-color .15s ease, background .15s ease",
                  "&:hover": {
                    borderColor: ACCENT,
                    background: "oklch(0.840 0.060 214 / 0.04)",
                  },
                }}
              >
                <Box
                  sx={{
                    width: "40px",
                    height: "40px",
                    borderRadius: "50%",
                    background: "oklch(0.205 0.007 248)",
                    border: "1px solid oklch(0.305 0.010 248)",
                    display: "grid",
                    placeItems: "center",
                    color: "oklch(0.530 0.009 248)",
                  }}
                >
                  <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8">
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4" />
                    <polyline points="17 8 12 3 7 8" />
                    <line x1="12" y1="3" x2="12" y2="15" />
                  </svg>
                </Box>
                <Box sx={{ textAlign: "center" }}>
                  <Box sx={{ fontFamily: displayFont, fontSize: "14px", color: "oklch(0.700 0.008 248)" }}>
                    Click to upload or drag & drop
                  </Box>
                  <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", color: "oklch(0.530 0.009 248)", mt: "4px" }}>
                    JPG, PNG, WEBP
                  </Box>
                </Box>
              </Box>
            )}

            <input
              ref={inputRef}
              type="file"
              accept="image/*"
              style={{ display: "none" }}
              onChange={handleFileChange}
            />
          </Box>

          {/* Error */}
          {error && (
            <Typography
              sx={{ color: "#ff6b6b", fontFamily: monoFont, fontSize: "12px", lineHeight: 1.5 }}
            >
              {error}
            </Typography>
          )}

          {/* Actions */}
          <Box sx={{ display: "flex", justifyContent: "flex-end", gap: "10px", mt: "4px" }}>
            <Box
              component="button"
              onClick={handleClose}
              sx={{
                background: "oklch(0.205 0.007 248)",
                border: "1px solid oklch(0.305 0.010 248)",
                color: "oklch(0.700 0.008 248)",
                borderRadius: "9px",
                padding: "11px 16px",
                fontFamily: displayFont,
                fontSize: "14px",
                fontWeight: 500,
                cursor: "pointer",
                "&:hover": { color: "oklch(0.960 0.004 248)", borderColor: "oklch(0.395 0.012 248)" },
              }}
            >
              Cancel
            </Box>
            <Box
              component="button"
              onClick={handleSubmit}
              disabled={loading}
              sx={{
                display: "inline-flex",
                alignItems: "center",
                gap: "8px",
                background: "oklch(0.960 0.004 248)",
                color: "oklch(0.155 0.006 248)",
                border: "none",
                borderRadius: "9px",
                padding: "11px 16px",
                fontFamily: displayFont,
                fontSize: "14px",
                fontWeight: 600,
                cursor: "pointer",
                transition: "box-shadow .15s ease",
                "&:hover": { boxShadow: `0 0 0 3px ${ACCENT_DIM}` },
                "&:disabled": { opacity: 0.5, cursor: "not-allowed" },
              }}
            >
              {loading ? <CircularProgress size={14} color="inherit" /> : "Add person"}
            </Box>
          </Box>
        </Box>
      </Box>
    </Box>
  );
}
