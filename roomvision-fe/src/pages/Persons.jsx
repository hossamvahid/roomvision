import { Box, CircularProgress, TextField, Typography } from "@mui/material";
import { useRef, useState } from "react";
import createPerson from "../services/person/createPerson";

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

export default function Persons() {
  const [name, setName]       = useState("");
  const [file, setFile]       = useState(null);
  const [preview, setPreview] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState("");
  const [success, setSuccess] = useState(false);
  const inputRef              = useRef(null);

  const reset = () => {
    setName("");
    setFile(null);
    setPreview(null);
    setError("");
    setSuccess(false);
  };

  const handleFileChange = (e) => {
    const selected = e.target.files[0];
    if (!selected) return;
    setFile(selected);
    setPreview(URL.createObjectURL(selected));
    setError("");
    setSuccess(false);
  };

  const handleDrop = (e) => {
    e.preventDefault();
    const dropped = e.dataTransfer.files[0];
    if (!dropped) return;
    setFile(dropped);
    setPreview(URL.createObjectURL(dropped));
    setError("");
    setSuccess(false);
  };

  const handleSubmit = async () => {
    if (!name.trim()) { setError("Name is required."); return; }
    if (!file)        { setError("Please select a photo."); return; }
    setLoading(true);
    setError("");
    setSuccess(false);
    try {
      await createPerson(name.trim(), file);
      setSuccess(true);
      reset();
    } catch (err) {
      const msg = err?.response?.data?.Error;
      setError(msg || "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box
      sx={{
        minHeight: "100vh",
        bgcolor: "oklch(0.135 0.006 248)",
        py: "48px",
        px: "28px",
        display: "flex",
        justifyContent: "center",
        alignItems: "flex-start",
      }}
    >
      <Box sx={{ width: "100%", maxWidth: "520px" }}>

        {/* Page header */}
        <Box sx={{ mb: "36px" }}>
          <Box sx={{ fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.2em", textTransform: "uppercase", color: ACCENT, mb: "9px" }}>
            Face database
          </Box>
          <Box component="h1" sx={{ margin: 0, fontFamily: displayFont, fontSize: "27px", fontWeight: 600, letterSpacing: "-0.02em", color: "oklch(0.960 0.004 248)" }}>
            Register person
          </Box>
          <Box sx={{ fontFamily: displayFont, fontSize: "14px", color: "oklch(0.700 0.008 248)", mt: "8px" }}>
            Add a new person to the recognition database. Make sure the photo clearly shows their face.
          </Box>
        </Box>

        {/* Card */}
        <Box
          sx={{
            background: "oklch(0.195 0.007 248)",
            border: "1px solid oklch(0.305 0.010 248)",
            borderRadius: "16px",
            padding: "28px",
            display: "flex",
            flexDirection: "column",
            gap: "20px",
          }}
        >
          {/* Name field */}
          <Box>
            <FieldLabel>Full name</FieldLabel>
            <TextField
              placeholder="e.g. Maria Ionescu"
              value={name}
              onChange={(e) => { setName(e.target.value); setError(""); setSuccess(false); }}
              fullWidth
              sx={inputSx}
            />
          </Box>

          {/* Photo upload */}
          <Box>
            <FieldLabel>Photo</FieldLabel>

            {preview ? (
              <Box sx={{ position: "relative" }}>
                <Box
                  component="img"
                  src={preview}
                  alt="preview"
                  sx={{
                    width: "100%",
                    height: "260px",
                    objectFit: "cover",
                    objectPosition: "top",
                    borderRadius: "10px",
                    border: `1px solid ${ACCENT}`,
                    display: "block",
                  }}
                />
                {/* Change photo overlay button */}
                <Box
                  component="button"
                  onClick={() => inputRef.current?.click()}
                  sx={{
                    position: "absolute",
                    bottom: "12px",
                    right: "12px",
                    background: "oklch(0.155 0.006 248 / 0.88)",
                    border: "1px solid oklch(0.395 0.012 248)",
                    borderRadius: "8px",
                    color: "oklch(0.960 0.004 248)",
                    fontFamily: displayFont,
                    fontSize: "12px",
                    fontWeight: 500,
                    padding: "8px 13px",
                    cursor: "pointer",
                    backdropFilter: "blur(6px)",
                    display: "inline-flex",
                    alignItems: "center",
                    gap: "6px",
                    "&:hover": { borderColor: ACCENT },
                  }}
                >
                  <svg width="13" height="13" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                    <polyline points="17 8 12 3 7 8"/>
                    <line x1="12" y1="3" x2="12" y2="15"/>
                  </svg>
                  Change photo
                </Box>
              </Box>
            ) : (
              <Box
                onDrop={handleDrop}
                onDragOver={(e) => e.preventDefault()}
                onClick={() => inputRef.current?.click()}
                sx={{
                  height: "180px",
                  borderRadius: "10px",
                  border: "2px dashed oklch(0.305 0.010 248)",
                  display: "flex",
                  flexDirection: "column",
                  alignItems: "center",
                  justifyContent: "center",
                  gap: "12px",
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
                    width: "44px",
                    height: "44px",
                    borderRadius: "50%",
                    background: "oklch(0.205 0.007 248)",
                    border: "1px solid oklch(0.305 0.010 248)",
                    display: "grid",
                    placeItems: "center",
                    color: "oklch(0.530 0.009 248)",
                  }}
                >
                  <svg width="20" height="20" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8">
                    <path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/>
                    <polyline points="17 8 12 3 7 8"/>
                    <line x1="12" y1="3" x2="12" y2="15"/>
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
            <Typography sx={{ color: "#ff6b6b", fontFamily: monoFont, fontSize: "12px", lineHeight: 1.5 }}>
              {error}
            </Typography>
          )}

          {/* Success */}
          {success && (
            <Box
              sx={{
                display: "flex",
                alignItems: "center",
                gap: "10px",
                background: "oklch(0.840 0.060 214 / 0.10)",
                border: "1px solid oklch(0.840 0.060 214 / 0.35)",
                borderRadius: "9px",
                padding: "12px 14px",
              }}
            >
              <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke={ACCENT} strokeWidth="2">
                <polyline points="20 6 9 17 4 12"/>
              </svg>
              <Box sx={{ fontFamily: displayFont, fontSize: "14px", color: ACCENT }}>
                Person registered successfully.
              </Box>
            </Box>
          )}

          {/* Submit */}
          <Box
            component="button"
            onClick={handleSubmit}
            disabled={loading}
            sx={{
              display: "inline-flex",
              alignItems: "center",
              justifyContent: "center",
              gap: "8px",
              background: "oklch(0.960 0.004 248)",
              color: "oklch(0.155 0.006 248)",
              border: "none",
              borderRadius: "9px",
              padding: "13px",
              fontFamily: displayFont,
              fontSize: "14px",
              fontWeight: 600,
              cursor: "pointer",
              transition: "box-shadow .15s ease",
              "&:hover": { boxShadow: `0 0 0 3px ${ACCENT_DIM}` },
              "&:disabled": { opacity: 0.5, cursor: "not-allowed" },
            }}
          >
            {loading
              ? <CircularProgress size={16} color="inherit" />
              : (
                <>
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2">
                    <path d="M16 19v-1a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v1"/>
                    <circle cx="9" cy="7" r="3.2"/>
                    <path d="M19 8v5M21.5 10.5h-5"/>
                  </svg>
                  Register person
                </>
              )
            }
          </Box>
        </Box>
      </Box>
    </Box>
  );
}
