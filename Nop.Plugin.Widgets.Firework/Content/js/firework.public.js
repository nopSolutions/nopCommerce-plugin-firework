var FW = {
  initialized: false,
  cartUrl: '',
  currency: '',
  init: function (cartUrl, currency) {
    if (this.initialized)
      return;

    this.cartUrl = cartUrl;
    this.currency = currency;

    if (window._fwn?.shopping) {
      window._fwn.shopping.configureCart({ url: this.cartUrl, currency: this.currency });
      window._fwn.shopping.onCartDisplayed(async (fwComponent) => {
        FW.initialized = true;
        // 1. Make a request to get the latest cart.
        const remoteCart = await FW.fetchCartFromServerOrCache();

        // 2. Return cart items.
        return remoteCart.map((remoteCartItem) => {
          return FW.mapShoppingCartItem(remoteCartItem);
        });
      });
      window._fwn.shopping.onCartUpdated(async ({ product, productUnit, quantity, previousQuantity }) => {
        // Make a request to update the remote cart.
        const result = await FW.updateVariant(product, productUnit, quantity, previousQuantity);

        return result.quantity;
      });
      window._fwn.shopping.onProductsLoaded(async ({ products }) => {
        const productExtIds = products.map((product) => product.product_ext_id);
        // Make a server request to get the latest product data.
        const remoteProducts = await FW.fetchProductsFromServer(productExtIds);
        return remoteProducts.map((remoteProduct) => FW.parseProduct(remoteProduct));
      });
      // if window._fwn?.shopping is loaded before this script tag, use it directly
    } else {
      document.addEventListener('fw:ready', () => {
        FW.init(cartUrl, currency);
      })
    }
  },
  /**
   * Fetches the shopping cart data from the server
   * @returns {Promise<Array<any>>}
   * */
  fetchCartFromServerOrCache: function () {
    return new Promise((resolve, reject) => {
      $.ajax({
        cache: false,
        type: 'GET',
        url: '/fireworkpublic/shoppingcart',
        error: function (jqXHR, textStatus, errorThrown) {
          reject(errorThrown);
        },
        success: function (data) {
          resolve(data);
        }
      });
    });
  },
  /**
   * Maps a shopping cart item
   * @param {any} cartItem
   * @returns {{unitId: string, quantity: number, product: any}}
   */
  mapShoppingCartItem: function (cartItem) {
    return { product: FW.parseProduct(cartItem.product), unitId: cartItem.unitId, quantity: cartItem.quantity }
  },
  parseProduct: function (remoteProduct) {
    return window._fwn.shopping.productFactory((builder) => {
      builder
        .description(remoteProduct.product_description)
        .extId(remoteProduct.product_ext_id)
        .name(remoteProduct.product_name)
        .currency(FW.currency)

      remoteProduct.product_units.forEach((remoteVariant) => {
        builder.variant((variantBuilder) => {
          variantBuilder
            .extId(remoteVariant.unit_ext_id)
            .isAvailable(remoteVariant.unit_quantity > 0 || remoteVariant.unit_downloadable)
            .name(remoteVariant.unit_name)
            .price(remoteVariant.unit_price)
            .url(remoteVariant.unit_url)
            .image((imageBuilder) => {
              var picture = remoteProduct.product_images.find((image) => image.unit_identifiers.includes(remoteVariant.unit_ext_id));
              if (!picture)
                picture = remoteProduct.product_images?.[0]

              if (picture) {
                imageBuilder
                  .extId(picture.image_ext_id)
                  .position(picture.image_position)
                  .title(picture.image_alt)
                  .url(picture.image_src)
              }
            });

          remoteVariant.unit_options.forEach(({ name, value }) => {
            variantBuilder.option({
              name,
              value
            });
          })
        })
      })
    }, true)
  },
  updateVariant: function (product, productUnit, quantity, previousQuantity) {
    return new Promise((resolve, reject) => {
      var postData = {
        ProductExtId: product.product_ext_id,
        UnitExtId: productUnit.unit_ext_id,
        Quantity: quantity,
        PreviousQuantity: previousQuantity
      };
      addAntiForgeryToken(postData);
      $.ajax({
        cache: false,
        type: 'POST',
        url: '/fireworkpublic/shoppingcart',
        data: postData,
        error: function (jqXHR, textStatus, errorThrown) {
          reject(errorThrown);
          AjaxCart.ajaxFailure();
        },
        success: function (data) {
          AjaxCart.success_process(data)
          resolve(data);
        },
        complete: AjaxCart.resetLoadWaiting
      });
    });
  },
  /**
   * Fetches the product data from the server
   * @param {string[]} productExtIds
   * @returns {Promise<any[]>}
   */
  fetchProductsFromServer: function (productExtIds) {
    return new Promise((resolve, reject) => {
      $.ajax({
        cache: false,
        type: 'GET',
        url: '/fireworkpublic/products?' + productExtIds.map(id => `productExtIds=${id}`).join('&'),
        error: function (jqXHR, textStatus, errorThrown) {
          reject(errorThrown);
        },
        success: function (data) {
          resolve(data);
        }
      });
    });
  },
}