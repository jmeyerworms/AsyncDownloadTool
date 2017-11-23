using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework.Internal;
using NUnit.Framework;
using ÜbungWPFDownloadTool;
using ÜbungWPFDownloadTool.Model;
using ÜbungWPFDownloadTool.ViewModels;
using Moq;
using ÜbungWPFDownloadTool.BusinessLayer;

namespace WPFTest
{
    [TestFixture]
    public class MyClass
    {
        [Test]
        public void Test1Download()
        {   
            //// Arrange
            //var mockedselectedfile = new Mock<ISelectFile>();
            //var mockedshowmessage = new Mock<IShow>();
            //mockedselectedfile.Setup(x => x.ShowSaveFileDialog("50MB.zip")).Returns(@"C:\test\");
            //mockedshowmessage.Setup(x => x.Message(""));

            //var result = new DownloaderViewModel(mockedselectedfile.Object, mockedshowmessage.Object);
            //result.Url = "http://ipv4.download.thinkbroadband.com/50MB.zip";

            //// Act
            //result.AddDownload();

            //// Assert
            //result.Downloads.Count().Should().Be(1);
        }
    }
}