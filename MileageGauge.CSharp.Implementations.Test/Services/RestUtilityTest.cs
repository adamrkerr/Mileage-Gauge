using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Autofac.Extras.Moq;
using MileageGauge.CSharp.Implementations.Services;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;

namespace MileageGauge.CSharp.Implementations.Test.Services
{
    [TestClass]
    public class RestUtilityTest
    {
        [TestMethod]
        public async Task ExecuteGetRequestAsyncTest()
        {

            using (var mock = AutoMock.GetLoose())
            {
                // Arrange - configure the mock
                var restUtility = mock.Create<RestUtility>();

                // Act
                var actual = await restUtility.ExecuteGetRequestAsync<VinQueryResponse>("https://vpic.nhtsa.dot.gov/api/vehicles/DecodeVin/1C3AN69L24X*?format=json");

                Assert.IsNotNull(actual);

            }
        }
    }
}
