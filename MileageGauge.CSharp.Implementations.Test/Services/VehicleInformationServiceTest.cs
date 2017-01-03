using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Autofac.Extras.Moq;
using MileageGauge.CSharp.Implementations.Services;
using MileageGauge.CSharp.Abstractions.Services;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Implementations.Test.Services
{
    [TestClass]
    public class VehicleInformationServiceTest
    {
        [TestMethod]
        public async Task GetVehicleInformationFromApiTest()
        {
            using (var mock = AutoMock.GetLoose())
            {
                // Arrange - configure the mock
                var restUtility = mock.Create<RestUtility>();
                mock.Provide<IRestUtility>(restUtility);

                var vehicleService = mock.Create<VehicleInformationService>();

                // Act
                var actual = await vehicleService.GetVehicleInformation(null);

                Assert.IsNotNull(actual);

            }
        }
    }
}
