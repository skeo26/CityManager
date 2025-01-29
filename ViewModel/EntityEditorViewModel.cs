using CityManagementApp.BL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CityManagementApp.ViewModel
{
    public class EntityEditorViewModel : INotifyPropertyChanged
    {
        private object _entity;

        public object Entity
        {
            get => _entity;
            set
            {
                _entity = value;
                OnPropertyChanged(nameof(Entity));
                GenerateFields();
            }
        }

        private List<EntityField> _fields;
        public List<EntityField> Fields
        {
            get => _fields;
            private set
            {
                _fields = value;
                OnPropertyChanged(nameof(Fields));
            }
        }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public Window CurrentWindow { get; set; }

        public event Action<object> SaveRequested;

        public EntityEditorViewModel()
        {
            SaveCommand = new RelayCommand(Save, CanSave);
            CancelCommand = new RelayCommand(Cancel);
        }

        private void GenerateFields()
        {
            if (Entity == null) return;

            var properties = Entity.GetType().GetProperties();
            Fields = properties.Select(p => new EntityField
            {
                Name = p.Name,
                Value = p.GetValue(Entity),
                PropertyInfo = p,
                IsEditable = !string.Equals(p.Name, "Id", StringComparison.OrdinalIgnoreCase), // Поле "Id" не редактируемо
                IsRequired = !string.Equals(p.Name, "dataDeregistration", StringComparison.OrdinalIgnoreCase) // Сделать необязательным
            }).ToList();

            foreach (var field in Fields)
            {
                field.PropertyChanged += (s, e) => ((RelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }

        private void Save()
        {
            foreach (var field in Fields)
            {
                var targetType = field.PropertyInfo.PropertyType;
                try
                {
                    if (field.Value == null && Nullable.GetUnderlyingType(targetType) != null)
                    {
                        field.PropertyInfo.SetValue(Entity, null);
                    }
                    else
                    {
                        object convertedValue = Convert.ChangeType(field.Value, targetType);
                        field.PropertyInfo.SetValue(Entity, convertedValue);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Ошибка введённых данных");
                    return;
                }
            }
            CurrentWindow.DialogResult = true;
            CurrentWindow.Close();
        }


        private bool CanSave() 
        {
            return Entity != null && Fields.Where(field => field.IsRequired).All(field => field.Value != null);
        }

        private void Cancel()
        {
            CurrentWindow.DialogResult = false; 
            CurrentWindow.Close();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
