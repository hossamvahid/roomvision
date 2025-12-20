export default function handlePreviousPage({ setCurrentPage }) {
  setCurrentPage((prev) => (prev > 1 ? prev - 1 : prev));
}
