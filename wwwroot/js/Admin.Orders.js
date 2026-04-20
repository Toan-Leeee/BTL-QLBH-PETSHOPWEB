document.addEventListener('DOMContentLoaded', () => {
  const selectedRow = document.querySelector('.admin-table .row-selected');
  const formPanel = document.querySelector('[data-order-form-panel]');
  const detailsPanel = document.querySelector('[data-order-details-panel]');
  const statusSelect = formPanel?.querySelector('select[name="status"]');

  if (selectedRow) {
    selectedRow.scrollIntoView({ block: 'center', behavior: 'smooth' });
  }

  if (formPanel && selectedRow) {
    formPanel.classList.add('is-highlighted');
  }

  if (detailsPanel) {
    detailsPanel.classList.add('is-highlighted');
  }

  statusSelect?.focus();
});
