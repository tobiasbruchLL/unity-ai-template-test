using R3;
using UnityEngine.UIElements;

namespace LL.Common
{
    public static class ButtonExtensions
    {
        public static Observable<Unit> OnClickedAsObservable(this Button button)
            => Observable.FromEvent(h => button.clicked += h, h => button.clicked -= h);
    }
}
