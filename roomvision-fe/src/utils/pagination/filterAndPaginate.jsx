import paginate from "./paginate";
export default function filterAndPaginate(
  items = [],
  searchTerm = "",
  page = 1,
  size = 10
) {
  const all = Array.isArray(items) ? items : [];
  const q = (searchTerm || "").toLowerCase().trim();
  const filteredItems = q
    ? all.filter((s) => String(s).toLowerCase().includes(q))
    : all.slice();

  const { pagedItems, totalPages, totalItems, currentPage, pageSize } =
    paginate(filteredItems, page, size);

  return {
    pagedItems,
    filteredItems,
    totalPages,
    totalItems,
    currentPage,
    pageSize,
  };
}

export { filterAndPaginate };
