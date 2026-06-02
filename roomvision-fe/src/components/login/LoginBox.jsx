import {
  Box,
  TextField,
  Button,
  Typography,
  InputAdornment,
} from "@mui/material";
import React, { useState } from "react";
import { Authenticate } from "../../services";
import { useNavigate } from "react-router-dom";
import BrandLockup from "./InfoSection";

const ACCENT = "oklch(0.840 0.060 214)";
const ACCENT_DIM = "oklch(0.840 0.060 214 / 0.16)";

const monoFont = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';

export default function LoginBox() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [showPassword, setShowPassword] = useState(false);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleLogin = async (e) => {
    if (e && e.preventDefault) e.preventDefault();

    if (!email || !password) {
      setError("Please fill in all fields");
      return;
    }

    setLoading(true);
    setError(null);

    try {
      await Authenticate(email, password);
      navigate("/rooms");
    } catch (err) {
      setError(err?.message || "Login failed. Please try again.");
      console.error("Login error:", err);
    } finally {
      setLoading(false);
    }
  };

  const fieldSx = {
    "& .MuiInputBase-root": {
      fontFamily: displayFont,
      fontSize: "15px",
      color: "oklch(0.960 0.004 248)",
      background: "oklch(0.205 0.007 248)",
      borderRadius: "10px",
      border: "1px solid oklch(0.305 0.010 248)",
      transition:
        "border-color .18s ease, box-shadow .18s ease, background .18s ease",
      "&:hover": {
        borderColor: "oklch(0.395 0.012 248)",
      },
      "&.Mui-focused": {
        borderColor: ACCENT,
        background: "oklch(0.195 0.007 248)",
        boxShadow: `0 0 0 3px ${ACCENT_DIM}`,
      },
    },
    "& .MuiInputBase-input": {
      padding: "14px 16px",
      "&::placeholder": {
        color: "oklch(0.530 0.009 248)",
        opacity: 1,
      },
    },
    "& .MuiOutlinedInput-notchedOutline": { border: "none" },
  };

  return (
    <Box
      sx={{
        display: "flex",
        flexDirection: "column",
        height: "100%",
        padding: "48px 60px",
        fontFamily: displayFont,
        "@media (max-width: 900px)": { padding: "36px 28px" },
      }}
    >
      <BrandLockup />

      {/* Form body */}
      <Box sx={{ my: "auto", width: "100%", maxWidth: "400px" }}>
        {/* Eyebrow */}
        <Box
          sx={{
            fontFamily: monoFont,
            fontSize: "11px",
            letterSpacing: "0.22em",
            textTransform: "uppercase",
            color: ACCENT,
            display: "flex",
            alignItems: "center",
            gap: "9px",
            mb: "18px",
            "&::before": {
              content: '""',
              width: "6px",
              height: "6px",
              borderRadius: "50%",
              background: ACCENT,
              boxShadow: `0 0 8px ${ACCENT}`,
              flexShrink: 0,
            },
          }}
        >
          Administrative Access
        </Box>

        <Typography
          component="h1"
          sx={{
            fontFamily: displayFont,
            fontSize: "30px",
            lineHeight: 1.1,
            fontWeight: 600,
            letterSpacing: "-0.02em",
            color: "oklch(0.960 0.004 248)",
            mb: "14px",
          }}
        >
          Sign in to the dashboard
        </Typography>

        <Typography
          sx={{
            fontFamily: displayFont,
            color: "oklch(0.700 0.008 248)",
            fontSize: "14.5px",
            lineHeight: 1.6,
            mb: "30px",
            maxWidth: "38ch",
          }}
        >
          Please enter your credentials in order to log in to the administrative
          dashboard.
        </Typography>

        {error && (
          <Typography
            sx={{
              color: "#ff6b6b",
              fontSize: "13px",
              mb: "16px",
              fontFamily: monoFont,
            }}
          >
            {error}
          </Typography>
        )}

        <Box
          component="form"
          onSubmit={handleLogin}
          sx={{ display: "flex", flexDirection: "column", gap: "18px" }}
        >
          {/* Email */}
          <Box sx={{ display: "flex", flexDirection: "column", gap: "8px" }}>
            <Box
              component="label"
              htmlFor="email"
              sx={{
                fontFamily: monoFont,
                fontSize: "11px",
                letterSpacing: "0.14em",
                textTransform: "uppercase",
                color: "oklch(0.530 0.009 248)",
              }}
            >
              Email address
            </Box>
            <TextField
              id="email"
              type="email"
              placeholder="name@organization.com"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              autoComplete="username"
              fullWidth
              sx={fieldSx}
            />
          </Box>

          {/* Password */}
          <Box sx={{ display: "flex", flexDirection: "column", gap: "8px" }}>
            <Box
              component="label"
              htmlFor="password"
              sx={{
                fontFamily: monoFont,
                fontSize: "11px",
                letterSpacing: "0.14em",
                textTransform: "uppercase",
                color: "oklch(0.530 0.009 248)",
              }}
            >
              Password
            </Box>
            <TextField
              id="password"
              type={showPassword ? "text" : "password"}
              placeholder="••••••••••••"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              autoComplete="current-password"
              fullWidth
              sx={fieldSx}
              InputProps={{
                endAdornment: (
                  <InputAdornment position="end">
                    <Box
                      component="button"
                      type="button"
                      onClick={() => setShowPassword((v) => !v)}
                      sx={{
                        background: "none",
                        border: "none",
                        color: "oklch(0.530 0.009 248)",
                        fontFamily: monoFont,
                        fontSize: "10px",
                        letterSpacing: "0.1em",
                        textTransform: "uppercase",
                        cursor: "pointer",
                        padding: "8px 10px",
                        borderRadius: "6px",
                        "&:hover": { color: "oklch(0.700 0.008 248)" },
                      }}
                    >
                      {showPassword ? "Hide" : "Show"}
                    </Box>
                  </InputAdornment>
                ),
              }}
            />
          </Box>

          {/* Submit */}
          <Button
            type="submit"
            disabled={loading}
            sx={{
              mt: "8px",
              width: "100%",
              background: "oklch(0.960 0.004 248)",
              color: "oklch(0.155 0.006 248)",
              fontFamily: displayFont,
              fontSize: "15px",
              fontWeight: 600,
              letterSpacing: "0.01em",
              borderRadius: "10px",
              padding: "15px 18px",
              textTransform: "none",
              display: "flex",
              alignItems: "center",
              justifyContent: "center",
              gap: "10px",
              transition: "transform .12s ease, box-shadow .18s ease",
              "&:hover": {
                background: "oklch(0.960 0.004 248)",
                boxShadow: `0 0 0 3px ${ACCENT_DIM}`,
              },
              "&:active": { transform: "translateY(1px)" },
              opacity: loading ? 0.7 : 1,
            }}
          >
            {loading ? "Signing in…" : "Enter dashboard"}
            {!loading && (
              <Box
                component="span"
                sx={{
                  fontFamily: monoFont,
                  transition: "transform .18s ease",
                  ".MuiButton-root:hover &": { transform: "translateX(3px)" },
                }}
              >
                →
              </Box>
            )}
          </Button>
        </Box>
      </Box>

      {/* Footer metadata strip */}
      <Box
        sx={{
          mt: "28px",
          display: "flex",
          gap: "18px",
          alignItems: "center",
          fontFamily: monoFont,
          fontSize: "10.5px",
          letterSpacing: "0.08em",
          color: "oklch(0.530 0.009 248)",
        }}
      >
        <span>ROOMVISION</span>
        <Box component="span" sx={{ color: "oklch(0.395 0.012 248)" }}>
          /
        </Box>
        <span>FACIAL RECOGNITION CORE</span>
      </Box>
    </Box>
  );
}
