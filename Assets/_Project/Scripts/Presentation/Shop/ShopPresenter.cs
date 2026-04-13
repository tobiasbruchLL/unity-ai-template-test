using System;
using R3;
using VContainer.Unity;
using UnityEngine.UIElements;
using LL.Core.Shop;
using VContainer;

namespace LL.Presentation.Shop
{
    public class ShopPresenter : IStartable, IDisposable
    {
        private readonly ShopState           _state;
        private readonly ShopView            _view;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private Action _onFeatured;
        private Action _onCurrency;
        private Action _onBundles;
        private Action _onOffers;

        [Inject]
        public ShopPresenter(ShopState state, ShopView view)
        {
            _state = state;
            _view = view;
        }

        public void Start()
        {
            _onFeatured = () => _state.SelectedCategory.Value = ShopCategory.Featured;
            _onCurrency = () => _state.SelectedCategory.Value = ShopCategory.Currency;
            _onBundles  = () => _state.SelectedCategory.Value = ShopCategory.Bundles;
            _onOffers   = () => _state.SelectedCategory.Value = ShopCategory.Offers;

            _view.BtnFeatured.clicked += _onFeatured;
            _view.BtnCurrency.clicked += _onCurrency;
            _view.BtnBundles.clicked  += _onBundles;
            _view.BtnOffers.clicked   += _onOffers;

            // Fires immediately with ShopCategory.Featured, setting correct initial UI state
            _state.SelectedCategory.Subscribe(OnCategoryChanged).AddTo(_disposables);
        }

        private void OnCategoryChanged(ShopCategory category)
        {
            SetVisible(_view.PanelFeatured, category == ShopCategory.Featured);
            SetVisible(_view.PanelCurrency, category == ShopCategory.Currency);
            SetVisible(_view.PanelBundles,  category == ShopCategory.Bundles);
            SetVisible(_view.PanelOffers,   category == ShopCategory.Offers);

            SetActive(_view.BtnFeatured, category == ShopCategory.Featured);
            SetActive(_view.BtnCurrency, category == ShopCategory.Currency);
            SetActive(_view.BtnBundles,  category == ShopCategory.Bundles);
            SetActive(_view.BtnOffers,   category == ShopCategory.Offers);
        }

        private static void SetVisible(VisualElement el, bool on)
        {
            if (on) el.AddToClassList("visible");
            else    el.RemoveFromClassList("visible");
        }

        private static void SetActive(VisualElement el, bool on)
        {
            if (on) el.AddToClassList("active-cat");
            else    el.RemoveFromClassList("active-cat");
        }

        public void Dispose()
        {
            _view.BtnFeatured.clicked -= _onFeatured;
            _view.BtnCurrency.clicked -= _onCurrency;
            _view.BtnBundles.clicked  -= _onBundles;
            _view.BtnOffers.clicked   -= _onOffers;
            _disposables.Dispose();
        }
    }
}
