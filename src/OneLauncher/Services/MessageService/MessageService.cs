using System;
using System.Windows;

namespace OneLauncher.Services.MessageService
{
    public class MessageService : IMessageService
    {
        public void ShowException(Exception exception)
        {
            MessageBox.Show(exception.Message, "Unhandled exception", MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}