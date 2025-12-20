import { Box, TextField, Button, Typography } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import React, { useState } from "react";
import { User, Lock } from "lucide-react";
import { Authenticate } from "../../services";
import { useNavigate } from "react-router-dom";

export default function LoginBox() {
  const theme = useTheme();
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
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
      const response = await Authenticate(email, password);
      navigate("/rooms");
    } catch (err) {
      setError(err?.message || "Login failed. Please try again.");
      console.error("Login error:", err);
    } finally {
      setLoading(false);
    }
  };

  return (
    <>
      <Box
        sx={{
          bgcolor: theme.palette.primary.main,
          borderRadius: "24px",
          padding: 6,
          display: "flex",
          flexDirection: "column",
          gap: 4,
          boxShadow: "0 8px 24px rgba(0,0,0,0.2)",
        }}
      >
        <Typography
          variant="h4"
          sx={{
            color: theme.palette.text.secondary,
            fontWeight: 600,
            mb: 2,
            fontSize: "2rem",
          }}
        >
          Login
        </Typography>

        {error && (
          <Typography
            sx={{
              color: "#ff6b6b",
              fontSize: "0.9rem",
              textAlign: "center",
            }}
          >
            {error}
          </Typography>
        )}

        <form onSubmit={handleLogin} style={{ display: "contents" }}>
          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 2,
              borderBottom: "2px solid rgba(255,255,255,0.3)",
              pb: 1,
            }}
          >
            <User
              size={28}
              color={theme.palette.text.secondary}
              strokeWidth={2}
            />
            <TextField
              placeholder="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              variant="standard"
              fullWidth
              InputProps={{
                disableUnderline: true,
              }}
              sx={{
                "& .MuiInput-input::placeholder": {
                  color: "rgba(255,255,255,0.7)",
                  opacity: 1,
                },
                "& .MuiInput-input": {
                  color: theme.palette.text.secondary,
                  fontSize: "1.1rem",
                },
              }}
            />
          </Box>

          <Box
            sx={{
              display: "flex",
              alignItems: "center",
              gap: 2,
              borderBottom: "2px solid rgba(255,255,255,0.3)",
              pb: 1,
            }}
          >
            <Lock
              size={28}
              color={theme.palette.text.secondary}
              strokeWidth={2}
            />
            <TextField
              placeholder="Password"
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              variant="standard"
              fullWidth
              InputProps={{
                disableUnderline: true,
              }}
              sx={{
                "& .MuiInput-input::placeholder": {
                  color: "rgba(255,255,255,0.7)",
                  opacity: 1,
                },
                "& .MuiInput-input": {
                  color: theme.palette.text.secondary,
                  fontSize: "1.1rem",
                },
              }}
            />
          </Box>

          <Button
            type="submit"
            disabled={loading}
            variant="contained"
            sx={{
              bgcolor: theme.palette.text.secondary,
              color: theme.palette.primary.main,
              fontWeight: 700,
              fontSize: "1.1rem",
              borderRadius: "50px",
              py: 1.8,
              mt: 2,
              "&:hover": {
                bgcolor: "rgba(255,255,255,0.9)",
              },
              opacity: loading ? 0.7 : 1,
              cursor: loading ? "not-allowed" : "pointer",
            }}
          >
            {loading ? "LOGGING IN..." : "LOGIN"}
          </Button>
        </form>
      </Box>
    </>
  );
}
