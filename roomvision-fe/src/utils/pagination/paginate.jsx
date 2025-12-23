export default function paginate(items = [], page = 1, size = 10) {
  const all = Array.isArray(items) ? items : [];
  const totalItems = all.length;
  const pageSize = Math.max(0, Number(size) || 0);

  if (pageSize <= 0) {
    return {
      pagedItems: all,
      totalPages: totalItems > 0 ? 1 : 0,
      totalItems,
      currentPage: 1,
      pageSize,
    };
  }

  const totalPages = Math.max(1, Math.ceil(totalItems / pageSize));
  const currentPage = Math.min(Math.max(1, Number(page) || 1), totalPages);
  const start = (currentPage - 1) * pageSize;
  const end = start + pageSize;
  const pagedItems = all.slice(start, end);

  return { pagedItems, totalPages, totalItems, currentPage, pageSize };
}

export { paginate };
