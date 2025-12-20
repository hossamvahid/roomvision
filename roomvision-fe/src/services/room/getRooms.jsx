import api from "../api";
import getToken from "../../utils/storage/getToken";
export default async function getRooms(page, pageSize = 6) {
  try {
    console.log(page);
    const token = getToken();

    const response = await api.get(
      `/room/get?page=${page}&pageSize=${pageSize}`,
      {
        headers: { Authorization: `Bearer ${token}` },
      }
    );

    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
