using System.Threading.Tasks;

namespace ÜbungWPFDownloadTool.BusinessLayer
{
    public interface ISelectFile
    {
        string ShowSaveFileDialog(string sourceFileName);
    }
}