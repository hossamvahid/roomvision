import api from "../api";
import getToken from "../../utils/storage/getToken";
export default async function getAccountName() {
  try {
    const token = getToken();

    const response = await api.get(`/account/name`, {
      headers: { Authorization: `Bearer ${token}` },
    });

    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
