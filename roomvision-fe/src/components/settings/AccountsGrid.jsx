import {
  Box,
  Typography,
  Button,
  Grid,
  Paper,
  CircularProgress,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { User, Mail } from "lucide-react";
import { handleNextPage, handlePreviousPage } from "../../utils/pagination";
import { useNavigate } from "react-router-dom";

export default function AccountsGrid({
  accounts,
  currentPage,
  totalPages,
  loading,
  setCurrentPage,
}) {
  const theme = useTheme();
  const navigate = useNavigate();
  return (
    <>
      {loading ? (
        <Box sx={{ display: "flex", justifyContent: "center", py: 4 }}>
          <CircularProgress />
        </Box>
      ) : (
        <>
          <Grid
            container
            spacing={3}
            sx={{
              mb: 4,
              display: "grid",
              gridTemplateColumns: "repeat(3, 1fr)",
              gap: 3,
            }}
          >
            {accounts.map((account) => (
              <Box key={account.id}>
                <Paper
                  sx={{
                    p: 3,
                    borderRadius: "8px",
                    boxShadow: "0 2px 8px rgba(0, 0, 0, 0.1)",
                    cursor: "pointer",
                    transition: "box-shadow 0.3s ease",
                    "&:hover": {
                      boxShadow: "0 4px 16px rgba(0, 0, 0, 0.15)",
                    },
                  }}
                >
                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1.5,
                      mb: 2,
                    }}
                  >
                    <User size={24} color={theme.palette.secondary.main} />
                    <Typography
                      variant="h5"
                      sx={{
                        fontWeight: 600,
                        color: theme.palette.secondary.main,
                        lineHeight: 1,
                        mb: 0,
                      }}
                    >
                      {account.name}
                    </Typography>
                  </Box>

                  <Box
                    sx={{
                      display: "flex",
                      alignItems: "center",
                      gap: 1,
                      mb: 1.5,
                    }}
                  >
                    <Mail size={24} color={theme.palette.secondary.main} />
                    <Typography
                      sx={{
                        color: theme.palette.secondary.main,
                        fontWeight: 500,
                      }}
                    >
                      {account.email}
                    </Typography>
                  </Box>
                </Paper>
              </Box>
            ))}
          </Grid>

          {totalPages > 1 && (
            <Box
              sx={{
                display: "flex",
                justifyContent: "center",
                alignItems: "center",
                gap: 2,
              }}
            >
              <Button
                onClick={() => handlePreviousPage({ setCurrentPage })}
                disabled={currentPage === 1}
                sx={{
                  backgroundColor: theme.palette.secondary.main,
                  color: "white",
                  borderRadius: "20px",
                  px: 3,
                  textTransform: "none",
                  fontSize: "16px",
                  fontWeight: 500,
                  "&:hover": {
                    backgroundColor: theme.palette.primary.main,
                  },
                  "&:disabled": {
                    backgroundColor: "#ccc",
                    color: "#999",
                  },
                }}
              >
                Back
              </Button>

              <Typography
                sx={{
                  fontWeight: 600,
                  fontSize: "18px",
                  color: theme.palette.secondary.main,
                }}
              >
                {currentPage}/{totalPages}
              </Typography>

              <Button
                onClick={() =>
                  handleNextPage({ currentPage, totalPages, setCurrentPage })
                }
                disabled={currentPage === totalPages}
                sx={{
                  backgroundColor: theme.palette.secondary.main,
                  color: "white",
                  borderRadius: "20px",
                  px: 3,
                  textTransform: "none",
                  fontSize: "16px",
                  fontWeight: 500,
                  "&:hover": {
                    backgroundColor: theme.palette.primary.main,
                  },
                  "&:disabled": {
                    backgroundColor: "#ccc",
                    color: "#999",
                  },
                }}
              >
                Next
              </Button>
            </Box>
          )}
        </>
      )}
    </>
  );
}
