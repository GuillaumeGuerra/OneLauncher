using System;

namespace OneLauncher.Services.MessageService
{
    public interface IMessageService
    {
        void ShowException(Exception exception);

        void ShowErrorMessage(string message);
    }
}