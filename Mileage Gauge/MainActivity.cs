using Android.App;
using Android.Widget;
using Android.OS;
using MileageGauge.DI;
using Autofac;
using System;
using Android.Content;
using Android.Bluetooth;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using MileageGauge.CSharp.Abstractions.Services.ServiceResponses;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Android.Runtime;
using MileageGauge.CSharp.Abstractions.Models;

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

        private string CurrentDeviceAddress { get; set; }

        private bool VehicleDetailsComplete { get; set; }

        IMainViewModel ViewModel { get; set; }

        public MainActivity()
        {
            CurrentDeviceAddress = string.Empty;
            VehicleDetailsComplete = false;
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            StartScanningButton.Click += StartScanningButton_Click;
            RefreshVehicleButton.Click += RefreshVehicleButton_Click;

            ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();

            //Do this here, because it does not always restore
            if (bundle != null)
            {
                RestoreValues(bundle);
            }
        }

        protected async override void OnResume()
        {
            base.OnResume();

            ViewModel.GetDiagnosticDeviceComplete = this.GetDiagnosticDeviceComplete;
            ViewModel.LoadVehicleDetailsComplete = this.LoadVehicleDetailsComplete;
            ViewModel.LoadVehicleDetailsOptionsRequired = this.PromptVehicleOptions;
            ViewModel.LoadVehicleDetailsModelRequired = this.PromptVehicleModels;

            await Task.Delay(10); //return control to UI?

            if (String.IsNullOrEmpty(CurrentDeviceAddress))
            {
                //we need to connect to a device
                await ValidateBluetoothEnabled();
            }
            else
            {
                await ViewModel.GetDiagnosticDevice(CurrentDeviceAddress);
            }

        }

        private const string CURRENT_DEVICE_ADDRESS = "CURRENT_DEVICE_ADDRESS";
        private const string VEHICLE_DETAILS_COMPLETE = "VEHICLE_DETAILS_COMPLETE";

        protected override void OnSaveInstanceState(Bundle outState)
        {
            outState.PutString(CURRENT_DEVICE_ADDRESS, CurrentDeviceAddress);
            outState.PutBoolean(VEHICLE_DETAILS_COMPLETE, VehicleDetailsComplete);

            base.OnSaveInstanceState(outState);
        }

        public override void OnRestoreInstanceState(Bundle savedInstanceState, PersistableBundle persistentState)
        {
            RestoreValues(savedInstanceState);

            base.OnRestoreInstanceState(savedInstanceState, persistentState);
        }

        protected override void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            RestoreValues(savedInstanceState);

            base.OnRestoreInstanceState(savedInstanceState);
        }

        private void RestoreValues(Bundle savedInstanceState)
        {
            CurrentDeviceAddress = savedInstanceState.GetString(CURRENT_DEVICE_ADDRESS);
            VehicleDetailsComplete = savedInstanceState.GetBoolean(VEHICLE_DETAILS_COMPLETE);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);

            if (requestCode == BluetoothEnableRequest)
            {
                if (resultCode == Result.Ok)
                {
                    await FindBluetoothDeviceForConnection();
                }
                else
                {
                    //TODO problem!
                }
            }
        }

        private async Task FindBluetoothDeviceForConnection()
        {
            var adapter = BluetoothAdapter.DefaultAdapter;

            var pairedDevices = adapter.BondedDevices;

            IEnumerable<BluetoothDeviceModel> deviceCollection;

            if (pairedDevices == null)
            {
                deviceCollection = null;
            }
            else
            {
                //TODO: somehow detect if this is already connected to an appropriate device
                //Perhaps store device after successful connection

                deviceCollection = pairedDevices.Select(p => new BluetoothDeviceModel { Address = p.Address, Name = p.Name });
            }

            await PromptBluetoothOptions(deviceCollection);
        }

        private async Task PromptBluetoothOptions(IEnumerable<BluetoothDeviceModel> pairedDevices)
        {

            if (pairedDevices == null)
            {
                pairedDevices = new List<BluetoothDeviceModel>();
            }

            pairedDevices = pairedDevices.Append(BluetoothDeviceModel.GetDemoDevice());

            var menu = new PopupMenu(this, ConnectingLayout);

            menu.Inflate(Resource.Menu.default_menu);

            foreach (var option in pairedDevices)
            {
                menu.Menu.Add(new Java.Lang.String(option.Name));
            }

            menu.MenuItemClick += async (s1, arg1) =>
            {
                var matched = pairedDevices.Where(v => arg1.Item.TitleFormatted.ToString() == v.Name).Single();

                await ViewModel.GetDiagnosticDevice(matched.Address);
            };

            menu.DismissEvent += async (s2, arg2) =>
            {
                //await ViewModel.ContinueWithoutVehicleDetails();
            };

            try
            {

                menu.Show();
            }
            catch (Exception ex)
            {
                var test = ex;
            }
        }

        private const int BluetoothEnableRequest = 1;

        private async Task ValidateBluetoothEnabled()
        {
            var adapter = BluetoothAdapter.DefaultAdapter;

            if (adapter == null)
            {
                //bluetooth is not supported, just show the demo device
                await PromptBluetoothOptions(null);
            }
            else
            {
                if (!adapter.IsEnabled)
                {
                    var enableBtIntent = new Intent(BluetoothAdapter.ActionRequestEnable);

                    StartActivityForResult(enableBtIntent, BluetoothEnableRequest);
                }
                else
                {
                    await FindBluetoothDeviceForConnection();
                }
            }
        }

        private async void RefreshVehicleButton_Click(object sender, EventArgs e)
        {
            YearText.Text = "year";
            MakeText.Text = "make";
            ModelText.Text = "model";
            EngineText.Text = "engine";
            VehicleDetailsComplete = false;

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

            EngineText.Text = "Please select:";

            var menu = new PopupMenu(this, EngineText);

            menu.Inflate(Resource.Menu.default_menu);

            foreach (var option in vehicleResponse.VehicleOptions)
            {
                menu.Menu.Add(new Java.Lang.String(option.Text));
            }

            menu.MenuItemClick += async (s1, arg1) =>
            {
                var matched = vehicleResponse.VehicleOptions.Where(v => arg1.Item.TitleFormatted.ToString() == v.Text).Single();

                await ViewModel.CompleteVehicleOption(matched);
            };

            menu.DismissEvent += async (s2, arg2) =>
            {
                //await ViewModel.ContinueWithoutVehicleDetails();
            };

            menu.Show();
        }

        private async void PromptVehicleModels(LoadVehicleDetailsModelRequiredResponse vehicleResponse)
        {
            UpdateVehicleDetails();

            ModelText.Text = "Please select:";

            var menu = new PopupMenu(this, ModelText);

            menu.Inflate(Resource.Menu.default_menu);

            foreach (var option in vehicleResponse.ModelOptions)
            {
                menu.Menu.Add(new Java.Lang.String(option));
            }

            menu.MenuItemClick += async (s1, arg1) =>
            {
                await ViewModel.CompleteVehicleModel(arg1.Item.TitleFormatted.ToString());
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
            CurrentDeviceAddress = deviceResponse.DeviceAddress;

            //TODO: handle real re-load
            if (!VehicleDetailsComplete)
            {
                await ViewModel.LoadVehicleDetails(false);
            }
            else
            {
                LoadVehicleDetailsComplete(new LoadVehicleDetailsCompleteResponse { Success = true });
            }
        }

        private void LoadVehicleDetailsComplete(LoadVehicleDetailsCompleteResponse vehicleResponse)
        {

            if (!vehicleResponse.Success)
            {
                throw new NotImplementedException("need to handle vehicle load failure!");
            }

            UpdateVehicleDetails();
            EngineText.Text = ViewModel.CurrentVehicle.Option;
            VehicleDetailsComplete = true;
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

