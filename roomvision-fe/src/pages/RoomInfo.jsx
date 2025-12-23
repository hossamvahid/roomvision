import { Box, Container } from "@mui/material";
import { useTheme } from "@mui/material/styles";
import { Title, PersonsGrid } from "../components/roomInfo";
import { useState, useEffect } from "react";
import { filterAndPaginate } from "../utils/pagination";
import getRoomInfo from "../services/room/getRoomInfo";
import { useSearchParams, useNavigate } from "react-router-dom";

export default function RoomInfo() {
  const theme = useTheme();
  const [data, setData] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);
  const navigate = useNavigate();
  const [searchParams] = useSearchParams();
  const pageSize = 9;
  const allFaces = data.identifiedFaces || [];
  const [searchTerm, setSearchTerm] = useState("");
  const handleSetSearchTerm = (v) => {
    setSearchTerm(v);
    setCurrentPage(1);
  };
  const roomName = searchParams.get("room");

  const fetchRoomsByPage = async (pageNumber) => {
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
    if (!roomName || roomName.trim() === "") {
      navigate("/rooms", { replace: true });
    }
  }, [roomName, navigate]);

  useEffect(() => {
    fetchRoomsByPage(currentPage);
  }, [currentPage]);

  const { pagedItems: pagedFaces, totalPages: derivedTotal } =
    filterAndPaginate(allFaces, searchTerm, currentPage, pageSize);

  useEffect(() => {
    setTotalPages(derivedTotal);
    if (derivedTotal > 0 && currentPage > derivedTotal)
      setCurrentPage(derivedTotal);
    if (derivedTotal === 0 && currentPage !== 1) setCurrentPage(1);
  }, [derivedTotal]);

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
          roomName={"A101"}
          totalFaces={data.totalFaces || 0}
          totalUnknown={data.totalUnknown || 0}
          scannedAt={data.scannedAt}
          searchTerm={searchTerm}
          setSearchTerm={handleSetSearchTerm}
        />
        <PersonsGrid
          faces={pagedFaces}
          currentPage={currentPage}
          totalPages={totalPages}
          loading={loading}
          setCurrentPage={setCurrentPage}
        />
      </Container>
    </Box>
  );
}
