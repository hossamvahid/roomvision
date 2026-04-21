import { Box, Typography, TextField, Button } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Search } from "lucide-react";
import { useEffect, useState } from "react";
import { getAccountName } from "../../services";
import AccountDialog from "./AccountDialog";

export default function Title() {
  const theme = useTheme();
  const [name, setName] = useState();
  const [dialogMode, setDialogMode] = useState(null);

  const fetchAccountName = async () => {
    try {
      const response = await getAccountName();
      setName(response.accountName);
    } catch (error) {
      console.error("Error fetching rooms:", error);
    }
  };

  useEffect(() => {
    fetchAccountName();
  }, [name]);

  return (
    <>
      <Box
        sx={{
          display: "flex",
          flexDirection: "column",
          justifyContent: "space-between",
          alignItems: "start",
          gap: 5,
          mb: 4,
        }}
      >
        <Typography
          variant="h3"
          sx={{
            fontWeight: 600,
            color: theme.palette.secondary.main,
            pb: 1,
            display: "inline-block",
          }}
        >
          Welcome {name}
        </Typography>

        <Box
          sx={{
            display: "flex",
            flexDirection: { xs: "column", md: "row" },
            justifyContent: "space-between",
            alignItems: { xs: "flex-start", md: "center" },
            width: "100%",
            gap: 2,
            mb: 4,
          }}
        >
          <Typography
            variant="h4"
            sx={{
              fontWeight: 600,
              color: theme.palette.secondary.main,
              borderBottom: `3px solid ${theme.palette.secondary.main}`,
              pb: 1,
              display: "inline-block",
            }}
          >
            Accounts
          </Typography>

          <Box sx={{ display: "flex", gap: 2 }}>
            <Button
              variant="contained"
              onClick={() => setDialogMode("create")}
              sx={{
                backgroundColor: theme.palette.secondary.main,
                textTransform: "none",
                borderRadius: "8px",
                fontWeight: 600,
                "&:hover": { backgroundColor: theme.palette.primary.main },
              }}
            >
              Create Account
            </Button>
            <Button
              variant="outlined"
              onClick={() => setDialogMode("reset")}
              sx={{
                color: theme.palette.secondary.main,
                borderColor: theme.palette.secondary.main,
                textTransform: "none",
                borderRadius: "8px",
                fontWeight: 600,
                "&:hover": {
                  borderColor: theme.palette.primary.main,
                  color: theme.palette.primary.main,
                },
              }}
            >
              Reset Password
            </Button>
          </Box>

          <AccountDialog
            mode={dialogMode}
            open={dialogMode !== null}
            onClose={() => setDialogMode(null)}
            onSuccess={() => window.location.reload()}
          />
        </Box>
      </Box>
    </>
  );
}
