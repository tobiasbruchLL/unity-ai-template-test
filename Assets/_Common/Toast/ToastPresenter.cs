using System;
using R3;
using VContainer;
using VContainer.Unity;

namespace LL.Common.Toast
{
    public class ToastPresenter : IStartable, IDisposable
    {
        private readonly ToastService        _toastService;
        private readonly ToastView           _view;
        private readonly CompositeDisposable _disposables = new();
        private readonly SerialDisposable    _hideTimer   = new();

        [Inject]
        public ToastPresenter(ToastService toastService, ToastView view)
        {
            _toastService = toastService;
            _view         = view;
        }

        public void Start()
        {
            _hideTimer.AddTo(_disposables);
            _toastService.OnToastRequested
                .Subscribe(OnToast)
                .AddTo(_disposables);
        }

        private void OnToast(string message)
        {
            _view.Label.text = message;
            _view.Container.AddToClassList("toast-visible");

            // Assigning Disposable cancels any running hide timer before starting a new one
            _hideTimer.Disposable = Observable.Timer(TimeSpan.FromSeconds(2.5))
                .Subscribe(_ => _view.Container.RemoveFromClassList("toast-visible"));
        }

        public void Dispose() => _disposables.Dispose();
    }
}
