document.addEventListener('DOMContentLoaded', () => {
  const imageInput = document.querySelector('[data-product-image-input]');
  const imagePreview = document.querySelector('[data-product-image-preview]');
  const fallbackImage = '/images/products/image 22.jpg';

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
});
