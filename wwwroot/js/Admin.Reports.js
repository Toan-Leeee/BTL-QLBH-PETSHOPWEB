document.addEventListener('DOMContentLoaded', () => {
  const stats = Array.from(document.querySelectorAll('[data-stat-value]'));

  stats.forEach((stat) => {
    const target = Number.parseFloat(stat.dataset.statValue || '0');

    if (!Number.isFinite(target)) {
      return;
    }

    const duration = 700;
    const start = performance.now();

    const step = (now) => {
      const progress = Math.min((now - start) / duration, 1);
      const value = Math.round(target * progress);
      stat.textContent = value.toLocaleString('vi-VN');

      if (progress < 1) {
        requestAnimationFrame(step);
      }
    };

    requestAnimationFrame(step);
  });
});
