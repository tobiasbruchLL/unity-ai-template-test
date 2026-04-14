using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;

namespace LL.Common.Toast
{
    public class ToastView : IStartable
    {
        private readonly UIDocument _document;

        public VisualElement Container { get; private set; }
        public Label Label { get; private set; }

        [Inject]
        public ToastView(UIDocument document)
        {
            _document = document;
        }

        public void Start()
        {
            var root = _document.rootVisualElement;
            Container = root.Q<VisualElement>("toast-container");
            Label     = root.Q<Label>("toast-label");
        }
    }
}
