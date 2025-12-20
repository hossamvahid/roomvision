import { Box, Typography, Button } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { LogOut } from "lucide-react";
import { NavigationMenu } from ".";
import { getToken, removeToken } from "../../utils/storage";
import { useEffect } from "react";
import { getRole } from "../../services";
import { useNavigate } from "react-router-dom";

export default function Navbar() {
  const theme = useTheme();
  const navigate = useNavigate();
  const handleLogout = () => {
    navigate("/login");
    removeToken();
  };

  useEffect(() => {
    const fetchRole = async () => {
      try {
        const role = await getRole();
        console.log(role);
      } catch (error) {
        removeToken();
        console.error(error.message);
      }
    };
    fetchRole();
  });

  return (
    <Box
      sx={{
        width: "250px",
        height: "100vh",
        bgcolor: theme.palette.primary.main,
        color: theme.palette.text.secondary,
        display: "flex",
        flexDirection: "column",
        padding: 3,
        position: "fixed",
        left: 0,
        top: 0,
        borderRadius: "0 30px 30px 0",
      }}
    >
      <Typography
        variant="h5"
        sx={{
          fontWeight: 700,
          mb: 4,
          textAlign: "center",
          fontSize: "1.5rem",
        }}
      >
        RoomVision
      </Typography>

      <NavigationMenu />

      <Button
        onClick={handleLogout}
        fullWidth
        sx={{
          bgcolor: "rgba(255,255,255,0.2)",
          color: theme.palette.text.secondary,
          py: 1.2,
          borderRadius: "8px",
          fontWeight: 600,
          display: "flex",
          alignItems: "center",
          gap: 1,
          "&:hover": {
            bgcolor: "rgba(255,255,255,0.3)",
          },
        }}
      >
        <LogOut size={20} />
        Logout
      </Button>
    </Box>
  );
}
