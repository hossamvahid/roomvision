import { Box, Container } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Title, PersonsGrid } from "../components/roomInfo";
import { useState, useEffect } from "react";
import getRoomInfo from "../services/room/getRoomInfo";
import { useSearchParams, useNavigate } from "react-router-dom";

export default function RoomInfo() {
  const theme = useTheme();
  const [data, setData] = useState([]);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const roomName = searchParams.get("room");

  const fetchRoomsByPage = async () => {
    setLoading(true);
    try {
      const response = await getRoomInfo(roomName);

      if (!response) {
        throw new Error("Room not found");
      }
      setData(response);
    } catch (error) {
      console.error("Error fetching rooms:", error);
      navigate("/rooms", { replace: true });
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoomsByPage();
  }, []);

  return (
    <Box
      sx={{
        backgroundColor: theme.palette.background.default,
        minHeight: "100vh",
        py: 4,
      }}
    >
      <Container maxWidth="lg">
        <Title
          roomName={roomName}
          totalFaces={data.totalFaces || 0}
          totalUnknown={data.totalUnknown || 0}
          scannedAt={data.scannedAt}
        />
        <PersonsGrid faces={data.identifiedFaces || []} loading={loading} />
      </Container>
    </Box>
  );
}
