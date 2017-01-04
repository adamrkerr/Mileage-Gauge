using Android.App;
using Android.Widget;
using Android.OS;
using MileageGauge.DI;
using Autofac;
using System;
using Android.Content;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

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

        private Button StartScanningButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.StartScanningButton);
            }
        }

        private Button RefreshVehicleButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.RefreshVehicleButton);
            }
        }

        private TextView YearText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.YearText);
            }
        }

        private TextView MakeText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.MakeText);
            }
        }

        private TextView ModelText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.ModelText);
            }
        }

        private TextView EngineText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.EngineText);
            }
        }

        IMainViewModel ViewModel { get; set; }

        protected override async void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            StartScanningButton.Click += StartScanningButton_Click;
            RefreshVehicleButton.Click += RefreshVehicleButton_Click;

            ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();

            ViewModel.GetDiagnosticDeviceComplete += this.GetDiagnosticDeviceComplete;
            ViewModel.LoadVehicleDetailsComplete += this.LoadVehicleDetailsComplete;
            ViewModel.LoadVehicleDetailsOptionsRequired += this.PromptVehicleOptions;


            await ViewModel.GetDiagnosticDevice();

        }

        private async void RefreshVehicleButton_Click(object sender, EventArgs e)
        {
            //better way to handle async?
            await ViewModel.LoadVehicleDetails(true);
        }

        private void StartScanningButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(LiveMileageActivity));
            StartActivity(intent);
        }

        private async void PromptVehicleOptions(LoadVehicleDetailsOptionRequiredResponse vehicleResponse)
        {
            UpdateVehicleDetails();

            var menu = new PopupMenu(this, EngineText);

            menu.Inflate(Resource.Menu.default_menu);

            foreach (var option in vehicleResponse.VehicleOptions)
            {
                menu.Menu.Add(new Java.Lang.String(option.Text));
            }

            menu.MenuItemClick += async (s1, arg1) =>
            {
                var matched = vehicleResponse.VehicleOptions.Where(v => arg1.Item.TitleFormatted.ToString() == v.Text).Single();

                await ViewModel.CompleteVehicleDetails(matched);
            };

            menu.DismissEvent += async (s2, arg2) =>
            {                
                //await ViewModel.ContinueWithoutVehicleDetails();
            };

            menu.Show();
        }

        private async void GetDiagnosticDeviceComplete(GetDiagnosticDeviceResponse deviceResponse)
        {
            if (!deviceResponse.Success)
            {
                throw new NotImplementedException("Need to handle connection failure!");
            }

            ConnectingLayout.Visibility = Android.Views.ViewStates.Gone;
            VehicleInfoLayout.Visibility = Android.Views.ViewStates.Visible;

            //TODO: handle real re-load
            await ViewModel.LoadVehicleDetails(false);
        }

        private async void LoadVehicleDetailsComplete(LoadVehicleDetailsCompleteResponse vehicleResponse)
        {

            if (!vehicleResponse.Success)
            {
                throw new NotImplementedException("need to handle vehicle load failure!");
            }

            UpdateVehicleDetails();
            EngineText.Text = ViewModel.CurrentVehicle.Option;
            StartScanningButton.Enabled = true;
        }

        private void UpdateVehicleDetails()
        {
            YearText.Text = ViewModel.CurrentVehicle.Year.ToString();
            MakeText.Text = ViewModel.CurrentVehicle.Make;
            ModelText.Text = ViewModel.CurrentVehicle.Model;
        }
    }
}

