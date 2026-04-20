document.addEventListener('DOMContentLoaded', () => {
  const selectedRow = document.querySelector('.admin-table .row-selected');
  const form = document.querySelector('[data-admin-form="customers"]');

  if (selectedRow) {
    selectedRow.scrollIntoView({ block: 'center', behavior: 'smooth' });
  }

  if (!form) {
    return;
  }

  const codeInput = form.querySelector('input[name="MaKhachHang"]');
  const nameInput = form.querySelector('input[name="TenKhachHang"]');
  const emailInput = form.querySelector('input[name="Email"]');

  if (codeInput?.value.trim()) {
    nameInput?.focus();
    return;
  }

  (nameInput || emailInput || codeInput)?.focus();
});
