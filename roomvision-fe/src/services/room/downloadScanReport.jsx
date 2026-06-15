import api from "../api";
import getToken from "../../utils/storage/getToken";

export default async function downloadScanReport(roomName) {
  const token = getToken();

  const response = await api.get(`/room/scan/report?room=${roomName}`, {
    headers: { Authorization: `Bearer ${token}` },
    responseType: "blob",
  });

  const url = window.URL.createObjectURL(new Blob([response.data], { type: "application/pdf" }));
  const link = document.createElement("a");
  link.href = url;
  link.setAttribute("download", `raport_${roomName}_${new Date().toISOString().slice(0, 10)}.pdf`);
  document.body.appendChild(link);
  link.click();
  link.remove();
  window.URL.revokeObjectURL(url);
}
