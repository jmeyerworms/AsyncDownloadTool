using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using ÜbungWPF.Annotations;
using ÜbungWPF.Model;

namespace ÜbungWPF.Viewmodel
{
    class KundeDetailsViewModel: INotifyPropertyChanged
    {
    private ObservableCollection<Kundendetails> _kundenListe = new ObservableCollection<Kundendetails>();

    public ObservableCollection<Kundendetails> KundenListe
    {
        get { return _kundenListe; }
        set
        {
            if (Equals(value, _kundenListe)) return;
            _kundenListe = value;
            OnPropertyChanged();
        }
    }

        #region Changedetails

        internal void SaveChanges(Kundendetails selectedItem, Kundendetails kundendetails)
        {
            if (kundendetails == null) return;
            
            selectedItem.Vorname = kundendetails.Vorname;
            selectedItem.Nachname = kundendetails.Nachname;

        }

        #endregion

        public KundeDetailsViewModel()
    {
        KundenListe.Add(new Kundendetails() {Vorname = "Müller", Nachname = "Mustermann"});
        KundenListe.Add(new Kundendetails() {Vorname = "Satter", Nachname = "jens"});
    }


    #region PropertyChanged

    public event PropertyChangedEventHandler PropertyChanged;

    [NotifyPropertyChangedInvocator]
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


        #endregion PropertyChanged

    }
}
