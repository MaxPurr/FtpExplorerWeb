using Microsoft.AspNetCore.Components;
using System;

namespace FtpExplorerWeb.Presentation.Components.Common
{
    public abstract class OpeningComponentBase : ComponentBase
    {
        private bool _isOpened = false;

        public bool IsOpened => _isOpened;
        protected abstract string RegularClasses { get; }
        protected virtual string ClassesOnOpen { get; } = string.Empty;
        protected virtual string ClassesOnClose { get; } = string.Empty;
        protected event Action? Opened;
        protected event Action? Closed;
        protected string Classes
        {
            get
            {
                string additionalClasses = _isOpened ? ClassesOnOpen : ClassesOnClose;
                return $"{RegularClasses} {additionalClasses}";
            }
        }

        public void OnOpened()
        {
            Opened?.Invoke();
        }

        public void OnClosed()
        {
            Closed?.Invoke();
        }

        public void Open()
        {
            _isOpened = true;
            OnOpened();
        }

        public void Close()
        {
            _isOpened = false;
            OnClosed();
        }
    }
}