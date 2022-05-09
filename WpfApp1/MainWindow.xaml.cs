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
using WpfApp1.Common;
using WpfApp1.ViewModel;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = MainViewModel.Instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainViewModel.Instance.ReadXml())
            {

            }
            if (MainViewModel.Instance.ReadIni())
            {

            }
            if(MainViewModel.Instance.ReadJson())
            {

            }

            if(MainViewModel.Instance.Run())
            {
                
            }
        }

        private void lviItem1_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {

        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MainViewModel.Instance.Stop();
        }
    }
}
