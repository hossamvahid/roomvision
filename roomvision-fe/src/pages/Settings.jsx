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
import { Title } from "../components/settings";
import { useState, useEffect } from "react";
import getAccounts from "../services/account/getAccounts";
import AccountsGrid from "../components/settings/AccountsGrid";
import { getAccountName } from "../services";

export default function Settings() {
  const theme = useTheme();
  const [accounts, setAccounts] = useState([]);
  const [currentPage, setCurrentPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(false);

  const fetchAccountsByPage = async (pageNumber) => {
    setLoading(true);
    try {
      const response = await getAccounts(currentPage);
      setAccounts(response.items);
      setTotalPages(response.totalPages);
    } catch (error) {
      console.error("Error fetching rooms:", error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAccountsByPage(currentPage);
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

        <AccountsGrid
          accounts={accounts}
          currentPage={currentPage}
          totalPages={totalPages}
          loading={loading}
          setCurrentPage={setCurrentPage}
        />
      </Container>
    </Box>
  );
}
