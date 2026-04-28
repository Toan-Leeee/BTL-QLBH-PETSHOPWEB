document.addEventListener('DOMContentLoaded', () => {
  const imageInput = document.querySelector('[data-product-image-input]');
  const imagePreview = document.querySelector('[data-product-image-preview]');
  const form = document.querySelector('[data-admin-form="products"]');
  const selectedRow = document.querySelector('.admin-table .row-selected');
  const fallbackImage = '/images/products/image 22.jpg';

  if (selectedRow) {
    selectedRow.scrollIntoView({ block: 'center', behavior: 'smooth' });
  }

  if (imageInput && imagePreview) {
    const syncPreview = () => {
      const nextSource = imageInput.value.trim() || fallbackImage;
      imagePreview.src = nextSource;
    };

    imageInput.addEventListener('input', syncPreview);
    imageInput.addEventListener('change', syncPreview);
    imagePreview.addEventListener('error', () => {
      imagePreview.src = fallbackImage;
    });
    syncPreview();
  }

  if (!form) {
    return;
  }

  const codeInput = form.querySelector('input[name="MaSanPham"]');
  const nameInput = form.querySelector('input[name="TenSanPham"]');
  (codeInput?.value.trim() ? nameInput : codeInput)?.focus();
});
