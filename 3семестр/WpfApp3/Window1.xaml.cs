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

using Path = System.IO.Path;


namespace WpfApp3
{
    /// <summary>
    /// Логика взаимодействия для Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public string choose;

        public Window1(List<string> history)
        {
            InitializeComponent();
            lastlisten.ItemsSource = history.Select(file => Path.GetFileNameWithoutExtension(file));
        }

        private void lastlisten_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            choose = lastlisten.SelectedItem.ToString();
            DialogResult = true;
        }
    }
}

