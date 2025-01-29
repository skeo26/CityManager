using CityManagementApp.BL;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CityManagementApp.Views;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using CityManagementApp.DataAccess.Repositories;
using CityManagementApp.DataAccess.Validate;
using System.Text.Json;
using System.Reflection;

namespace CityManagementApp.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private readonly IRepositoryFactory _repositoryFactory;
        private readonly ValidatorFactory _validatorFactory;
        private object _selectedItem;
        private string _selectedTable;

        public ObservableCollection<object> Items { get; set; } = new ObservableCollection<object>();
        public ICommand LoadTableCommand { get; }
        public ICommand AddCommand { get; }
        public ICommand EditCommand { get; }
        public ICommand DeleteCommand { get; }

        public bool IsItemSelected => SelectedItem != null;

        public object SelectedItem
        {
            get => _selectedItem;
            set
            {
                _selectedItem = value;
                OnPropertyChanged(nameof(SelectedItem));
                OnPropertyChanged(nameof(IsItemSelected));
                (EditCommand as RelayCommand)?.RaiseCanExecuteChanged();
                (DeleteCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public string SelectedTable
        {
            get => _selectedTable;
            set
            {
                _selectedTable = value;
                OnPropertyChanged(nameof(SelectedTable));
                LoadTableDataAsync();
                (AddCommand as RelayCommand)?.RaiseCanExecuteChanged();
            }
        }

        public MainViewModel(IRepositoryFactory repositoryFactory)
        {
            _repositoryFactory = repositoryFactory;
            _validatorFactory = new ValidatorFactory(repositoryFactory);
            LoadTableNames();
            LoadTableCommand = new RelayCommand(LoadTableDataAsync);
            AddCommand = new RelayCommand(AddItem, CanAddItem);
            EditCommand = new RelayCommand(EditItem, CanEditItemOrDelete);
            DeleteCommand = new RelayCommand(DeleteItem, CanEditItemOrDelete);
        }

        private ObservableCollection<string> _tableNames = new ObservableCollection<string>();
        public ObservableCollection<string> TableNames
        {
            get => _tableNames;
            set
            {
                _tableNames = value;
                OnPropertyChanged(nameof(TableNames));
            }
        }

        private bool CanEditItemOrDelete()
        {
            return IsItemSelected;
        }

        private void LoadTableNames()
        {
            var tableNames = _repositoryFactory.GetTableNames();
            foreach (var name in tableNames)
            {
                TableNames.Add(name);
            }
        }

        private async void LoadTableDataAsync()
        {
            if (string.IsNullOrEmpty(SelectedTable)) return;

            var repository = _repositoryFactory.GetRepository(SelectedTable);
            if (repository == null) return;

            var data = await repository.GetAllAsync();
            Items.Clear();
            foreach (var item in data)
            {
                Items.Add(item);
            }
            OnPropertyChanged(nameof(Items));
            ResetDataGrid();
        }

        private void ResetDataGrid()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dataGrid = Application.Current.MainWindow?.FindName("dataGrid") as DataGrid;
                if (dataGrid != null)
                {
                    var currentItemsSource = dataGrid.ItemsSource;
                    dataGrid.ItemsSource = null;
                    dataGrid.ItemsSource = currentItemsSource;
                }
            });
        }

        private async void DeleteItem()
        {
            if (SelectedItem == null) return;

            var repository = _repositoryFactory.GetRepository(SelectedTable);
            if (repository == null) return;

            if (SelectedItem is BaseEntity entity)
            {
                await repository.DeleteAsync(entity.Id);
                LoadTableDataAsync();
            }
        }

        private async void AddItem()
        {
            var repository = _repositoryFactory.GetRepository(SelectedTable);
            if (repository == null) return;

            var newItem = _repositoryFactory.CreateNewItemForTable(SelectedTable);
            if (newItem == null) return;

            var maxID = await repository.GetNextIdAsync(SelectedTable);
            var idProperty = newItem.GetType().GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(int))
            {
                idProperty.SetValue(newItem, maxID);
            }

            var editorViewModel = new EntityEditorViewModel { Entity = newItem };
            var dialogResult = OpenEditorDialog(editorViewModel);

            if (dialogResult != null)
            {
                var validationErrors = await ValidateEntityAsync(dialogResult);
                if (validationErrors.Any())
                {
                    ShowValidationErrors(validationErrors);
                    return;
                }
                await repository.AddAsync(dialogResult);
                LoadTableDataAsync();
            }
        }

        private async void EditItem()
        {
            var repository = _repositoryFactory.GetRepository(SelectedTable);
            if (SelectedItem == null) return;

            var editableEntity = DeepCopy(SelectedItem);

            var editorViewModel = new EntityEditorViewModel
            {
                Entity = editableEntity
            };

            var dialogResult = OpenEditorDialog(editorViewModel);

            if (dialogResult != null)
            {
                var validationErrors = await ValidateEntityAsync(dialogResult);
                if (validationErrors.Any())
                {
                    ShowValidationErrors(validationErrors);
                    return;
                }

                UpdateOriginalEntity(SelectedItem, dialogResult);

                await repository.UpdateAsync(editorViewModel.Entity);
                LoadTableDataAsync();
            }
        }

        private void ShowValidationErrors(List<string> validationErrors)
        {
            var errorMessage = string.Join(Environment.NewLine, validationErrors);
            MessageBox.Show(errorMessage, "Ошибка валидации", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private bool CanAddItem()
        {
            return !string.IsNullOrEmpty(SelectedTable);
        }

        private object OpenEditorDialog(EntityEditorViewModel editorViewModel)
        {
            var dialog = new EntityEditorWindow
            {
                DataContext = editorViewModel 
            };
            editorViewModel.CurrentWindow = dialog;

            bool? result = dialog.ShowDialog();
            return result == true ? editorViewModel.Entity : null;
        }

        private async Task<List<string>> ValidateEntityAsync(object entity)
        {
            var validator = _validatorFactory.GetValidator(SelectedTable);
            return await validator.ValidateAsync(entity);
        }

        private T DeepCopy<T>(T source)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));

            var type = source.GetType();
            var copy = Activator.CreateInstance(type);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanRead && property.CanWrite)
                {
                    var value = property.GetValue(source);
                    property.SetValue(copy, value);
                }
            }

            return (T)copy;
        }

        private void UpdateOriginalEntity(object original, object updated)
        {
            foreach (var property in original.GetType().GetProperties())
            {
                if (property.CanWrite)
                {
                    var updatedValue = property.GetValue(updated);
                    property.SetValue(original, updatedValue);
                }
            }
        }
    }
}
