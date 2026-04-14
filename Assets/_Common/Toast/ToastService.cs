using R3;

namespace LL.Common.Toast
{
    public class ToastService : IToastService
    {
        private readonly Subject<string> _subject = new();

        public Observable<string> OnToastRequested => _subject;

        public void Show(string message) => _subject.OnNext(message);
    }
}
