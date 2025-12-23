import api from "../api";
import getToken from "../../utils/storage/getToken";
export default async function getRoomInfo(roomName) {
  try {
    const token = getToken();

    const response = await api.get(`/room/scan/result?room=${roomName}`, {
      headers: { Authorization: `Bearer ${token}` },
    });

    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
