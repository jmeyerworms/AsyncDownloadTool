using System.Windows;

namespace WPFDownloadTool.BusinessLayer.Show
{
    public class Show : IShow
    {
        public Show()
        {
        }

        public void Message(string message)
        {
            MessageBox.Show(message);
        }
    }
}