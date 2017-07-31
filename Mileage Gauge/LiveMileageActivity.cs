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
using MileageGauge.DI;
using Autofac;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using Android.Support.V7.App;

namespace MileageGauge
{
    [Activity(Label = "LiveMileageActivity", Theme = "@style/Theme.AppCompat")]
    public class LiveMileageActivity : AppCompatActivity
    {
        private TextView VehicleText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.VehicleText);
            }
        }

        private TextView HighwayText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.HighwayText);
            }
        }

        private TextView CombinedText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.CombinedText);
            }
        }

        private TextView CityText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.CityText);
            }
        }

        private TextView AverageMPGText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.AverageMPGText);
            }
        }

        private TextView InstantMPGText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.InstantMPGText);
            }
        }

        private TextView ThrottleText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.ThrottleText);
            }
        }

        private TextView MPHText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.MPHText);
            }
        }

        private Button MileageBack
        {
            get
            {
                return FindViewById<Button>(Resource.Id.BackButton);
            }
        }

        IMainViewModel ViewModel { get; set; }

        IMPGMonitorViewModel MonitorViewModel { get; set; }

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.LiveMileage);

            ViewModel = ContainerManager.Container.Resolve<IMainViewModel>();

            VehicleText.Text = ViewModel.CurrentVehicle.Description;
            CityText.Text = ViewModel.CurrentVehicle.CityMPG.ToString();
            CombinedText.Text = ViewModel.CurrentVehicle.CombinedMPG.ToString();
            HighwayText.Text = ViewModel.CurrentVehicle.HighwayMPG.ToString();
            
            MileageBack.Click += MileageBack_Click;

        }

        protected async override void OnResume()
        {
            base.OnResume();

            MonitorViewModel = ContainerManager.Container.Resolve<IMPGMonitorViewModel>();

            MonitorViewModel.UpdateMPG = MonitorViewModel_UpdateMPG;

            await MonitorViewModel.BeginMonitoringMPG();
        }

        protected override void OnPause()
        {
            base.OnStop();

            MonitorViewModel.UpdateMPG = null;

            MonitorViewModel.EndMonitoringMPG().Wait(); //do this synchronously
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            //this should already be done, but be safe in case of background termination
            if (MonitorViewModel != null)
            {
                MonitorViewModel.UpdateMPG = null;

                MonitorViewModel.EndMonitoringMPG().Wait();
            }
        }

        private void MonitorViewModel_UpdateMPG(MPGUpdateResponse response)
        {
            RunOnUiThread(() =>
            {
                InstantMPGText.Text = response.InstantMPG.ToString("##0.00");
                ThrottleText.Text = response.CurrentThrottlePercentage.ToString("##0");
                AverageMPGText.Text = response.AverageMPG.ToString("##0.00");
                MPHText.Text = response.CurrentMPH.ToString("##0");

                if (response.CurrentThrottlePercentage < 33)
                {
                    ThrottleText.SetTextColor(new Android.Graphics.Color(14,234,14));
                }
                else if (response.CurrentThrottlePercentage < 67)
                {
                    ThrottleText.SetTextColor(new Android.Graphics.Color(234,234,14));
                }
                else
                {
                    ThrottleText.SetTextColor(new Android.Graphics.Color(234,14,14));
                }

                if (response.InstantMPG < (ViewModel.CurrentVehicle.CombinedMPG * .33))
                {
                    InstantMPGText.SetTextColor(new Android.Graphics.Color(234, 14, 14));
                }
                else if (response.InstantMPG < (ViewModel.CurrentVehicle.CombinedMPG * .67))
                {
                    InstantMPGText.SetTextColor(new Android.Graphics.Color(234, 234, 14));
                }
                else
                {
                    InstantMPGText.SetTextColor(new Android.Graphics.Color(14, 234, 14));
                }

                if (response.AverageMPG < (ViewModel.CurrentVehicle.CombinedMPG * .33))
                {
                    AverageMPGText.SetTextColor(new Android.Graphics.Color(234, 14, 14));
                }
                else if (response.AverageMPG < (ViewModel.CurrentVehicle.CombinedMPG * .67))
                {
                    AverageMPGText.SetTextColor(new Android.Graphics.Color(234, 234, 14));
                }
                else
                {
                    AverageMPGText.SetTextColor(new Android.Graphics.Color(14, 234, 14));
                }
            });
        }

        private void MileageBack_Click(object sender, EventArgs e)
        {
            var intent = new Intent(this, typeof(VehicleSelectionActivity));
            StartActivity(intent);
        }
    }
}