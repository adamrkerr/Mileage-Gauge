using MileageGauge.CSharp.Abstractions.ResponseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MileageGauge.CSharp.Abstractions.ViewModels
{
    public interface IAddVehicleViewModel
    {
        VehicleModel CurrentVehicle { get; set; }

        Action<LoadVehicleDetailsCompleteResponse> LoadVehicleDetailsComplete
        {
            get; set;
        }

        Action<LoadVehicleDetailsOptionRequiredResponse> LoadVehicleDetailsOptionsRequired
        {
            get; set;
        }

        Action<LoadVehicleDetailsModelRequiredResponse> LoadVehicleDetailsModelRequired
        {
            get; set;
        }

        Task LoadVehicleDetailsFromDevice();

        Task LoadVehicleDetailsFromVin(string vin);

        Task CompleteVehicleOption(VehicleOptionViewModel selectedOption);

        Task CompleteVehicleModel(string selectedModel);

        Task<AddVehicleToCollectionResponse> AddVehicleToCollection();
    }
}
