using System.Windows;

namespace ÜbungWPFDownloadTool.BusinessLayer.Show
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