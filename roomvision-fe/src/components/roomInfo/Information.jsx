import { Box, Typography } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Calendar, UserRoundX, Users } from "lucide-react";
import { formatDate } from "../../utils/date";

export default function Information({ totalFaces, totalUnknown, scannedAt }) {
  const theme = useTheme();

  return (
    <Box
      sx={{
        display: "flex",
        alignItems: "center",
        gap: 4,
        flexWrap: "wrap",
      }}
    >
      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <Users size={35} color={theme.palette.secondary.main} />
        <Typography sx={{ color: theme.palette.secondary.main, fontSize: 25 }}>
          {totalFaces}
        </Typography>
      </Box>

      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <UserRoundX size={30} color={theme.palette.secondary.main} />
        <Typography sx={{ color: theme.palette.secondary.main, fontSize: 25 }}>
          {totalUnknown}
        </Typography>
      </Box>

      <Box sx={{ display: "flex", alignItems: "center", gap: 1 }}>
        <Calendar size={30} color={theme.palette.secondary.main} />
        <Typography sx={{ color: theme.palette.secondary.main, fontSize: 25 }}>
          {formatDate(scannedAt)}
        </Typography>
      </Box>
    </Box>
  );
}
