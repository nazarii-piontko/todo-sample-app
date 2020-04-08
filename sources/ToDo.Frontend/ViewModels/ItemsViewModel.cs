using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using ToDo.Backend.DTO.ToDo;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.ViewModels
{
    public sealed class ItemsViewModel : BaseViewModel
    {
        private readonly IToDoItemsService _itemsService;

        public ItemsViewModel(IToDoItemsService itemsService)
        {
            _itemsService = itemsService;
        }

        public IList<ToDoItem> Items { get; private set; } = new List<ToDoItem>();
        
        public ToDoList CurrentList { get; private set; }
        
        public ToDoItem SelectedItem { get; private set; }
        
        [Required]
        public string NewItemText { get; set; }
        
        public async Task ShowItemsAsync(ToDoList list)
        {
            try
            {
                CurrentList = list;

                if (list != null)
                {
                    Items = await _itemsService.GetItemsAsync(CurrentList.Id)
                        .ConfigureAwait(true);
                }
                else
                {
                    Items = new List<ToDoItem>();
                }

                OnStateChanged();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
        
        public void SelectItem(ToDoItem item)
        {
            if (item.Id == SelectedItem?.Id)
                return;
            
            SelectedItem = item;
            
            OnStateChanged();
        }

        public async Task CreateNewItemAsync()
        {
            try
            {
                var item = await _itemsService.CreateItemAsync(CurrentList.Id, NewItemText)
                    .ConfigureAwait(true);
                
                Items.Add(item);

                NewItemText = string.Empty;
                
                OnStateChanged();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
        
        public async Task EditItemAsync(ToDoItem item, string newText)
        {
            try
            {
                var editedItem = await _itemsService.EditItemAsync(CurrentList.Id, item.Id, newText)
                    .ConfigureAwait(true);
                
                item.Text = editedItem.Text;
                
                OnStateChanged();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
        
        public async Task RemoveItemAsync(ToDoItem item)
        {
            try
            {
                await _itemsService.DeleteItemAsync(CurrentList.Id, item.Id)
                    .ConfigureAwait(true);
                
                Items.Remove(item);
                
                OnStateChanged();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
    }
}