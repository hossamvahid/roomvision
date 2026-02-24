import { Box, Typography, Grid, Paper, CircularProgress } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Users, Calendar } from "lucide-react";
import { formatDate } from "../../utils/date";

export default function PersonsGrid({ faces, loading }) {
  const theme = useTheme();

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
            {faces.map((face) => (
              <Box key={face}>
                <Paper
                  sx={{
                    p: 2,
                    minHeight: 60,
                    display: "flex",
                    justifyContent: "center",
                    alignItems: "center",
                    borderRadius: "8px",
                    boxShadow: "0 2px 8px rgba(0, 0, 0, 0.1)",
                    backgroundColor:
                      face === "Unknown" ? theme.palette.error.main : "white",
                  }}
                >
                  <Typography
                    variant="h5"
                    sx={{
                      fontWeight: 600,
                      color:
                        face === "Unknown"
                          ? "white"
                          : theme.palette.secondary.main,
                    }}
                  >
                    {face}
                  </Typography>
                </Paper>
              </Box>
            ))}
          </Grid>
        </>
      )}
    </>
  );
}
