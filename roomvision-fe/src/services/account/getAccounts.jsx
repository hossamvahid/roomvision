import api from "../api";
import getToken from "../../utils/storage/getToken";
export default async function getAccounts(page, pageSize = 6) {
  try {
    const token = getToken();

    const response = await api.get(
      `/account/list?page=${page}&pageSize=${pageSize}`,
      {
        headers: { Authorization: `Bearer ${token}` },
      },
    );

    return response.data;
  } catch (error) {
    throw error.response?.data || error.message;
  }
}
