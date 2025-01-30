# CityManagementApp

## Описание проекта
CityManagementApp — это WPF-приложение для управления данными о городах, районах, улицах, домах, жителях и их регистрациях. Оно использует архитектуру MVVM и включает систему валидации данных перед сохранением в базу.

## Стек технологий
- **.NET (C#)** — основа приложения
- **WPF** — для пользовательского интерфейса
- **MVVM** — разделение логики, представления и данных
- **PostgreSQL** — база данных
ADO.NET** — работа с БД

## Структура проекта
```
CityManagementApp/
│── CityManagementApp.BL/            # Бизнес-логика
│── CityManagementApp.DataAccess/    # Доступ к данным
│   ├── Repositories/                # Репозитории
│   ├── Validate/                     # Валидаторы
│── CityManagementApp.ViewModel/      # ViewModel (MVVM)
│── CityManagementApp.Views/          # Представления (XAML)
│── CityManagementApp/                # Точка входа
```

## Основные компоненты

### 1. **Фабрика репозиториев**
**`RepositoryFactory`** создаёт репозитории для работы с базой данных.
```csharp
var repository = _repositoryFactory.GetRepository("city");
```

### 2. **Валидация данных**
Перед добавлением/изменением записей данные проверяются с помощью валидаторов:
```csharp
var validator = _validatorFactory.GetValidator("city");
var errors = await validator.ValidateAsync(newCity);
```

### 3. **Редактирование данных**
Используется **EntityEditorViewModel** и **EntityEditorWindow** для взаимодействия с пользователем.
```csharp
var editorViewModel = new EntityEditorViewModel { Entity = selectedItem };
var dialogResult = OpenEditorDialog(editorViewModel);
```

### 4. **Главная ViewModel**
**`MainViewModel`** управляет выбором таблицы, загрузкой данных и CRUD-операциями.
```csharp
public ICommand AddCommand { get; }
public ICommand EditCommand { get; }
public ICommand DeleteCommand { get; }
```

## Установка и запуск
1. Установите **PostgreSQL** и настройте соединение в `app.config`.
2. Соберите и запустите проект в Visual Studio.
3. Приложение откроется с главным окном для работы с данными.

## Рекомендации
- [ ] Улучшить обработку ошибок
- [ ] Реализовать Unit-тесты
