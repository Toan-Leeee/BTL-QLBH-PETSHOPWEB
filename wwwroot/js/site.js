document.addEventListener('DOMContentLoaded', () => {
  document.querySelectorAll('input[type="password"]').forEach((input) => {
    input.addEventListener('focus', () => input.parentElement?.classList.add('is-focused'));
    input.addEventListener('blur', () => input.parentElement?.classList.remove('is-focused'));
  });
});
