import { getToken } from "../../utils/storage";
import api from "../api";
export default async function authenticate(email, password) {
  try {
    const token = getToken();

    const response = await api.get("/authentication/role", {
      headers: { Authorization: `Bearer ${token}` },
    });

    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
