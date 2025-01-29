using Npgsql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Text.RegularExpressions;
using CityManagementApp.DataAccess.Repositories;

namespace CityManagementApp
{
    /// <summary>
    /// Логика взаимодействия для AddEditWindow.xaml
    /// </summary>
    public partial class AddEditWindow : Window
    {
        private readonly Type _entityType;
        private IRepository<object>? _currentRepository;
        private readonly Dictionary<string, TextBox> _fieldInputs = new();
        private string _connectionString = "";
        public Dictionary<string, TextBox> FieldInputs => _fieldInputs;

        public AddEditWindow(Type entityType, IRepository<object> repository, object? existingEntity = null)
        {
            InitializeComponent();
            _entityType = entityType;
            _currentRepository = repository;
            GenerateFields(existingEntity);
        }


        private async void GenerateFields(object? existingEntity = null)
        {
            var properties = _entityType.GetProperties()
                .Where(p => p.CanWrite)
                .ToList();

            foreach (var property in properties)
            {
                FieldsPanel.Children.Add(new TextBlock
                {
                    Text = property.Name,
                    Margin = new Thickness(0, 5, 0, 5),
                    FontWeight = FontWeights.Bold
                });

                var textBox = new TextBox { Margin = new Thickness(0, 5, 0, 10), IsEnabled = property.Name != "Id" };
                if (property.Name.Equals("Id", StringComparison.OrdinalIgnoreCase) && property.PropertyType == typeof(int))
                {
                    if (existingEntity == null) // Режим добавления
                    {
                        var nextId = await _currentRepository.GetNextIdAsync(_entityType.Name);
                        textBox.Text = nextId.ToString();
                        textBox.IsEnabled = false;
                    }
                    else // Режим редактирования
                    {
                        var currentId = property.GetValue(existingEntity);
                        textBox.Text = currentId?.ToString() ?? string.Empty;
                        textBox.IsEnabled = false;
                    }
                }
                else
                {
                   
                    if (existingEntity != null)
                    {
                        var value = property.GetValue(existingEntity);
                        textBox.Text = value?.ToString() ?? string.Empty;
                    }
                }
                FieldsPanel.Children.Add(textBox);

                _fieldInputs[property.Name] = textBox;
            }
        }

        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var entity = Activator.CreateInstance(_entityType);
            var isValid = true;
            var errorMessages = new List<string>();

            var fieldsToExcludeFromDuplicateCheck = new List<string> { "Id", "population", "id_city", "id_district", "sizeStreet", "fullName", "birthday", "id_house", "dateRegistration", "dataDeregistration", "id_resident", "id_house", "numberHouse", "floorHouse", "id_street" };
            var fieldsCanBeEmpty = new List<string> { "dataDeregistration" , "dateRegistration" };
            foreach (var property in _entityType.GetProperties().Where(p => p.CanWrite))
            {
                if (_fieldInputs.TryGetValue(property.Name, out var textBox))
                {
                    try
                    {
                        if (string.IsNullOrWhiteSpace(textBox.Text) && !fieldsCanBeEmpty.Contains(property.Name))
                        {
                            errorMessages.Add($"Поле {property.Name} не может быть пустым.");
                            isValid = false;
                            continue;
                        }

                        object value;

                        if (property.PropertyType == typeof(int))
                        {
                            if (!int.TryParse(textBox.Text, out var intValue))
                            {
                                errorMessages.Add($"Поле {property.Name} должно быть числом.");
                                isValid = false;
                                continue;
                            }
                            value = intValue;
                        }
                        else if (property.PropertyType == typeof(double))
                        {
                            if (!double.TryParse(textBox.Text, out var doubleValue))
                            {
                                errorMessages.Add($"Поле {property.Name} должно быть числом с плавающей точкой.");
                                isValid = false;
                                continue;
                            }
                            value = doubleValue;
                        }
                        else if (property.PropertyType == typeof(DateOnly))
                        {
                            if (!DateOnly.TryParse(textBox.Text, out var dateOnlyValue))
                            {
                                errorMessages.Add($"Поле {property.Name} должно быть датой в формате 'ГГГГ-ММ-ДД'.");
                                isValid = false;
                                continue;
                            }
                            value = dateOnlyValue;
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            if (textBox.Text.Length > 100)
                            {
                                errorMessages.Add($"Поле {property.Name} не должно превышать 100 символов.");
                                isValid = false;
                                continue;
                            }

                            if (!Regex.IsMatch(textBox.Text, @"^[А-Яа-яЁё0-9\s\.\:]+$"))
                            {
                                errorMessages.Add($"Поле {property.Name} может содержать только буквы, цифры и пробелы.");
                                isValid = false;
                                continue;
                            }

                            value = textBox.Text;
                        }
                        else
                        {
                            value = Convert.ChangeType(textBox.Text, property.PropertyType);
                        }

                        if (!fieldsToExcludeFromDuplicateCheck.Contains(property.Name))
                        {
                            if (await IsValueDuplicateAsync(property.Name, value))
                            {
                                errorMessages.Add($"Значение {textBox.Text} для поля {property.Name} уже существует.");
                                isValid = false;
                                continue;
                            }
                        }

                        property.SetValue(entity, value);
                    }
                    catch (Exception ex)
                    {
                        errorMessages.Add($"Ошибка в поле {property.Name}: {ex.Message}");
                        isValid = false;
                    }
                }
            }

            if (!isValid)
            {
                MessageBox.Show(string.Join("\n", errorMessages), "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            DialogResult = true;
            Tag = entity;
            Close();
        }

        private async Task<bool> IsValueDuplicateAsync(string propertyName, object value)
        {
            using (var connection = new NpgsqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                var query = $"SELECT COUNT(*) FROM {_entityType.Name} WHERE {propertyName} = @value";
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@value", value ?? DBNull.Value);
                    var result = await command.ExecuteScalarAsync();
                    return Convert.ToInt32(result) > 1;
                }
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
