using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ÜbungWPF.Annotations;

namespace ÜbungWPF.Model
{
    class Kundendetails : INotifyPropertyChanged
    {
        private string _vorname;
        private string _nachname;
        public int Id { get; set; }


        public string Nachname
        {
            get { return _nachname; }
            set
            {
                if (value == _nachname) return;
                _nachname = value;
                OnPropertyChanged();
            }
        }

        public string Vorname
        {
            get { return _vorname; }
            set
            {
                if (value == _vorname) return;
                _vorname = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public override string ToString()
        {
            return this.Nachname + ", " + this.Vorname;
        }
    }
}
