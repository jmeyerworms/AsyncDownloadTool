using Caliburn.Micro;
using PropertyChanged;
using System.Windows.Data;
using ÜbungWPFCaliburnMicro.Model;
using System;
using System.Globalization;

namespace ÜbungWPFCaliburnMicro.ViewModels
{
    public class StringToIntConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((int)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return int.Parse((string)value);
        }
    }

    public class KundeneintragViewModel : PropertyChangedBase
    {
        public KundeneintragViewModel(Customer customer)
        {
            Customer = customer;

            Vorname = Customer.FirstName;
            Nachname = Customer.LastName;
            Alter = Customer.Age;

            IsChanged = false;
        }
        public Customer Customer { get; }
        public string Nachname { get; set; }
        public string Vorname { get; set; }
        public int Alter { get; set; }

        public bool IsChanged { get; set; }

        private void OnNachnameChanged()
        {
            Customer.LastName = Nachname;
        }

        private void OnVornameChanged()
        {
            Customer.FirstName = Vorname;
        }

        private void OnAlterChanged()
        {
            Customer.Age = Alter;
        }

    }
}
