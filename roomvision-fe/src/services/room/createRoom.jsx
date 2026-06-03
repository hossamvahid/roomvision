import api from "../api";
import getToken from "../../utils/storage/getToken";

export default async function createRoom(roomName, password) {
  try {
    const token = getToken();
    const response = await api.post(
      `/room/create`,
      { roomName, password },
      { headers: { Authorization: `Bearer ${token}` } },
    );
    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
