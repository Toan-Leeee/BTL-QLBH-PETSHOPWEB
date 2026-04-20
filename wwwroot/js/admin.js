document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('form[data-confirm]').forEach((form) => {
    form.addEventListener('submit', (event) => {
      const message = form.getAttribute('data-confirm') || 'Ban co chac muon thuc hien thao tac nay?';
      if (!confirm(message)) {
        event.preventDefault();
      }
    });
  });

  document.querySelectorAll('.admin-table .row-selected').forEach((row) => {
    row.setAttribute('tabindex', '-1');
  });
});
