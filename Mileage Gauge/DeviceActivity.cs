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
using Android.Support.V7.Widget;
using MileageGauge.Adapters;

namespace MileageGauge
{
    [Activity(Label = "Mileage Gauge", MainLauncher = true, Icon = "@drawable/icon")]
    public class DeviceActivity : Activity
    {
        private LinearLayout ConnectingLayout
        {
            get
            {
                return FindViewById<LinearLayout>(Resource.Id.ConnectingLayout);
            }
        }

        private LinearLayout DeviceSelectionLayout
        {
            get
            {
                return FindViewById<LinearLayout>(Resource.Id.DeviceSelectionLayout);
            }
        }

        private RecyclerView DeviceRecyclerView
        {
            get
            {
                return FindViewById<RecyclerView>(Resource.Id.DeviceRecyclerView);
            }
        }      
        
        IMainViewModel ViewModel { get; set; }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.DeviceSelection);

            var layoutManager = new LinearLayoutManager(this);
            DeviceRecyclerView.SetLayoutManager(layoutManager);

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

            await Task.Delay(10); //return control to UI?

            if (String.IsNullOrEmpty(ViewModel.DiagnosticDeviceAddress))
            {
                //we need to connect to a device
                await ValidateBluetoothEnabled();
            }
            else
            {
                await ViewModel.GetDiagnosticDevice(ViewModel.DiagnosticDeviceAddress);
            }

        }
        
        protected override void OnSaveInstanceState(Bundle outState)
        {
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

            PromptBluetoothOptions(deviceCollection);
        }

        private void PromptBluetoothOptions(IEnumerable<BluetoothDeviceModel> pairedDevices)
        {

            if (pairedDevices == null)
            {
                pairedDevices = new List<BluetoothDeviceModel>();
            }

            pairedDevices = pairedDevices.Append(BluetoothDeviceModel.GetDemoDevice());
            
            // specify an adapter
            var adapter = new BluetoothDeviceAdapter(pairedDevices);
            adapter.ItemClick += async (s1, arg1) =>
            {                
                await ViewModel.GetDiagnosticDevice(arg1.DeviceAddress);
            };

            DeviceRecyclerView.SetAdapter(adapter);

            ConnectingLayout.Visibility = Android.Views.ViewStates.Gone;
            DeviceSelectionLayout.Visibility = Android.Views.ViewStates.Visible;
        }

        private const int BluetoothEnableRequest = 1;

        private async Task ValidateBluetoothEnabled()
        {
            var adapter = BluetoothAdapter.DefaultAdapter;

            if (adapter == null)
            {
                //bluetooth is not supported, just show the demo device
                PromptBluetoothOptions(null);
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

        private void GetDiagnosticDeviceComplete(GetDiagnosticDeviceResponse deviceResponse)
        {
            if (!deviceResponse.Success)
            {
                throw new NotImplementedException("Need to handle connection failure!");
            }

            ConnectingLayout.Visibility = Android.Views.ViewStates.Gone;

            var intent = new Intent(this, typeof(VehicleSelectionActivity));
            //var intent = new Intent(this, typeof(AddVehicleActivity));
            StartActivity(intent);

        }

    }
}

