export default function handleNextPage({ setCurrentPage, totalPages }) {
  setCurrentPage((prev) => (prev < totalPages ? prev + 1 : prev));
}
