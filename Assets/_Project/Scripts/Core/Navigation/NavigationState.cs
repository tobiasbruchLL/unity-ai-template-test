using R3;

namespace LL.Core.Navigation
{
    public class NavigationState
    {
        public ReactiveProperty<Tab> CurrentTab { get; } = new ReactiveProperty<Tab>(Tab.Home);
    }
}
