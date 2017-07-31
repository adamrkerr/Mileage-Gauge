using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac.Extras.Moq;
using MileageGauge.CSharp.Implementations.Services;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.Services.ELM327;
using Moq;
using System.Linq;

namespace MileageGauge.CSharp.Implementations.Test.Services
{
    [TestClass]
    public class DiagnosticDeviceServiceTest
    {
        [TestMethod]
        public async Task TestDiagnosticCodeParsing()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange - configure the mock
                var elmSvc = mock.Mock<IELM327CommunicationService>();

                elmSvc.Setup(e => e.GetDiagnosticCodes())
                .ReturnsAsync("165600000000");

                elmSvc.Setup(e => e.Connect(It.IsAny<string>()))
                    .ReturnsAsync(new ConnectionResponse { Success=true });

                var svcResolver = mock.Mock<ICommunicationServiceResolver>().Setup(e => e.ResolveCommunicationService(It.IsAny<string>()))
                    .Returns(elmSvc.Object);

                var diagService = mock.Create<DiagnosticDeviceService>();

                await diagService.Connect("test");

                // Act
                var actual = await diagService.GetDiagnosticCodes();

                Assert.IsNotNull(actual);
                Assert.AreEqual("P1656", actual.ElementAt(0));

            }
        }
    }
}
