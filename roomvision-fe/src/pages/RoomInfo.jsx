import { Box } from "@mui/material";
import { Title, PersonsGrid } from "../components/roomInfo";
import { useState, useEffect } from "react";
import getRoomInfo from "../services/room/getRoomInfo";
import { useSearchParams, useNavigate } from "react-router-dom";

export default function RoomInfo() {
  const [data, setData] = useState({});
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const roomName = searchParams.get("room");

  useEffect(() => {
    setLoading(true);
    getRoomInfo(roomName)
      .then((response) => {
        if (!response) throw new Error("Room not found");
        setData(response);
      })
      .catch(() => navigate("/rooms", { replace: true }))
      .finally(() => setLoading(false));
  }, []);

  return (
    <Box sx={{ minHeight: "100vh", bgcolor: "oklch(0.135 0.006 248)", py: "32px", px: "28px" }}>
      <Box sx={{ maxWidth: "1320px", margin: "0 auto" }}>
        <Title
          roomName={roomName}
          totalFaces={data.totalFaces || 0}
          totalUnknown={data.totalUnknown || 0}
          scannedAt={data.scannedAt}
        />
        <PersonsGrid faces={data.identifiedFaces || []} loading={loading} />
      </Box>
    </Box>
  );
}
