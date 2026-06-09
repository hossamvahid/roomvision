import { Box, TextField, CircularProgress, Typography } from "@mui/material";
import { useEffect, useRef, useState } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { removeToken } from "../../utils/storage";
import { getRole } from "../../services";
import getAccountName from "../../services/account/getAccountName";
import getAccounts from "../../services/account/getAccounts";
import createAccount from "../../services/account/createAccount";
import deleteAccount from "../../services/account/deleteAccount";
import resetPassword from "../../services/account/resetPassword";
import createPerson from "../../services/person/createPerson";

const ACCENT      = "oklch(0.840 0.060 214)";
const ACCENT_DIM  = "oklch(0.840 0.060 214 / 0.16)";
const monoFont    = '"JetBrains Mono", ui-monospace, monospace';
const displayFont = '"Space Grotesk", sans-serif';

/* ── shared primitives ── */
function FieldLabel({ children }) {
  return (
    <Box component="label" sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", display: "block", mb: "7px" }}>
      {children}
    </Box>
  );
}

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
  "& .MuiInputBase-input": {
    padding: "12px 13px",
    "&::placeholder": { color: "oklch(0.530 0.009 248)", opacity: 1 },
  },
  "& .MuiOutlinedInput-notchedOutline": { border: "none" },
};

function BtnPrimary({ onClick, disabled, children }) {
  return (
    <Box component="button" onClick={onClick} disabled={disabled}
      sx={{ display: "inline-flex", alignItems: "center", gap: "8px", background: "oklch(0.960 0.004 248)", color: "oklch(0.155 0.006 248)", border: "none", borderRadius: "9px", padding: "11px 16px", fontFamily: displayFont, fontSize: "14px", fontWeight: 600, cursor: "pointer", transition: "box-shadow .15s ease", "&:hover": { boxShadow: `0 0 0 3px ${ACCENT_DIM}` }, "&:disabled": { opacity: 0.5, cursor: "not-allowed" } }}>
      {children}
    </Box>
  );
}

function BtnGhost({ onClick, children }) {
  return (
    <Box component="button" onClick={onClick}
      sx={{ background: "oklch(0.205 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", color: "oklch(0.700 0.008 248)", borderRadius: "9px", padding: "11px 16px", fontFamily: displayFont, fontSize: "14px", fontWeight: 500, cursor: "pointer", "&:hover": { color: "oklch(0.960 0.004 248)", borderColor: "oklch(0.395 0.012 248)" } }}>
      {children}
    </Box>
  );
}

/* ── Role segmented control ── */
function RoleSegment({ value, onChange }) {
  return (
    <Box sx={{ display: "flex", background: "oklch(0.195 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", borderRadius: "9px", padding: "3px", gap: "2px" }}>
      {["Admin", "Viewer"].map((role) => (
        <Box key={role} component="button" type="button" onClick={() => onChange(role)}
          sx={{ flex: 1, background: value === role ? "oklch(0.225 0.008 248)" : "none", border: "none", borderRadius: "6px", padding: "8px 12px", fontFamily: monoFont, fontSize: "11px", letterSpacing: "0.06em", textTransform: "uppercase", color: value === role ? "oklch(0.960 0.004 248)" : "oklch(0.530 0.009 248)", cursor: "pointer", transition: "all .15s ease", "&:hover": { color: value === role ? "oklch(0.960 0.004 248)" : "oklch(0.700 0.008 248)" } }}>
          {role}
        </Box>
      ))}
    </Box>
  );
}

