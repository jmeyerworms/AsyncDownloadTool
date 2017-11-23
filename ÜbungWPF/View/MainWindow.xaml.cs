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
using ÜbungWPF.Model;
using ÜbungWPF.Viewmodel;

namespace ÜbungWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private KundeDetailsViewModel _viewModel;
        public MainWindow()
        {
            InitializeComponent();

            _viewModel = new KundeDetailsViewModel();
            this.DataContext = _viewModel;
            
            ListViewKunden.SelectionChanged += ListViewKundenOnSelectionChanged;
            ButtonSave.Click += ButtonSave_Click;
        }

        private void ListViewKundenOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
        {
            var item = ListViewKunden.SelectedItem as Kundendetails;
            TextBoxVorname.Text = item.Vorname;
            TextBoxNachname.Text = item.Nachname;
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            
            if ((ListViewKunden.SelectedItem is Kundendetails))
            {
                _viewModel.SaveChanges((Kundendetails)ListViewKunden.SelectedItem,new Kundendetails() { Vorname = TextBoxVorname.Text, Nachname = TextBoxNachname.Text } );
                ListViewKunden.Items.Refresh();
            }
            

        }



    }
}
