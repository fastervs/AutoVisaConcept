using AutoVisaConcept.VM;
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

namespace AutoVisaConcept
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        int selected_index = -1;
        ViewModel VM = new ViewModel();
        public MainWindow()
        {
            InitializeComponent();
            
            DataContext = VM;
            
            
            //dataGrid.SelectedIndex
            //image.Source=dataGrid.SelectedItem.
        }

        

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selected_index = dataGrid.SelectedIndex;
            image.Source = new BitmapImage(new Uri(VM.Persons[selected_index].FilePath));
        }
    }
}
