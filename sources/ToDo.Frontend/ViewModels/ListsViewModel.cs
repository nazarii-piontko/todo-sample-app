using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using ToDo.Backend.DTO.ToDo;
using ToDo.Frontend.Services.Abstractions;

namespace ToDo.Frontend.ViewModels
{
    public sealed class ListsViewModel : BaseViewModel
    {
        private readonly IToDoListsService _listsService;

        public ListsViewModel(IToDoListsService listsService)
        {
            _listsService = listsService;
        }
        
        public EventCallback<ToDoList> SelectedListChanged { get; set; }

        public IList<ToDoList> Lists { get; private set; } = new List<ToDoList>();
        
        public ToDoList SelectedList { get; private set; }
        
        [Required]
        public string NewListName { get; set; }
        
        public override async Task InitializeAsync()
        {
            try
            {
                Lists = await _listsService.GetListsAsync()
                    .ConfigureAwait(true);
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }

        public async Task CreateNewListAsync()
        {
            try
            {
                var list = await _listsService.CreateListAsync(NewListName)
                    .ConfigureAwait(true);
                
                Lists.Add(list);

                NewListName = string.Empty;
                
                OnStateChanged();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }

        public async Task SelectListAsync(ToDoList list)
        {
            if (list.Id == SelectedList?.Id)
                return;
            
            SelectedList = list;
            
            await SelectedListChanged.InvokeAsync(list)
                .ConfigureAwait(true);
            
            OnStateChanged();
        }
        
        public async Task EditListAsync(ToDoList list, string newName)
        {
            try
            {
                var item = await _listsService.EditListAsync(list.Id, newName)
                    .ConfigureAwait(true);
                
                list.Name = item.Name;
                
                if (SelectedList?.Id == list.Id)
                {
                    await SelectedListChanged.InvokeAsync(SelectedList)
                        .ConfigureAwait(true);
                }
                
                OnStateChanged();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync(ex)
                    .ConfigureAwait(true);
            }
        }
        
        public async Task RemoveListAsync(ToDoList list)
        {
            try
            {
                await _listsService.DeleteListAsync(list.Id)
                    .ConfigureAwait(true);
                
                Lists.Remove(list);

                if (SelectedList?.Id == list.Id)
                {
                    SelectedList = null;
                    await SelectedListChanged.InvokeAsync(null)
                        .ConfigureAwait(true);
                }
                
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