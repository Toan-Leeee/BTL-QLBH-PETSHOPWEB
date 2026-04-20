document.addEventListener('DOMContentLoaded', () => {
  const form = document.querySelector('[data-import-form]');
  const detailsPanel = document.querySelector('[data-import-details]');
  const rows = Array.from(document.querySelectorAll('[data-import-row]'));

  if (detailsPanel) {
    detailsPanel.scrollIntoView({ block: 'nearest', behavior: 'smooth' });
  }

  if (!form || rows.length === 0) {
    return;
  }

  const syncRow = (row) => {
    const product = row.querySelector('[data-import-product]');
    const quantity = row.querySelector('[data-import-quantity]');
    const price = row.querySelector('[data-import-price]');
    const hasProduct = Boolean(product?.value);

    quantity.disabled = !hasProduct;
    price.disabled = !hasProduct;
    row.classList.toggle('is-ready', hasProduct);

    if (!hasProduct) {
      quantity.value = '';
      price.value = '';
    }
  };

  rows.forEach((row, index) => {
    const product = row.querySelector('[data-import-product]');
    product?.addEventListener('change', () => syncRow(row));
    syncRow(row);

    if (index === 0) {
      product?.focus();
    }
  });
});
