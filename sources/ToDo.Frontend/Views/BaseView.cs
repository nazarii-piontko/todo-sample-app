using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using ToDo.Frontend.ViewModels;

namespace ToDo.Frontend.Views
{
    [Authorize]
    public abstract class BaseView : ComponentBase
    {
        [Inject] 
        protected IJSRuntime JSRuntime { get; set; }
        
        [Inject] 
        protected IServiceProvider Provider { get; set; }
        
        [Inject] 
        protected NavigationManager NavigationManager { get; set; }
        
        internal ValueTask ShowErrorAsync(string message)
        {
            return JSRuntime.InvokeVoidAsync("showError", message);
        }

        internal void NavigateTo(string uri, bool forceLoad)
        {
            NavigationManager.NavigateTo(uri, forceLoad);
        }
    }
    
    public abstract class BaseView<TModel> : BaseView
        where TModel : BaseViewModel
    {
        private TModel _model;

        protected override Task OnInitializedAsync()
        {
            return Model.InitializeAsync();
        }

        [Parameter]
        public TModel Model
        {
            get
            {
                if (_model == null)
                    SetModel(CreateViewModel()); 
                return _model;
            }
            set => SetModel(value);
        }

        private TModel CreateViewModel()
        {
            var model = Provider.GetRequiredService<TModel>();
            return model;
        }

        private void SetModel(TModel model)
        {
            if (ReferenceEquals(_model, model))
                return;
            
            if (_model != null)
                _model.StateChanged -= ModelOnStateChanged;

            _model = model;
            
            if (_model != null)
            {
                _model.View = this;
                _model.StateChanged += ModelOnStateChanged;
            }
        }

        private void ModelOnStateChanged(object sender, EventArgs e)
        {
            StateHasChanged();
        }
    }
}