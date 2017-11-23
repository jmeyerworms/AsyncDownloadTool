using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ÜbungWPFCaliburnMicro
{
    public class ButtonBehavior : Behavior<Button>
    {
        public static readonly DependencyProperty SProperty = DependencyProperty.Register(
            "S", typeof(string), typeof(ButtonBehavior), new PropertyMetadata(default(string)));

        public string S
        {
            get { return (string) GetValue(SProperty); }
            set { SetValue(SProperty, value); }
        }

        private int _counter;

        protected override void OnAttached()
        {
            AssociatedObject.Click += (sender, args) =>
            {
                _counter++;

                S = "Test " + _counter;
            };
        }
    }
}