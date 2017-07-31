using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.DI;
using Autofac;
using Android.Support.V7.Widget;
using Android.Support.V7.App;
using System.Threading.Tasks;
using MileageGauge.Adapters;

namespace MileageGauge
{
    [Activity(Label = "Mileage Gauge: Read Diagnostic Codes", Theme = "@style/Theme.AppCompat")]
    public class DiagnosticActivity : AppCompatActivity
    {
        private TextView VehicleText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.VehicleText);
            }
        }

        private TextView AllClearText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.AllClearText);
            }
        }

        private Button DiagnosticBack
        {
            get
            {
                return FindViewById<Button>(Resource.Id.BackButton);
            }
        }

        private Button ClearCodeButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.ClearCodeButton);
            }
        }

        private Button RefreshButton
        {
            get
            {
                return FindViewById<Button>(Resource.Id.RefreshButton);
            }
        }

        private RecyclerView DiagnosticCodeRecyclerView
        {
            get
            {
                return FindViewById<RecyclerView>(Resource.Id.DiagnosticCodeRecyclerView);
            }
        }

        IMainViewModel ViewModel { get; set; }

        IDiagnosticViewModel DiagnosticModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Diagnostic);

            var layoutManager = new LinearLayoutManager(this);
            DiagnosticCodeRecyclerView.SetLayoutManager(layoutManager);

            DiagnosticBack.Click += DiagnosticBack_Click;

            RefreshButton.Click += RefreshButton_Click;

            ClearCodeButton.Click += ClearCodeButton_Click;

            ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();
            DiagnosticModel = ContainerManager.Container.Resolve<IDiagnosticViewModel>();

            VehicleText.Text = ViewModel.CurrentVehicle.Description;

            //Do this here, because it does not always restore
            if (savedInstanceState != null)
            {
                RestoreValues(savedInstanceState);
            }
        }

        protected async override void OnResume()
        {
            base.OnResume();

            await LoadDiagnosticCodes();
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
        
        private async Task LoadDiagnosticCodes()
        {
            var codes = await DiagnosticModel.GetDiagnosticCodes();

            if(codes.Count() > 0)
            {
                DiagnosticCodeRecyclerView.Visibility = ViewStates.Visible;
                AllClearText.Visibility = ViewStates.Gone;
            }
            else
            {
                DiagnosticCodeRecyclerView.Visibility = ViewStates.Gone;
                AllClearText.Visibility = ViewStates.Visible;
            }

            // specify an adapter
            var adapter = new DiagnosticCodeAdapter(codes);
            adapter.ItemClick += (s1, arg1) =>
            {
                LaunchSearch(arg1.Code);
            };

            DiagnosticCodeRecyclerView.SetAdapter(adapter);

        }

        private void LaunchSearch(string code)
        {
            var intent = new Intent(Intent.ActionWebSearch);
            intent.PutExtra(SearchManager.Query, $"{ViewModel.CurrentVehicle.Year} {ViewModel.CurrentVehicle.Make} cel {code}");
            StartActivity(intent);
        }

        private async void ClearCodeButton_Click(object sender, EventArgs e)
        {
            await DiagnosticModel.ClearDiagnosticCodes();
            await LoadDiagnosticCodes();
        }

        private async void RefreshButton_Click(object sender, EventArgs e)
        {
            await LoadDiagnosticCodes();
        }

        private void DiagnosticBack_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(VehicleSelectionActivity));
            StartActivity(intent);
        }
    }


}