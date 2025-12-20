import { List, ListItem, ListItemText, Box } from "@mui/material";
import { useNavigate, useLocation } from "react-router-dom";

export default function Navbar() {
  const navigate = useNavigate();
  const location = useLocation();

  const menuItems = [
    { label: "Rooms", path: "/rooms" },
    { label: "Settings", path: "/settings" },
  ];

  return (
    <>
      <List sx={{ flex: 1 }}>
        {menuItems.map((item) => (
          <ListItem
            key={item.label}
            button
            onClick={() => navigate(item.path)}
            sx={{
              flexDirection: "column",
              alignItems: "center",
              borderRadius: "8px",
              mb: 1,
              cursor: "pointer",
              "&:hover": {
                bgcolor: "rgba(255,255,255,0.1)",
              },
            }}
          >
            <ListItemText
              primary={item.label}
              sx={{
                "& .MuiListItemText-primary": {
                  fontSize: "1.3rem",
                  textAlign: "center",
                  fontWeight: 500,
                },
              }}
            />

            {location.pathname === item.path && (
              <Box
                sx={{
                  width: "40%",
                  height: "3px",
                  bgcolor: "text.secondary",
                  borderRadius: "2px",
                  mt: "4px",
                }}
              />
            )}
          </ListItem>
        ))}
      </List>
    </>
  );
}
