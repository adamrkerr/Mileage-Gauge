using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Support.V4.Content;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.DI;
using Autofac;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ResponseModels;

namespace MileageGauge
{
    [Activity(Label = "AddVehicleActivity")]
    public class AddVehicleActivity : Activity
    {
        private ImageButton ScanButton
        {
            get
            {
                return FindViewById<ImageButton>(Resource.Id.ScanButton);
            }
        }

        private ImageButton RetrieveButton
        {
            get
            {
                return FindViewById<ImageButton>(Resource.Id.RetrieveButton);
            }
        }

        private LinearLayout VehicleInfoLayout
        {
            get
            {
                return FindViewById<LinearLayout>(Resource.Id.VehicleInfoLayout);
            }
        }

        private Button RefreshVehicleButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.RefreshVehicleButton);
            }
        }

        private Button AddVehicleButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.AddVehicleButton);
            }
        }

        private Button CancelButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.CancelButton);
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

        private EditText VinEntryText
        {
            get
            {
                return FindViewById<EditText>(Resource.Id.VinEntryText);
            }
        }

        private bool VehicleDetailsComplete { get; set; }

        IAddVehicleViewModel ViewModel { get; set; }

        public AddVehicleActivity()
        {
            VehicleDetailsComplete = false;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.AddVehicle);

            ViewModel = ContainerManager.Container.Resolve<IAddVehicleViewModel>();

            ScanButton.Click += ScanButton_Click;
            RetrieveButton.Click += RetrieveButton_Click;
            AddVehicleButton.Click += AddVehicleButton_Click;
            CancelButton.Click += CancelButton_Click;

            //Do this here, because it does not always restore
            if (savedInstanceState != null)
            {
                RestoreValues(savedInstanceState);
            }
        }

        protected async override void OnResume()
        {
            base.OnResume();

            ViewModel.LoadVehicleDetailsComplete = this.LoadVehicleDetailsComplete;
            ViewModel.LoadVehicleDetailsOptionsRequired = this.PromptVehicleOptions;
            ViewModel.LoadVehicleDetailsModelRequired = this.PromptVehicleModels;

            await Task.Delay(10); //return control to UI?

            //TODO: handle real re-load
            if (VehicleDetailsComplete)            
            {
                LoadVehicleDetailsComplete(new LoadVehicleDetailsCompleteResponse { Success = true });
            }

        }

        private const string VEHICLE_DETAILS_COMPLETE = "VEHICLE_DETAILS_COMPLETE";

        protected override void OnSaveInstanceState(Bundle outState)
        {
            //outState.PutString(CURRENT_DEVICE_ADDRESS, CurrentDeviceAddress);
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
            //CurrentDeviceAddress = savedInstanceState.GetString(CURRENT_DEVICE_ADDRESS);
            VehicleDetailsComplete = savedInstanceState.GetBoolean(VEHICLE_DETAILS_COMPLETE);
        }

        private async void RefreshVehicleButton_Click(object sender, EventArgs e)
        {
            YearText.Text = "(year)";
            MakeText.Text = "(make)";
            ModelText.Text = "(model)";
            EngineText.Text = "(engine)";
            VehicleDetailsComplete = false;

            if (!String.IsNullOrEmpty(VinEntryText.Text))
            {
                //better way to handle async?
                await ViewModel.LoadVehicleDetailsFromVin(VinEntryText.Text);
            }
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

        private void LoadVehicleDetailsComplete(LoadVehicleDetailsCompleteResponse vehicleResponse)
        {
            if (!vehicleResponse.Success)
            {
                throw new NotImplementedException("need to handle vehicle load failure!");
            }

            UpdateVehicleDetails();
            EngineText.Text = ViewModel.CurrentVehicle.Option;
            VehicleDetailsComplete = true;
        }

        private void UpdateVehicleDetails()
        {
            if (String.IsNullOrEmpty(VinEntryText.Text))
            {
                VinEntryText.Text = ViewModel.CurrentVehicle.VIN;
            }

            YearText.Text = ViewModel.CurrentVehicle.Year.ToString();
            MakeText.Text = ViewModel.CurrentVehicle.Make;
            ModelText.Text = ViewModel.CurrentVehicle.Model;
        }

        private void RetrieveButton_Click(object sender, EventArgs e)
        {

            VehicleDetailsComplete = false;

            var vin = VinEntryText.Text;

            ViewModel.LoadVehicleDetailsFromVin(vin);
        }

        private void ScanButton_Click(object sender, EventArgs e)
        {

            VehicleDetailsComplete = false;

            ViewModel.LoadVehicleDetailsFromDevice();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(VehicleSelectionActivity));
            StartActivity(intent);
        }

        private async void AddVehicleButton_Click(object sender, EventArgs e)
        {
            if (!VehicleDetailsComplete)
            {
                return;
            }

            var added = await ViewModel.AddVehicleToCollection();

            if (!added.Success)
            {
                throw new Exception(added.Message);
            }
            else
            {
                var intent = new Intent(this, typeof(VehicleSelectionActivity));
                StartActivity(intent);
            }

        }

    }
}