using System.Threading.Tasks;
using Microsoft.Win32;

namespace ÜbungWPFDownloadTool.BusinessLayer
{
    public class SelectFile : ISelectFile
    {
        public string ShowSaveFileDialog(string sourceFileName)
        {
            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.FileName = sourceFileName;
            saveFileDialog1.Filter = "All files (*.*)|*.*";
            saveFileDialog1.Title = "Save Download file";
            if (saveFileDialog1.ShowDialog() == true)
            {
                return saveFileDialog1.FileName;
            }
            return null;
        }
    }
}