using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ÜbungWPFCaliburnMicro.ViewModels
{
    public class KundenDetailsViewModel : Screen
    {
        public bool IsChanged{get; set;}

        public string Vorname { get; set; }                
        public string Nachname { get; set; }
        public int Alter { get; set; }
        public event EventHandler ContentChanged;

        public override void NotifyOfPropertyChange(string propertyName = null)
        {
            ContentChanged?.Invoke(this, EventArgs.Empty);
            base.NotifyOfPropertyChange(propertyName);
        }

        //public void OnIsChangedChanged()
        //{
        //    ContentChanged?.Invoke(this, EventArgs.Empty);
        //}


        public void Load()
        {
            IsChanged = false;
        }
    }
}