/* ── Modal shell ── */
function Modal({ open, onClose, title, subtitle, wide, children }) {
  useEffect(() => {
    const handler = (e) => { if (e.key === "Escape") onClose(); };
    if (open) document.addEventListener("keydown", handler);
    return () => document.removeEventListener("keydown", handler);
  }, [open, onClose]);

  if (!open) return null;

  return (
    <Box onClick={onClose} sx={{ position: "fixed", inset: 0, zIndex: 200, background: "oklch(0.1 0.006 248 / 0.66)", backdropFilter: "blur(3px)", display: "grid", placeItems: "center", padding: "24px" }}>
      <Box onClick={(e) => e.stopPropagation()} sx={{ width: "100%", maxWidth: wide ? "540px" : "460px", background: "oklch(0.195 0.007 248)", border: "1px solid oklch(0.395 0.012 248)", borderRadius: "16px", boxShadow: "0 30px 70px oklch(0 0 0 / 0.55)", maxHeight: "calc(100vh - 48px)", overflowY: "auto" }}>
        <Box sx={{ display: "flex", alignItems: "flex-start", justifyContent: "space-between", gap: "16px", padding: "22px 24px 0" }}>
          <Box>
            <Box sx={{ fontFamily: displayFont, fontSize: "19px", fontWeight: 600, letterSpacing: "-0.01em", color: "oklch(0.960 0.004 248)" }}>{title}</Box>
            {subtitle && <Box sx={{ fontFamily: displayFont, fontSize: "13px", color: "oklch(0.700 0.008 248)", mt: "6px" }}>{subtitle}</Box>}
          </Box>
          <Box component="button" onClick={onClose} sx={{ width: "32px", height: "32px", flexShrink: 0, borderRadius: "8px", background: "oklch(0.205 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", color: "oklch(0.700 0.008 248)", cursor: "pointer", display: "grid", placeItems: "center", "&:hover": { color: "oklch(0.960 0.004 248)", borderColor: "oklch(0.395 0.012 248)" } }}>
            <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M18 6 6 18M6 6l12 12"/></svg>
          </Box>
        </Box>
        <Box sx={{ padding: "20px 24px 24px" }}>{children}</Box>
      </Box>
    </Box>
  );
}

