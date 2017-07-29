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

        private ImageButton RunButton
        {
            get
            {
                return FindViewById<ImageButton>(Resource.Id.RunButton);
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

        private bool VehicleDetailsComplete { get; set; }

        IMainViewModel ViewModel { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.AddVehicleLayout);

            //ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();

            //Do this here, because it does not always restore
            if (savedInstanceState != null)
            {
                //RestoreValues(savedInstanceState);
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            //if (Build.VERSION.SdkInt >= BuildVersionCodes.Lollipop)
            //{
            //    // Android 5.0 and higher can load the SVG image from resources.

            //var scanIcon = ContextCompat.GetDrawable(ApplicationContext, Resource.Drawable.ic_settings_remote_black_24px);
            //ScanButton.SetImageDrawable(scanIcon);
            //}

            //var runIcon = ContextCompat.GetDrawable(ApplicationContext, Resource.Drawable.ic_check_black_24px);
            //RunButton.SetImageDrawable(runIcon);

        }
    }
}