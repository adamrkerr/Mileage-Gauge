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

namespace MileageGauge
{
    [Activity(Label = "LiveMileageActivity")]
    public class LiveMileageActivity : Activity
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
                return FindViewById<Button>(Resource.Id.MileageBack);
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

            VehicleText.Text = $"{ViewModel.CurrentVehicle.Make} {ViewModel.CurrentVehicle.Model}";

            CityText.Text = ViewModel.CurrentVehicle.CityMPG.ToString();
            CombinedText.Text = ViewModel.CurrentVehicle.CombinedMPG.ToString();
            HighwayText.Text = ViewModel.CurrentVehicle.HighwayMPG.ToString();

            MileageBack.Click += MileageBack_Click;

            MonitorViewModel = ContainerManager.Container.Resolve<IMPGMonitorViewModel>();

            MonitorViewModel.UpdateMPG += MonitorViewModel_UpdateMPG;

            await MonitorViewModel.BeginMonitoringMPG();
        }

        protected async override void OnStop()
        {
            base.OnStop();

            await MonitorViewModel.EndMonitoringMPG();
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
            var intent = new Intent(this, typeof(MainActivity));
            StartActivity(intent);
        }
    }
}