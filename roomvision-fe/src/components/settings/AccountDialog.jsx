import {
  Dialog,
  DialogTitle,
  DialogContent,
  DialogActions,
  TextField,
  Button,
  CircularProgress,
  Typography,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { useState } from "react";
import createAccount from "../../services/account/createAccount";
import resetPassword from "../../services/account/resetPassword";

export default function AccountDialog({ mode, open, onClose, onSuccess }) {
  const theme = useTheme();
  const isCreate = mode === "create";

  const [email, setEmail] = useState("");
  const [name, setName] = useState("");
  const [newPassword, setNewPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleClose = () => {
    setEmail("");
    setName("");
    setNewPassword("");
    setError("");
    onClose();
  };

  const handleSubmit = async () => {
    setError("");
    setLoading(true);
    try {
      if (isCreate) {
        await createAccount(email, name);
      } else {
        await resetPassword(newPassword);
      }
      handleClose();
      if (onSuccess) onSuccess();
    } catch (err) {
      setError(typeof err === "string" ? err : "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  const fieldSx = {
    "& .MuiInputLabel-root": { color: theme.palette.primary.main },
    "& .MuiInputLabel-root.Mui-focused": { color: theme.palette.secondary.main },
    "& .MuiOutlinedInput-root": {
      color: theme.palette.primary.main,
      "& fieldset": { borderColor: theme.palette.primary.main },
      "&:hover fieldset": { borderColor: theme.palette.secondary.main },
      "&.Mui-focused fieldset": { borderColor: theme.palette.secondary.main },
    },
  };

  const buttonSx = {
    backgroundColor: theme.palette.secondary.main,
    textTransform: "none",
    borderRadius: "8px",
    fontWeight: 600,
    "&:hover": { backgroundColor: theme.palette.primary.main },
  };

  return (
    <Dialog open={open} onClose={handleClose} maxWidth="xs" fullWidth>
      <DialogTitle sx={{ fontWeight: 600, color: theme.palette.secondary.main }}>
        {isCreate ? "Create Account" : "Reset Password"}
      </DialogTitle>

      <DialogContent sx={{ display: "flex", flexDirection: "column", gap: 2, pt: "16px !important" }}>
        {isCreate ? (
          <>
            <TextField
              label="Email"
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              fullWidth
              size="small"
              sx={fieldSx}
            />
            <TextField
              label="Name"
              value={name}
              onChange={(e) => setName(e.target.value)}
              fullWidth
              size="small"
              sx={fieldSx}
            />
          </>
        ) : (
          <TextField
            label="New Password"
            type="password"
            value={newPassword}
            onChange={(e) => setNewPassword(e.target.value)}
            fullWidth
            size="small"
            sx={fieldSx}
          />
        )}

        {error && (
          <Typography variant="body2" color="error">
            {error}
          </Typography>
        )}
      </DialogContent>

      <DialogActions sx={{ px: 3, pb: 2 }}>
        <Button
          onClick={handleClose}
          variant="outlined"
          sx={{
            color: theme.palette.secondary.main,
            borderColor: theme.palette.secondary.main,
            textTransform: "none",
            borderRadius: "8px",
            fontWeight: 600,
            "&:hover": { borderColor: theme.palette.primary.main, color: theme.palette.primary.main },
          }}
        >
          Cancel
        </Button>
        <Button variant="contained" onClick={handleSubmit} disabled={loading} sx={buttonSx}>
          {loading ? <CircularProgress size={20} color="inherit" /> : isCreate ? "Create" : "Reset"}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