/* ── Change Password modal ── */
function ChangePasswordModal({ open, onClose }) {
  const [newPassword, setNewPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  const handleClose = () => { setNewPassword(""); setError(""); onClose(); };

  const handleSubmit = async () => {
    if (!newPassword) { setError("Please enter a new password."); return; }
    setLoading(true); setError("");
    try {
      await resetPassword(newPassword);
      handleClose();
    } catch (err) {
      setError(typeof err === "string" ? err : "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal open={open} onClose={handleClose} title="Change password" subtitle="Enter a new password for your account.">
      <Box sx={{ mb: "15px" }}>
        <FieldLabel>New password</FieldLabel>
        <TextField type="password" placeholder="••••••••••••" value={newPassword} onChange={(e) => setNewPassword(e.target.value)} fullWidth sx={inputSx} />
      </Box>
      {error && <Typography sx={{ color: "#ff6b6b", fontFamily: monoFont, fontSize: "12px", mb: "12px" }}>{error}</Typography>}
      <Box sx={{ display: "flex", justifyContent: "flex-end", gap: "10px", mt: "6px" }}>
        <BtnGhost onClick={handleClose}>Cancel</BtnGhost>
        <BtnPrimary onClick={handleSubmit} disabled={loading}>{loading ? <CircularProgress size={14} color="inherit" /> : "Update password"}</BtnPrimary>
      </Box>
    </Modal>
  );
}

/* ── Administrators modal ── */
function AdminsModal({ open, onClose }) {
  const [accounts, setAccounts] = useState([]);
  const [loading, setLoading] = useState(false);
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [role, setRole] = useState("Viewer");
  const [submitting, setSubmitting] = useState(false);
  const [error, setError] = useState("");

  useEffect(() => {
    if (!open) return;
    setLoading(true);
    getAccounts(1, 50)
      .then((data) => setAccounts(data?.items ?? data ?? []))
      .catch(() => {})
      .finally(() => setLoading(false));
  }, [open]);

  const handleClose = () => { setName(""); setEmail(""); setRole("Viewer"); setError(""); onClose(); };

  const handleDelete = async (id) => {
    try {
      await deleteAccount(id);
      const data = await getAccounts(1, 50);
      setAccounts(data?.items ?? data ?? []);
    } catch (err) {
      setError(typeof err === "string" ? err : "Failed to delete account.");
    }
  };

  const handleAdd = async () => {
    if (!name || !email) { setError("Name and email are required."); return; }
    setSubmitting(true); setError("");
    try {
      await createAccount(email, name, role);
      const data = await getAccounts(1, 50);
      setAccounts(data?.items ?? data ?? []);
      setName(""); setEmail(""); setRole("Viewer");
    } catch (err) {
      setError(typeof err === "string" ? err : "Something went wrong.");
    } finally {
      setSubmitting(false);
    }
  };

  const initials = (n) => n ? n.split(" ").map((p) => p[0]).join("").slice(0, 2).toUpperCase() : "??";
  const isAdmin  = (r) => r === "Admin" || r === 0;

  return (
    <Modal open={open} onClose={handleClose} title="Administrators" subtitle="Manage administrative accounts." wide>
      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", py: "24px" }}><CircularProgress size={20} sx={{ color: ACCENT }} /></Box>
      ) : (
        <Box sx={{ display: "flex", flexDirection: "column", gap: "8px", mb: "22px" }}>
          {accounts.map((acc) => (
            <Box key={acc.id ?? acc.email} sx={{ display: "flex", alignItems: "center", gap: "12px", background: "oklch(0.205 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", borderRadius: "10px", padding: "11px 13px" }}>
              <Box sx={{ width: "34px", height: "34px", borderRadius: "50%", flexShrink: 0, background: "oklch(0.225 0.008 248)", border: "1px solid oklch(0.395 0.012 248)", display: "grid", placeItems: "center", fontFamily: monoFont, fontSize: "11px", fontWeight: 600, color: "oklch(0.960 0.004 248)" }}>
                {initials(acc.name)}
              </Box>
              <Box sx={{ minWidth: 0, flex: 1 }}>
                <Box sx={{ fontFamily: displayFont, fontSize: "14px", fontWeight: 500, color: "oklch(0.960 0.004 248)" }}>{acc.name}</Box>
                <Box sx={{ fontFamily: monoFont, fontSize: "11px", color: "oklch(0.530 0.009 248)", mt: "1px", overflow: "hidden", textOverflow: "ellipsis", whiteSpace: "nowrap" }}>{acc.email}</Box>
              </Box>
              <Box sx={{ fontFamily: monoFont, fontSize: "9.5px", letterSpacing: "0.08em", textTransform: "uppercase", padding: "4px 8px", borderRadius: "100px", border: isAdmin(acc.role) ? `1px solid oklch(0.84 0.06 214 / 0.35)` : "1px solid oklch(0.395 0.012 248)", color: isAdmin(acc.role) ? ACCENT : "oklch(0.700 0.008 248)", flexShrink: 0 }}>
                {isAdmin(acc.role) ? "Admin" : "Viewer"}
              </Box>
              {!isAdmin(acc.role) && (
                <Box component="button" onClick={() => handleDelete(acc.id)}
                  sx={{ width: "30px", height: "30px", borderRadius: "7px", flexShrink: 0, background: "none", border: "1px solid transparent", color: "oklch(0.530 0.009 248)", cursor: "pointer", display: "grid", placeItems: "center", transition: "color .14s ease, border-color .14s ease", "&:hover": { color: "oklch(0.680 0.150 28)", borderColor: "oklch(0.680 0.150 28 / 0.4)" } }}>
                  <svg width="15" height="15" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><polyline points="3 6 5 6 21 6"/><path d="M19 6l-1 14H6L5 6"/><path d="M10 11v6M14 11v6"/><path d="M9 6V4h6v2"/></svg>
                </Box>
              )}
            </Box>
          ))}
          {accounts.length === 0 && <Box sx={{ fontFamily: monoFont, fontSize: "12px", color: "oklch(0.530 0.009 248)", textAlign: "center", py: "16px" }}>No accounts found.</Box>}
        </Box>
      )}

      {/* add form */}
      <Box sx={{ fontFamily: monoFont, fontSize: "10.5px", letterSpacing: "0.12em", textTransform: "uppercase", color: "oklch(0.530 0.009 248)", mb: "12px" }}>Add account</Box>
      <Box sx={{ display: "grid", gridTemplateColumns: "1fr 1fr", gap: "12px", mb: "12px" }}>
        <Box>
          <FieldLabel>Name</FieldLabel>
          <TextField placeholder="Full name" value={name} onChange={(e) => setName(e.target.value)} fullWidth sx={inputSx} />
        </Box>
        <Box>
          <FieldLabel>Email</FieldLabel>
          <TextField type="email" placeholder="name@org.com" value={email} onChange={(e) => setEmail(e.target.value)} fullWidth sx={inputSx} />
        </Box>
      </Box>
      <Box sx={{ mb: "14px" }}>
        <FieldLabel>Role</FieldLabel>
        <RoleSegment value={role} onChange={setRole} />
      </Box>
      <Box sx={{ fontFamily: monoFont, fontSize: "11px", lineHeight: 1.5, color: "oklch(0.530 0.009 248)", mb: "14px" }}>
        A temporary password is generated automatically and emailed on creation.
      </Box>
      {error && <Typography sx={{ color: "#ff6b6b", fontFamily: monoFont, fontSize: "12px", mb: "12px" }}>{error}</Typography>}
      <Box sx={{ display: "flex", justifyContent: "flex-end", gap: "10px" }}>
        <BtnGhost onClick={handleClose}>Cancel</BtnGhost>
        <BtnPrimary onClick={handleAdd} disabled={submitting}>{submitting ? <CircularProgress size={14} color="inherit" /> : "Add account"}</BtnPrimary>
      </Box>
    </Modal>
  );
}

/* ── Add Person modal ── */
function AddPersonModal({ open, onClose }) {
  const [name, setName]       = useState("");
  const [file, setFile]       = useState(null);
  const [preview, setPreview] = useState(null);
  const [loading, setLoading] = useState(false);
  const [error, setError]     = useState("");
  const [success, setSuccess] = useState(false);
  const inputRef              = useRef(null);

  const handleClose = () => { setName(""); setFile(null); setPreview(null); setError(""); setSuccess(false); onClose(); };

  const handleFile = (f) => { if (!f) return; setFile(f); setPreview(URL.createObjectURL(f)); setError(""); setSuccess(false); };

  const handleSubmit = async () => {
    if (!name.trim()) { setError("Name is required."); return; }
    if (!file)        { setError("Please select a photo."); return; }
    setLoading(true); setError(""); setSuccess(false);
    try {
      await createPerson(name.trim(), file);
      setSuccess(true);
      setName(""); setFile(null); setPreview(null);
    } catch (err) {
      setError(err?.response?.data?.Error || "Something went wrong.");
    } finally {
      setLoading(false);
    }
  };

  return (
    <Modal open={open} onClose={handleClose} title="Add person" subtitle="Register a new face in the recognition database.">
      {/* Name */}
      <Box sx={{ mb: "15px" }}>
        <FieldLabel>Full name</FieldLabel>
        <TextField placeholder="e.g. Maria Ionescu" value={name} onChange={(e) => { setName(e.target.value); setError(""); setSuccess(false); }} fullWidth sx={inputSx} />
      </Box>

      {/* Photo */}
      <Box sx={{ mb: "15px" }}>
        <FieldLabel>Photo</FieldLabel>
        {preview ? (
          <Box sx={{ position: "relative" }}>
            <Box component="img" src={preview} alt="preview"
              sx={{ width: "100%", height: "200px", objectFit: "cover", objectPosition: "top", borderRadius: "10px", border: `1px solid ${ACCENT}`, display: "block" }} />
            <Box component="button" onClick={() => inputRef.current?.click()}
              sx={{ position: "absolute", bottom: "10px", right: "10px", background: "oklch(0.155 0.006 248 / 0.88)", border: "1px solid oklch(0.395 0.012 248)", borderRadius: "8px", color: "oklch(0.960 0.004 248)", fontFamily: displayFont, fontSize: "12px", fontWeight: 500, padding: "7px 12px", cursor: "pointer", backdropFilter: "blur(6px)", "&:hover": { borderColor: ACCENT } }}>
              Change photo
            </Box>
          </Box>
        ) : (
          <Box
            onDrop={(e) => { e.preventDefault(); handleFile(e.dataTransfer.files[0]); }}
            onDragOver={(e) => e.preventDefault()}
            onClick={() => inputRef.current?.click()}
            sx={{ height: "140px", borderRadius: "10px", border: "2px dashed oklch(0.305 0.010 248)", display: "flex", flexDirection: "column", alignItems: "center", justifyContent: "center", gap: "10px", cursor: "pointer", transition: "border-color .15s ease, background .15s ease", "&:hover": { borderColor: ACCENT, background: "oklch(0.840 0.060 214 / 0.04)" } }}
          >
            <Box sx={{ width: "38px", height: "38px", borderRadius: "50%", background: "oklch(0.205 0.007 248)", border: "1px solid oklch(0.305 0.010 248)", display: "grid", placeItems: "center", color: "oklch(0.530 0.009 248)" }}>
              <svg width="18" height="18" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="1.8"><path d="M21 15v4a2 2 0 0 1-2 2H5a2 2 0 0 1-2-2v-4"/><polyline points="17 8 12 3 7 8"/><line x1="12" y1="3" x2="12" y2="15"/></svg>
            </Box>
            <Box sx={{ textAlign: "center" }}>
              <Box sx={{ fontFamily: displayFont, fontSize: "13px", color: "oklch(0.700 0.008 248)" }}>Click to upload or drag & drop</Box>
              <Box sx={{ fontFamily: monoFont, fontSize: "10px", color: "oklch(0.530 0.009 248)", mt: "3px" }}>JPG, PNG, WEBP</Box>
            </Box>
          </Box>
        )}
        <input ref={inputRef} type="file" accept="image/*" style={{ display: "none" }} onChange={(e) => handleFile(e.target.files[0])} />
      </Box>

      {/* Error */}
      {error && <Typography sx={{ color: "#ff6b6b", fontFamily: monoFont, fontSize: "12px", mb: "12px" }}>{error}</Typography>}

      {/* Success */}
      {success && (
        <Box sx={{ display: "flex", alignItems: "center", gap: "9px", background: "oklch(0.840 0.060 214 / 0.10)", border: `1px solid oklch(0.840 0.060 214 / 0.35)`, borderRadius: "9px", padding: "11px 13px", mb: "12px" }}>
          <svg width="14" height="14" viewBox="0 0 24 24" fill="none" stroke={ACCENT} strokeWidth="2"><polyline points="20 6 9 17 4 12"/></svg>
          <Box sx={{ fontFamily: displayFont, fontSize: "13px", color: ACCENT }}>Person registered successfully.</Box>
        </Box>
      )}

      <Box sx={{ display: "flex", justifyContent: "flex-end", gap: "10px" }}>
        <BtnGhost onClick={handleClose}>Cancel</BtnGhost>
        <BtnPrimary onClick={handleSubmit} disabled={loading}>{loading ? <CircularProgress size={14} color="inherit" /> : "Add person"}</BtnPrimary>
      </Box>
    </Modal>
  );
}

/* ══════════════════════════════════════════
   Main Navbar
══════════════════════════════════════════ */
export default function Navbar() {
  const navigate  = useNavigate();
  const location  = useLocation();
  const menuRef   = useRef(null);

  const [menuOpen,     setMenuOpen]     = useState(false);
  const [passwordOpen, setPasswordOpen] = useState(false);
  const [adminsOpen,   setAdminsOpen]   = useState(false);
  const [personOpen,   setPersonOpen]   = useState(false);
  const [initials,     setInitials]     = useState("AD");
  const [isAdmin,      setIsAdmin]      = useState(false);

  /* fetch name for avatar initials */
  useEffect(() => {
    getAccountName()
      .then((data) => {
        const name = data?.accountName ?? data;
        if (name) setInitials(name.trim().split(" ").map((p) => p[0]).join("").slice(0, 2).toUpperCase());
      })
      .catch(() => {});
  }, []);

  /* fetch role — redirect to login if token invalid, set isAdmin flag */
  useEffect(() => {
    getRole()
      .then((data) => {
        const role = data?.role ?? data;
        setIsAdmin(role === "Admin" || role === 0);
      })
      .catch(() => { removeToken(); navigate("/login"); });
  }, []);

  /* close menu on outside click */
  useEffect(() => {
    const handler = (e) => { if (menuRef.current && !menuRef.current.contains(e.target)) setMenuOpen(false); };
    if (menuOpen) document.addEventListener("mousedown", handler);
    return () => document.removeEventListener("mousedown", handler);
  }, [menuOpen]);

  const handleLogout = () => { removeToken(); navigate("/login"); };
  const openModal = (modal) => {
    setMenuOpen(false);
    if (modal === "password") setPasswordOpen(true);
    if (modal === "admins")   setAdminsOpen(true);
    if (modal === "person")   setPersonOpen(true);
  };

  const menuItemSx = { width: "100%", display: "flex", alignItems: "center", gap: "11px", background: "none", border: "none", cursor: "pointer", textAlign: "left", color: "oklch(0.700 0.008 248)", fontFamily: displayFont, fontSize: "13.5px", padding: "10px", borderRadius: "8px", transition: "background .14s ease, color .14s ease", "&:hover": { background: "oklch(0.225 0.008 248)", color: "oklch(0.960 0.004 248)" } };

  return (
    <>
      <Box component="nav" sx={{ position: "sticky", top: 0, zIndex: 50, display: "flex", alignItems: "center", gap: "28px", height: "64px", padding: "0 28px", background: "oklch(0.155 0.006 248 / 0.85)", backdropFilter: "blur(12px)", borderBottom: "1px solid oklch(0.305 0.010 248)", fontFamily: displayFont }}>

        {/* Brand */}
        <Box component="a" href="#" onClick={(e) => { e.preventDefault(); navigate("/rooms"); }} sx={{ display: "flex", alignItems: "center", gap: "11px", textDecoration: "none", flexShrink: 0 }}>
          <Box sx={{ position: "relative", width: "26px", height: "26px", flexShrink: 0 }}>
            {[{ top: 0, left: 0, borderRight: "none", borderBottom: "none" }, { top: 0, right: 0, borderLeft: "none", borderBottom: "none" }, { bottom: 0, left: 0, borderRight: "none", borderTop: "none" }, { bottom: 0, right: 0, borderLeft: "none", borderTop: "none" }].map((pos, i) => (
              <Box key={i} sx={{ position: "absolute", width: "8px", height: "8px", border: `1.5px solid ${ACCENT}`, ...pos }} />
            ))}
            <Box sx={{ position: "absolute", inset: 0, margin: "auto", width: "6px", height: "6px", borderRadius: "50%", background: ACCENT, boxShadow: `0 0 9px ${ACCENT}` }} />
          </Box>
          <Box sx={{ fontWeight: 600, fontSize: "17px", letterSpacing: "-0.01em", color: "oklch(0.960 0.004 248)" }}>
            Room<Box component="span" sx={{ color: "oklch(0.700 0.008 248)", fontWeight: 400 }}>Vision</Box>
          </Box>
        </Box>

        {/* Nav links */}
        <Box sx={{ display: "flex", alignItems: "center", gap: "4px" }}>
          <Box component="a" href="#" onClick={(e) => { e.preventDefault(); navigate("/rooms"); }}
            sx={{ fontSize: "14px", color: location.pathname === "/rooms" ? "oklch(0.960 0.004 248)" : "oklch(0.700 0.008 248)", background: location.pathname === "/rooms" ? "oklch(0.225 0.008 248)" : "transparent", padding: "8px 13px", borderRadius: "8px", textDecoration: "none", transition: "color .15s ease, background .15s ease", "&:hover": { color: "oklch(0.960 0.004 248)", background: "oklch(0.195 0.007 248)" } }}>
            Rooms
          </Box>
        </Box>

        {/* Right — avatar + dropdown */}
        <Box ref={menuRef} sx={{ marginLeft: "auto", position: "relative" }}>
          <Box component="button" onClick={() => setMenuOpen((v) => !v)}
            sx={{ width: "36px", height: "36px", borderRadius: "50%", background: "linear-gradient(135deg, oklch(0.225 0.008 248), oklch(0.195 0.007 248))", border: "1px solid oklch(0.395 0.012 248)", display: "grid", placeItems: "center", fontFamily: monoFont, fontSize: "12px", fontWeight: 600, color: "oklch(0.960 0.004 248)", cursor: "pointer" }}>
            {initials}
          </Box>

          {menuOpen && (
            <Box sx={{ position: "absolute", top: "calc(100% + 10px)", right: 0, width: "220px", background: "oklch(0.195 0.007 248)", border: "1px solid oklch(0.395 0.012 248)", borderRadius: "12px", padding: "6px", boxShadow: "0 18px 48px oklch(0 0 0 / 0.5)", zIndex: 60 }}>

              {/* Change password — visible to everyone */}
              <Box component="button" onClick={() => openModal("password")} sx={menuItemSx}>
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><rect x="4" y="11" width="16" height="9" rx="2"/><path d="M8 11V8a4 4 0 0 1 8 0v3"/></svg>
                Change password
              </Box>

              {/* Add person — visible to everyone */}
              <Box component="button" onClick={() => openModal("person")} sx={menuItemSx}>
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M16 19v-1a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v1"/><circle cx="9" cy="7" r="3.2"/><path d="M19 8v5M21.5 10.5h-5"/></svg>
                Add person
              </Box>

              {/* Administrators — Admin only */}
              {isAdmin && (
                <Box component="button" onClick={() => openModal("admins")} sx={menuItemSx}>
                  <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M16 19v-1a4 4 0 0 0-4-4H6a4 4 0 0 0-4 4v1"/><circle cx="9" cy="7" r="3.2"/><path d="M19 8v5M21.5 10.5h-5"/></svg>
                  Administrators
                </Box>
              )}

              <Box sx={{ height: "1px", background: "oklch(0.305 0.010 248)", margin: "6px 4px" }} />

              <Box component="button" onClick={handleLogout} sx={menuItemSx}>
                <svg width="16" height="16" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2"><path d="M15 17l5-5-5-5"/><path d="M20 12H9"/><path d="M9 20H6a2 2 0 0 1-2-2V6a2 2 0 0 1 2-2h3"/></svg>
                Sign out
              </Box>
            </Box>
          )}
        </Box>
      </Box>

      <ChangePasswordModal open={passwordOpen} onClose={() => setPasswordOpen(false)} />
      <AddPersonModal open={personOpen} onClose={() => setPersonOpen(false)} />
      {isAdmin && <AdminsModal open={adminsOpen} onClose={() => setAdminsOpen(false)} />}
    </>
  );
}
