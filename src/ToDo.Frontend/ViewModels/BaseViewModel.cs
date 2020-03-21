using System;
using System.Threading.Tasks;
using ToDo.Frontend.Views;

namespace ToDo.Frontend.ViewModels
{
    public abstract class BaseViewModel
    {
        public event EventHandler StateChanged;
        
        internal BaseView View { get; set; }
        
        public virtual Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        protected ValueTask ShowErrorAsync(string message)
        {
            return View.ShowErrorAsync(message);
        }
        
        protected ValueTask ShowErrorAsync(Exception ex)
        {
            return ShowErrorAsync(ex.Message);
        }

        protected void NavigateTo(string uri, bool forceLoad = false)
        {
            View.NavigateTo(uri, forceLoad);
        }

        protected void OnStateChanged()
        {
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}