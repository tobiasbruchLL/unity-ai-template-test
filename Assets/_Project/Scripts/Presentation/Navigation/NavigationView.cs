using UnityEngine;
using UnityEngine.UIElements;

namespace LL.Presentation.Navigation
{
    [RequireComponent(typeof(UIDocument))]
    public class NavigationView : MonoBehaviour
    {
        public VisualElement TabPlaceholder { get; private set; }
        public VisualElement TabHome        { get; private set; }
        public VisualElement TabShop        { get; private set; }
        public Button BtnPlaceholder        { get; private set; }
        public Button BtnHome               { get; private set; }
        public Button BtnShop               { get; private set; }

        private void Awake()
        {
            var root = GetComponent<UIDocument>().rootVisualElement;
            TabPlaceholder = root.Q<VisualElement>("tab-placeholder");
            TabHome        = root.Q<VisualElement>("tab-home");
            TabShop        = root.Q<VisualElement>("tab-shop");
            BtnPlaceholder = root.Q<Button>("btn-placeholder");
            BtnHome        = root.Q<Button>("btn-home");
            BtnShop        = root.Q<Button>("btn-shop");
        }
    }
}
