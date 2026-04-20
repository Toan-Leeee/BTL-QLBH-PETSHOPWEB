document.addEventListener('DOMContentLoaded', () => {
  const selectedRow = document.querySelector('.admin-table .row-selected');
  const profileCard = document.querySelector('[data-admin-profile-card]');
  const form = document.querySelector('[data-admin-form="accounts"]');

  if (selectedRow) {
    selectedRow.scrollIntoView({ block: 'center', behavior: 'smooth' });
  }

  if (profileCard && selectedRow) {
    profileCard.classList.add('is-highlighted');
  }

  if (!form) {
    return;
  }

  const codeInput = form.querySelector('input[name="MaNhanVien"]');
  const emptyField = Array.from(form.querySelectorAll('input'))
    .find((input) => input.type !== 'hidden' && !input.value.trim());

  const focusTarget = codeInput && codeInput.value.trim()
    ? form.querySelector('input[name="HoTen"]')
    : (emptyField || codeInput);

  focusTarget?.focus();
});
