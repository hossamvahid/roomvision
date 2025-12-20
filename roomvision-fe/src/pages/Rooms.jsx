import {
  Box,
  Container,
  Typography,
  Button,
  Grid,
  Paper,
  CircularProgress,
} from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Title, RoomsGrid } from "../components/rooms";
import { useState, useEffect } from "react";
import getRooms from "../services/room/getRooms";

export default function Rooms() {
  const theme = useTheme();
  const [rooms, setRooms] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);

  const fetchRoomsByPage = async (pageNumber) => {
    setLoading(true);
    try {
      const response = await getRooms(currentPage);
      setRooms(response.items);
      setTotalPages(response.totalPages);
    } catch (error) {
      console.error("Error fetching rooms:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchRoomsByPage(currentPage);
  }, [currentPage]);

  return (
    <Box
      sx={{
        backgroundColor: theme.palette.background.default,
        minHeight: "100vh",
        py: 4,
      }}
    >
      <Container maxWidth="lg">
        <Title />

        <RoomsGrid
          rooms={rooms}
          currentPage={currentPage}
          totalPages={totalPages}
          loading={loading}
          setCurrentPage={setCurrentPage}
        />
      </Container>
    </Box>
  );
}
