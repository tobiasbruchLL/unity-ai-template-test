using System;
using R3;
using VContainer;
using VContainer.Unity;
using UnityEngine.UIElements;
using BreakInfinity;
using LL.Common;
using LL.Core.Inventory;
using LL.Core.Shop;

namespace LL.Presentation.Shop
{
    public class ShopPresenter : IStartable, IDisposable
    {
        private readonly ShopState           _state;
        private readonly ShopView            _view;
        private readonly Inventory           _inventory;
        private readonly IIAPService         _iapService;
        private readonly CompositeDisposable _disposables = new();

        // Product ID list aligned to BtnGemPacks indices
        private static readonly string[] GemProductIds =
        {
            IAPProductIds.Gems100,
            IAPProductIds.Gems500,
            IAPProductIds.Gems1000,
            IAPProductIds.Gems2500,
            IAPProductIds.Gems5000,
            IAPProductIds.Gems10000,
        };

        [Inject]
        public ShopPresenter(ShopState state, ShopView view, Inventory inventory, IIAPService iapService)
        {
            _state      = state;
            _view       = view;
            _inventory  = inventory;
            _iapService = iapService;
        }

        public void Start()
        {
            _iapService.Initialize();

            // ── Category tabs ─────────────────────────────────────────────────
            _view.BtnFeatured.OnClickedAsObservable()
                .Subscribe(_ => _state.SelectedCategory.Value = ShopCategory.Featured).AddTo(_disposables);
            _view.BtnCurrency.OnClickedAsObservable()
                .Subscribe(_ => _state.SelectedCategory.Value = ShopCategory.Currency).AddTo(_disposables);
            _view.BtnBundles.OnClickedAsObservable()
                .Subscribe(_ => _state.SelectedCategory.Value = ShopCategory.Bundles).AddTo(_disposables);
            _view.BtnOffers.OnClickedAsObservable()
                .Subscribe(_ => _state.SelectedCategory.Value = ShopCategory.Offers).AddTo(_disposables);

            _state.SelectedCategory.Subscribe(OnCategoryChanged).AddTo(_disposables);

            // ── Gem pack IAP buttons ──────────────────────────────────────────
            for (var i = 0; i < GemProductIds.Length; i++)
            {
                var productId = GemProductIds[i];
                _view.BtnGemPacks[i].OnClickedAsObservable()
                    .Subscribe(_ => _iapService.BuyProduct(productId)).AddTo(_disposables);
            }

            // ── Coin pack buttons (spend gems) ────────────────────────────────
            for (var i = 0; i < CoinPackCatalog.Packs.Length; i++)
            {
                var pack = CoinPackCatalog.Packs[i];

                _view.BtnCoinPacks[i].OnClickedAsObservable()
                    .Subscribe(_ =>
                    {
                        var cost = new InventoryItem(InventoryIds.Gems, new BigDouble(pack.GemCost));
                        if (!_inventory.TrySpend(cost)) return;
                        _inventory.Add(new InventoryItem(InventoryIds.Coins, new BigDouble(pack.Coins)));
                    }).AddTo(_disposables);

                // Grey out the button reactively whenever the player can't afford it
                var btn = _view.BtnCoinPacks[i];
                _inventory
                    .CanSpend(new InventoryItem(InventoryIds.Gems, new BigDouble(pack.GemCost)))
                    .Subscribe(canAfford => btn.SetEnabled(canAfford))
                    .AddTo(_disposables);
            }

            // ── HUD currency labels ───────────────────────────────────────────
            _inventory.GetAmount(InventoryIds.Gems)
                .Subscribe(v => _view.LabelGems.text = FormatAmount(v))
                .AddTo(_disposables);

            _inventory.GetAmount(InventoryIds.Coins)
                .Subscribe(v => _view.LabelCoins.text = FormatAmount(v))
                .AddTo(_disposables);
        }

        // ── Private helpers ───────────────────────────────────────────────────

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

        /// <summary>Formats a BigDouble for display; shows integers without decimals.</summary>
        private static string FormatAmount(BigDouble value)
            => BigDouble.IsInfinity(value) ? "∞" : value.ToString();

        // ── IDisposable ───────────────────────────────────────────────────────

        public void Dispose() => _disposables.Dispose();
    }
}
