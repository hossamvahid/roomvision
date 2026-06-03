import api from "../api";
import getToken from "../../utils/storage/getToken";

export default async function deleteAccount(id) {
  try {
    const token = getToken();
    const response = await api.delete(`/account/${id}`, {
      headers: { Authorization: `Bearer ${token}` },
    });
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
