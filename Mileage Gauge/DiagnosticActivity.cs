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
using Autofac;
using Android.Support.V7.Widget;
using Android.Support.V7.App;

namespace MileageGauge
{
    [Activity(Label = "DiagnosticActivity", Theme = "@style/Theme.AppCompat")]
    public class DiagnosticActivity : AppCompatActivity
    {
        private TextView VehicleText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.VehicleText);
            }
        }

        private Button DiagnosticBack
        {
            get
            {
                return FindViewById<Button>(Resource.Id.BackButton);
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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.Diagnostic);

            ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();

            VehicleText.Text = ViewModel.CurrentVehicle.Description;

            DiagnosticBack.Click += DiagnosticBack_Click;
        }

        private void DiagnosticBack_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(VehicleSelectionActivity));
            StartActivity(intent);
        }
    }


}