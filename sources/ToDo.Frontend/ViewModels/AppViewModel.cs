using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ToDo.Backend.DTO.ToDo;

namespace ToDo.Frontend.ViewModels
{
    public sealed class AppViewModel : BaseViewModel
    {
        public AppViewModel(ListsViewModel listsViewModel, ItemsViewModel itemsViewModel)
        {
            ListsViewModel = listsViewModel;
            ItemsViewModel = itemsViewModel;

            ListsViewModel.SelectedListChanged = new EventCallback<ToDoList>(null,
                new Func<ToDoList, Task>(HandleSelectedListChangedAsync));
        }

        public ToDoList SelectedList { get; private set; }

        public ListsViewModel ListsViewModel { get; }
        
        public ItemsViewModel ItemsViewModel { get; }

        private async Task HandleSelectedListChangedAsync(ToDoList list)
        {
            SelectedList = list;
            
            await ItemsViewModel.ShowItemsAsync(list)
                .ConfigureAwait(true);
            
            OnStateChanged();
        }
    }
}