using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace LL.Presentation.Shop
{
    public class ShopView : IStartable
    {
        private readonly UIDocument document;


        // Category tab buttons
        public Button BtnFeatured { get; private set; }
        public Button BtnCurrency { get; private set; }
        public Button BtnBundles { get; private set; }
        public Button BtnOffers { get; private set; }

        // Content panels (toggled visible/hidden)
        public VisualElement PanelFeatured { get; private set; }
        public VisualElement PanelCurrency { get; private set; }
        public VisualElement PanelBundles { get; private set; }
        public VisualElement PanelOffers { get; private set; }

        // Currency HUD labels
        public Label LabelGems { get; private set; }
        public Label LabelCoins { get; private set; }

        [Inject]
        public ShopView(UIDocument document)
        {
            this.document = document;
        }

        public void Start()
        {
            var root = document.rootVisualElement;

            BtnFeatured = root.Q<Button>("btn-cat-featured");
            BtnCurrency = root.Q<Button>("btn-cat-currency");
            BtnBundles = root.Q<Button>("btn-cat-bundles");
            BtnOffers = root.Q<Button>("btn-cat-offers");

            PanelFeatured = root.Q<VisualElement>("panel-featured");
            PanelCurrency = root.Q<VisualElement>("panel-currency");
            PanelBundles = root.Q<VisualElement>("panel-bundles");
            PanelOffers = root.Q<VisualElement>("panel-offers");

            LabelGems = root.Q<Label>("label-gems");
            LabelCoins = root.Q<Label>("label-coins");
        }
    }
}
