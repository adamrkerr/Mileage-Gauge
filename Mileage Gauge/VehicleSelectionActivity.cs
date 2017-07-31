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
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.DI;
using System.Threading.Tasks;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using Autofac;
using Android.Support.V7.Widget;
using MileageGauge.Adapters;
using Android.Support.V7.App;

namespace MileageGauge
{
    [Activity(Label = "VehicleSelectionActivity", Theme = "@style/Theme.AppCompat")]
    public class VehicleSelectionActivity : AppCompatActivity
    {
        private Button AddNewVehicleButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.AddNewVehicleButton);
            }
        }

        private RecyclerView VehicleRecyclerView
        {
            get
            {
                return FindViewById<RecyclerView>(Resource.Id.VehicleRecyclerView);
            }
        }
       

        IMainViewModel MainViewModel { get; set; }

         protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.VehicleSelection);

            var layoutManager = new LinearLayoutManager(this);
            VehicleRecyclerView.SetLayoutManager(layoutManager);
            
            AddNewVehicleButton.Click += AddNewVehicleButton_Click;

            MainViewModel = ContainerManager.Container.Resolve<IMainViewModel>();

            //Do this here, because it does not always restore
            if (savedInstanceState != null)
            {
                RestoreValues(savedInstanceState);
            }
        }

        protected async override void OnResume()
        {
            base.OnResume();

            await LoadVehicleHistory();
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
        
        private void AddNewVehicleButton_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(AddVehicleActivity));
            StartActivity(intent);
        }

        private void LaunchMileageActivity(VehicleModel vehicle)
        {
            MainViewModel.SetCurrentVehicle(vehicle);

            var intent = new Intent(this, typeof(LiveMileageActivity));
            StartActivity(intent);
        }

        private async void DeleteVehicle(VehicleModel vehicle)
        {
            await MainViewModel.RemoveVehicle(vehicle.VIN);

            await LoadVehicleHistory();
        }

        private void LaunchDiagnosticActivity(VehicleModel vehicle)
        {
            MainViewModel.SetCurrentVehicle(vehicle);

            var intent = new Intent(this, typeof(DiagnosticActivity));
            StartActivity(intent);
        }

        private async Task LoadVehicleHistory()
        {
            var vehicles = (await MainViewModel.GetVehicleHistory()).OrderByDescending(v => v.LastSelected);
            // specify an adapter
            var adapter = new VehicleHistoryAdapter(vehicles);
            adapter.ItemClick += (s1, arg1) =>
            {
                LaunchMileageActivity(arg1.ViewModel);
            };

            adapter.DeleteRequest += (s1, arg1) =>
            {
                DeleteVehicle(arg1.ViewModel);
            };

            adapter.MileageRequest += (s1, arg1) =>
            {
                LaunchMileageActivity(arg1.ViewModel);
            };

            adapter.DiagnosticRequest += (s1, arg1) =>
            {
                LaunchDiagnosticActivity(arg1.ViewModel);
            };

            VehicleRecyclerView.SetAdapter(adapter);
        }

    }
}