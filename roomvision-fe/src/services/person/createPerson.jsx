import api from "../api";

export default async function createPerson(name, file) {
  const form = new FormData();
  form.append("personName", name);
  form.append("file", file);
  const response = await api.post("/api/v1/person/create", form, {
    headers: { "Content-Type": "multipart/form-data" },
  });
  return response.data;
}
