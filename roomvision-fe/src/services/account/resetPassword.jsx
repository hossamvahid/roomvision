import api from "../api";
import getToken from "../../utils/storage/getToken";
export default async function resetPassword(newPassword) {
  try {
    const token = getToken();
    const response = await api.patch(
      `/account/reset-password`,
      { newPassword },
      { headers: { Authorization: `Bearer ${token}` } },
    );
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
