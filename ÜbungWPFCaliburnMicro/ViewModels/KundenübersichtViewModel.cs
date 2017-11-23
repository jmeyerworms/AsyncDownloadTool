using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using Caliburn.Micro;
using PropertyChanged;
using ÜbungWPFCaliburnMicro.BusinessLayer;

namespace ÜbungWPFCaliburnMicro.ViewModels
{
    public class KundenübersichtViewModel : Screen
    {

        public KundenübersichtViewModel(KundenDetailsViewModel kundenDetailsViewModel, ICustomerService customerService)
        {
            _customerService = customerService;

            Details = kundenDetailsViewModel;

            Details.ContentChanged += DetailsOnContentChanged;
        }

        private void DetailsOnContentChanged(object sender, EventArgs e)
        {
            ButtonIsActive = ((KundenDetailsViewModel)sender).IsChanged;
        }

        public ObservableCollection<KundeneintragViewModel> Kundenliste { get; set; }
        public string PropertyInViewModel { get; set; }
        public KundeneintragViewModel SelectedKundenDetails { get; set; }
        public KundenDetailsViewModel Details { get; set; }
        private readonly ICustomerService _customerService;
        
        public bool ButtonIsActive { get; set; }

        public void SaveChanges()
        {
            if (SelectedKundenDetails == null)
                return;

            ButtonIsActive = false;
            SelectedKundenDetails.Vorname = Details.Vorname;
            SelectedKundenDetails.Nachname = Details.Nachname;
            SelectedKundenDetails.Alter = Details.Alter;

            _customerService.Save(SelectedKundenDetails.Customer);
        }


        private void OnPropertyInViewModelChanged()
        {
            Debug.WriteLine("Clicked: " + PropertyInViewModel);
        }

        

        private void OnSelectedKundenDetailsChanged()
        {
            Details.Vorname = SelectedKundenDetails.Vorname;
            Details.Nachname = SelectedKundenDetails.Nachname;
            Details.Alter = SelectedKundenDetails.Alter;

            Details.Load();

            ButtonIsActive = false;
        }
        
        
        protected override void OnInitialize()
        {
            var customerViewModels = _customerService.Load().Select(x => new KundeneintragViewModel(x));

            Kundenliste = new ObservableCollection<KundeneintragViewModel>(customerViewModels);
        }
    }
}
