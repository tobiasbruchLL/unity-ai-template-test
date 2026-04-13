using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace LL.Presentation.Navigation
{
    public class NavigationView : IStartable
    {
        private readonly UIDocument document;

        public VisualElement TabPlaceholder { get; private set; }
        public VisualElement TabHome { get; private set; }
        public VisualElement TabShop { get; private set; }
        public Button BtnPlaceholder { get; private set; }
        public Button BtnHome { get; private set; }
        public Button BtnShop { get; private set; }

        [Inject]
        public NavigationView(UIDocument document)
        {
            this.document = document;
        }

        public void Start()
        {
            var root = document.rootVisualElement;

            TabPlaceholder = root.Q<VisualElement>("tab-placeholder");
            TabHome = root.Q<VisualElement>("tab-home");
            TabShop = root.Q<VisualElement>("tab-shop");
            BtnPlaceholder = root.Q<Button>("btn-placeholder");
            BtnHome = root.Q<Button>("btn-home");
            BtnShop = root.Q<Button>("btn-shop");
        }
    }
}
