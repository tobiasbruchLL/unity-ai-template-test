using System;
using R3;
using VContainer.Unity;
using UnityEngine.UIElements;
using LL.Core.Navigation;

namespace LL.Presentation.Navigation
{
    public class NavigationPresenter : IStartable, IDisposable
    {
        private readonly NavigationState     _state;
        private readonly NavigationView      _view;
        private readonly CompositeDisposable _disposables = new CompositeDisposable();

        private Action _onPlaceholder;
        private Action _onHome;
        private Action _onShop;

        public NavigationPresenter(NavigationState state, NavigationView view)
        {
            _state = state;
            _view  = view;
        }

        public void Start()
        {
            _onPlaceholder = () => _state.CurrentTab.Value = LL.Core.Navigation.Tab.Placeholder;
            _onHome        = () => _state.CurrentTab.Value = LL.Core.Navigation.Tab.Home;
            _onShop        = () => _state.CurrentTab.Value = LL.Core.Navigation.Tab.Shop;

            _view.BtnPlaceholder.clicked += _onPlaceholder;
            _view.BtnHome.clicked        += _onHome;
            _view.BtnShop.clicked        += _onShop;

            // Fires immediately with Tab.Home, setting correct initial UI state
            _state.CurrentTab.Subscribe(OnTabChanged).AddTo(_disposables);
        }

        private void OnTabChanged(LL.Core.Navigation.Tab tab)
        {
            SetVisible(_view.TabPlaceholder, tab == LL.Core.Navigation.Tab.Placeholder);
            SetVisible(_view.TabHome,        tab == LL.Core.Navigation.Tab.Home);
            SetVisible(_view.TabShop,        tab == LL.Core.Navigation.Tab.Shop);
            SetActive(_view.BtnPlaceholder,  tab == LL.Core.Navigation.Tab.Placeholder);
            SetActive(_view.BtnHome,         tab == LL.Core.Navigation.Tab.Home);
            SetActive(_view.BtnShop,         tab == LL.Core.Navigation.Tab.Shop);
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

        public void Dispose()
        {
            _view.BtnPlaceholder.clicked -= _onPlaceholder;
            _view.BtnHome.clicked        -= _onHome;
            _view.BtnShop.clicked        -= _onShop;
            _disposables.Dispose();
        }
    }
}
