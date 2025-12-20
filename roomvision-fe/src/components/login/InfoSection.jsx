import { Box, Typography } from "@mui/material";
import { useTheme } from "@mui/material/styles";

export default function InfoSection() {
  const theme = useTheme();

  return (
    <>
      <Box>
        <Typography
          variant="h3"
          sx={{
            fontWeight: 700,
            mb: 3,
            color: theme.palette.text.primary,
            fontSize: "3rem",
          }}
        >
          RoomVision
        </Typography>
        <Typography
          variant="body1"
          sx={{
            color: theme.palette.text.primary,
            lineHeight: 1.8,
            mb: 2,
            fontSize: "1.1rem",
          }}
        >
          is an advanced system designed for full-room facial recognition,
          capable of identifying and analyzing faces across an entire room in
          real time. It provides accurate detection, tracking, and insights to
          enhance security, access control, and smart environment management.
        </Typography>
        <Typography
          variant="body2"
          sx={{
            color: theme.palette.text.primary,
            fontWeight: 600,
            fontSize: "1.05rem",
          }}
        >
          Please enter your credentials in order to log in to the administrative
          dashboard.
        </Typography>
      </Box>
    </>
  );
}
