import { Box } from "@mui/material";
import { Title, RoomsGrid } from "../components/rooms";
import { useState, useEffect } from "react";
import getRooms from "../services/room/getRooms";
import { getRole } from "../services";

export default function Rooms() {
  const [rooms, setRooms]             = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages]   = useState(1);
  const [loading, setLoading]         = useState(false);
  const [isAdmin, setIsAdmin]         = useState(false);

  const fetchRooms = async (pageNumber) => {
    setLoading(true);
    try {
      const response = await getRooms(pageNumber);
      setRooms(response.items);
      setTotalPages(response.totalPages);
    } catch (error) {
      console.error("Error fetching rooms:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    getRole()
      .then((data) => {
        const role = data?.role ?? data;
        setIsAdmin(role === "Admin" || role === 0);
      })
      .catch(() => {});
  }, []);

  useEffect(() => {
    fetchRooms(currentPage);
  }, [currentPage]);

  return (
    <Box sx={{ minHeight: "100vh", bgcolor: "oklch(0.135 0.006 248)", py: "32px", px: "28px" }}>
      <Box sx={{ maxWidth: "1320px", margin: "0 auto" }}>
        <Title isAdmin={isAdmin} onRoomCreated={() => fetchRooms(currentPage)} />
        <RoomsGrid
          rooms={rooms}
          currentPage={currentPage}
          totalPages={totalPages}
          loading={loading}
          setCurrentPage={setCurrentPage}
        />
      </Box>
    </Box>
  );
}
