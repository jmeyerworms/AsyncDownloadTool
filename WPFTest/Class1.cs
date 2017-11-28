using System.Linq;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using WPFDownloadTool.BusinessLayer;
using WPFDownloadTool.BusinessLayer.Download;
using WPFDownloadTool.BusinessLayer.Show;
using WPFDownloadTool.BusinessLayer.Url;
using WPFDownloadTool.ViewModels;

namespace WPFTest
{
    [TestFixture]
    public class DownloadTest
    {
        private Mock<ISelectFile> Getmockedselectedfile()
        {
            var mockedselectedfile = new Mock<ISelectFile>();
            mockedselectedfile.Setup(x => x.ShowSaveFileDialog("50MB.zip")).Returns(@"C:\test\50MB.zip");
            return mockedselectedfile;
        }
        private static Mock<IShow> GetMockedshowmessage()
        {
            var mockedshowmessage = new Mock<IShow>();
            mockedshowmessage.Setup(x => x.Message(""));
            return mockedshowmessage;
        }

        private static Mock<ICreateEngine> GetMockedICreateEngine()
        {
            var mockedICreateEngine = new Mock<ICreateEngine>();
            var mockedDownloadService = new Mock<IDownloadService>();
            mockedICreateEngine.Setup(x => x.For(Engine.HttpWeb)).Returns(mockedDownloadService.Object);
            return mockedICreateEngine;
        }

        private Mock<IUrlService> GetMockedIUrlService()
        {
            var mockedIUrlService = new Mock<IUrlService>();
            mockedIUrlService.Setup(x => x.IsUrlValid(@"http://ipv4.download.thinkbroadband.com/50MB.zip")).Returns(true);
            mockedIUrlService.Setup(x => x.GetValideDownloadFile(@"http://ipv4.download.thinkbroadband.com/50MB.zip"))
                .ReturnsAsync(@"50MB.zip");
            return mockedIUrlService;
        }

        [Test]
        public void Test1_AddDownload_ToList()
        {
            // Arrange
            
            var mockedshowmessage = GetMockedshowmessage();
            var mockedselectedfile = Getmockedselectedfile();
            var mockedIUrlService = GetMockedIUrlService();
            var mockedICreateEngine = GetMockedICreateEngine();


            var result = new DownloaderViewModel(mockedselectedfile.Object, mockedshowmessage.Object, mockedIUrlService.Object, mockedICreateEngine.Object)
            {
                Url = "http://ipv4.download.thinkbroadband.com/50MB.zip"
            };

            // Act
            result.AddDownload();
            

            // Assert
            result.Downloads.Count().Should().Be(1);
            
        }



        [Test]
        public void Test1_AddDownload_CHANGEVIEW()
        {

            var mockedshowmessage = GetMockedshowmessage();
            var mockedselectedfile = Getmockedselectedfile();
            var mockedIUrlService = GetMockedIUrlService();
            var mockedICreateEngine = GetMockedICreateEngine();


            var result = new DownloaderViewModel(mockedselectedfile.Object, mockedshowmessage.Object, mockedIUrlService.Object, mockedICreateEngine.Object)
            {
                Url = "http://ipv4.download.thinkbroadband.com/50MB.zip"
            };

            // Act
            result.AddDownload();
            result.StartDownload();            

            // Assert

            result.AreDownloadDetailsShown.ShouldBeEquivalentTo(true);

        }





    }
}