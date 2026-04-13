using System;
using LL.Common;
using LL.Core.Navigation;
using R3;
using UnityEngine.UIElements;
using VContainer;
using VContainer.Unity;
using Tab = LL.Core.Navigation.Tab;

namespace LL.Presentation.Navigation
{
    public class NavigationPresenter : IStartable, IDisposable
    {
        private readonly NavigationState     _state;
        private readonly NavigationView      _view;
        private readonly CompositeDisposable _disposables = new();

        [Inject]
        public NavigationPresenter(NavigationState state, NavigationView view)
        {
            _state = state;
            _view  = view;
        }

        public void Start()
        {
            _view.BtnPlaceholder.OnClickedAsObservable()
                .Subscribe(_ => _state.CurrentTab.Value = Tab.Placeholder).AddTo(_disposables);
            _view.BtnHome.OnClickedAsObservable()
                .Subscribe(_ => _state.CurrentTab.Value = Tab.Home).AddTo(_disposables);
            _view.BtnShop.OnClickedAsObservable()
                .Subscribe(_ => _state.CurrentTab.Value = Tab.Shop).AddTo(_disposables);

            // Fires immediately with Tab.Home, setting correct initial UI state
            _state.CurrentTab.Subscribe(OnTabChanged).AddTo(_disposables);
        }

        private void OnTabChanged(Tab tab)
        {
            SetVisible(_view.TabPlaceholder, tab == Tab.Placeholder);
            SetVisible(_view.TabHome,        tab == Tab.Home);
            SetVisible(_view.TabShop,        tab == Tab.Shop);
            SetActive(_view.BtnPlaceholder,  tab == Tab.Placeholder);
            SetActive(_view.BtnHome,         tab == Tab.Home);
            SetActive(_view.BtnShop,         tab == Tab.Shop);
        }

        private static void SetVisible(VisualElement el, bool on)
        {
            if (on) el.AddToClassList("visible");
            else    el.RemoveFromClassList("visible");
        }

        private static void SetActive(VisualElement el, bool on)
        {
            if (on) el.AddToClassList("active");
            else    el.RemoveFromClassList("active");
        }

        public void Dispose() => _disposables.Dispose();
    }
}
