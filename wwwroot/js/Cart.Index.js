document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-cart-quantity-form]').forEach((form) => {
        const input = form.querySelector('[data-quantity-input]');
        const picker = form.querySelector('.quantity-picker');
        const stockHint = form.querySelector('[data-stock-hint]');
        if (!input) {
            return;
        }

        const min = Number(input.min || 1);
        const max = Number(input.max || 9999);

        form.querySelectorAll('[data-quantity-change]').forEach((button) => {
            button.addEventListener('click', () => {
                const step = Number(button.getAttribute('data-quantity-change') || '0');
                const current = Number(input.value || min);
                const next = Math.max(min, Math.min(max, current + step));

                if (next === current) {
                    if (picker) {
                        picker.classList.add('flash-limit');
                        window.setTimeout(() => picker.classList.remove('flash-limit'), 420);
                    }

                    if (stockHint && current >= max && step > 0) {
                        stockHint.classList.add('limit-reached');
                        stockHint.textContent = `Chỉ còn ${max} sản phẩm trong kho`;
                    }

                    return;
                }

                if (stockHint) {
                    stockHint.classList.remove('limit-reached');
                    stockHint.textContent = `Còn ${max} sản phẩm`;
                }

                input.value = String(next);
                form.requestSubmit();
            });
        });
    });
});
