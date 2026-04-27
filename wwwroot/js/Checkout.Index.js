document.addEventListener('DOMContentLoaded', () => {
    const syncSelectableGroup = (selector, activeClass) => {
        const options = document.querySelectorAll(selector);
        options.forEach((option) => {
            const input = option.querySelector('input[type="radio"]');
            if (!input) {
                return;
            }

            const sync = () => {
                options.forEach((item) => item.classList.remove(activeClass));
                if (input.checked) {
                    option.classList.add(activeClass);
                }
            };

            option.addEventListener('click', sync);
            input.addEventListener('change', sync);
            sync();
        });
    };

    syncSelectableGroup('[data-delivery-card]', 'selected');
    syncSelectableGroup('[data-payment-option]', 'checked');
});
