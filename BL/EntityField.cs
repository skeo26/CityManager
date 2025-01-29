using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CityManagementApp.BL
{
    public class EntityField : INotifyPropertyChanged
    {
        private object _value;

        public string Name { get; set; }
        public object Value
        {
            get => _value;
            set
            {
                _value = value;
                OnPropertyChanged(nameof(Value));
            }
        }
        public PropertyInfo PropertyInfo { get; set; }
        public bool IsEditable { get; set; } = true;
        public bool IsRequired { get; set; } = true;

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
