using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace LL.Presentation.Shop
{
    public class ShopView : IStartable
    {
        private readonly UIDocument _document;

        // Category tab buttons
        public Button BtnFeatured { get; private set; }
        public Button BtnCurrency { get; private set; }
        public Button BtnBundles  { get; private set; }
        public Button BtnOffers   { get; private set; }

        // Content panels (toggled visible/hidden)
        public VisualElement PanelFeatured { get; private set; }
        public VisualElement PanelCurrency { get; private set; }
        public VisualElement PanelBundles  { get; private set; }
        public VisualElement PanelOffers   { get; private set; }

        // Currency HUD labels
        public Label LabelGems  { get; private set; }
        public Label LabelCoins { get; private set; }

        // IAP gem pack buy buttons (btn-gem-0 … btn-gem-5)
        public Button[] BtnGemPacks { get; private set; }

        // Coin pack buy buttons (btn-coin-0 … btn-coin-3)
        public Button[] BtnCoinPacks { get; private set; }

        [Inject]
        public ShopView(UIDocument document)
        {
            _document = document;
        }

        public void Start()
        {
            var root = _document.rootVisualElement;

            BtnFeatured = root.Q<Button>("btn-cat-featured");
            BtnCurrency = root.Q<Button>("btn-cat-currency");
            BtnBundles  = root.Q<Button>("btn-cat-bundles");
            BtnOffers   = root.Q<Button>("btn-cat-offers");

            PanelFeatured = root.Q<VisualElement>("panel-featured");
            PanelCurrency = root.Q<VisualElement>("panel-currency");
            PanelBundles  = root.Q<VisualElement>("panel-bundles");
            PanelOffers   = root.Q<VisualElement>("panel-offers");

            LabelGems  = root.Q<Label>("label-gems");
            LabelCoins = root.Q<Label>("label-coins");

            BtnGemPacks = new Button[]
            {
                root.Q<Button>("btn-gem-0"),
                root.Q<Button>("btn-gem-1"),
                root.Q<Button>("btn-gem-2"),
                root.Q<Button>("btn-gem-3"),
                root.Q<Button>("btn-gem-4"),
                root.Q<Button>("btn-gem-5"),
            };

            BtnCoinPacks = new Button[]
            {
                root.Q<Button>("btn-coin-0"),
                root.Q<Button>("btn-coin-1"),
                root.Q<Button>("btn-coin-2"),
                root.Q<Button>("btn-coin-3"),
            };
        }
    }
}
