using CityManagementApp.DataAccess;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using CityManagementApp.BL; 
using CityManagementApp.DataAccess;
using CityManagementApp.ViewModel;
using System.Configuration;
using CityManagementApp.DataAccess.Repositories;

namespace CityManagementApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            var repositoryFactory = new RepositoryFactory(ConfigurationManager.ConnectionStrings["DefaultConnection"]?.ConnectionString);
            DataContext = new MainViewModel(repositoryFactory);
        }
    }
}
