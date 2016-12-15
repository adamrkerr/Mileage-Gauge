using Android.App;
using Android.Widget;
using Android.OS;
using MileageGauge.Abstractions;
using MileageGauge.DI;
using Autofac;
using MileageGauge.Abstractions.ResponseModels;

namespace MileageGauge
{
    [Activity(Label = "Mileage Gauge", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private LinearLayout ConnectingLayout
        {
            get
            {
                return FindViewById<LinearLayout>(Resource.Id.ConnectingLayout);
            }
        }

        private LinearLayout VehicleInfoLayout
        {
            get
            {
                return FindViewById<LinearLayout>(Resource.Id.VehicleInfoLayout);
            }
        }

        IMainViewModel ViewModel { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            

            using (var scope = ContainerManager.Container.BeginLifetimeScope())
            {
                ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();
            }

            ViewModel.GetDiagnosticDeviceComplete += this.GetDiagnosticDeviceComplete;

            ViewModel.GetDiagnosticDevice();
            
        }

        private void GetDiagnosticDeviceComplete(GetDiagnosticDeviceResponse deviceResponse)
        {
            if (deviceResponse.Success)
            {
                ConnectingLayout.Visibility = Android.Views.ViewStates.Invisible;
                VehicleInfoLayout.Visibility = Android.Views.ViewStates.Visible;
            }
        }
    }
}

