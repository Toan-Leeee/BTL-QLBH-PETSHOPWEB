document.addEventListener('DOMContentLoaded', () => {
  const selectedRow = document.querySelector('.admin-table .row-selected');
  const recordCard = document.querySelector('[data-admin-record-card]');
  const form = document.querySelector('[data-admin-form="categories"]');
  const description = form?.querySelector('[data-auto-resize]');

  if (selectedRow) {
    selectedRow.scrollIntoView({ block: 'center', behavior: 'smooth' });
  }

  if (recordCard && selectedRow) {
    recordCard.classList.add('is-highlighted');
  }

  const resizeTextarea = () => {
    if (!description) {
      return;
    }

    description.style.height = 'auto';
    description.style.height = `${description.scrollHeight}px`;
  };

  description?.addEventListener('input', resizeTextarea);
  resizeTextarea();

  if (!form) {
    return;
  }

  const codeInput = form.querySelector('input[name="MaDanhMuc"]');
  const nameInput = form.querySelector('input[name="TenDanhMuc"]');
  (codeInput?.value.trim() ? nameInput : codeInput)?.focus();
});
