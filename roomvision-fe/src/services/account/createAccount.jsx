import api from "../api";
import getToken from "../../utils/storage/getToken";
export default async function createAccount(email, name) {
  try {
    const token = getToken();
    const response = await api.post(
      `/account/create`,
      { email, name },
      { headers: { Authorization: `Bearer ${token}` } },
    );
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
