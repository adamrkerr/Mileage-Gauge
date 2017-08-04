using System;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Widget;
using MileageGauge.DI;
using Autofac;
using MileageGauge.CSharp.Abstractions.ViewModels;
using MileageGauge.CSharp.Abstractions.ResponseModels;
using Android.Support.V7.App;
using Android.Support.V7.Content.Res;
using Android.Graphics.Drawables;
using Android.Views;

namespace MileageGauge
{
    [Activity(Label = "Mileage Gauge: Average MPG", Theme = "@style/Theme.AppCompat")]
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

        private TextView TotalGallons
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.TotalGallons);
            }
        }

        private TextView TotalMiles
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.TotalMiles);
            }
        }

        private TextView MPHText
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.MPHText);
            }
        }

        private TextView DangerToManifold
        {
            get
            {
                return FindViewById<TextView>(Resource.Id.DangerToManifold);
            }
        }

        private Button MileageBack
        {
            get
            {
                return FindViewById<Button>(Resource.Id.BackButton);
            }
        }

        private ImageButton ResetButton
        {
            get
            {
                return FindViewById<ImageButton>(Resource.Id.ResetButton);
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

            DangerToManifold.Background = this.GetDrawable(Resource.Drawable.danger_to_manifold_bg); 

            MileageBack.Click += MileageBack_Click;

            ResetButton.LongClick += ResetButton_Click;

        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            MonitorViewModel?.RequestTripReset();
        }

        protected async override void OnResume()
        {
            base.OnResume();

            MonitorViewModel = ContainerManager.Container.Resolve<IMPGMonitorViewModel>();

            MonitorViewModel.UpdateMPG = MonitorViewModel_UpdateMPG;

            await MonitorViewModel.BeginMonitoringMPG(ViewModel.CurrentVehicle.VIN);

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
            //if (MonitorViewModel != null)
            //{
            //    MonitorViewModel.UpdateMPG = null;

            //    MonitorViewModel.EndMonitoringMPG().Wait();
            //}
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

                var gallonString = "0";
                if(response.GallonsUsed < 10)
                {
                    gallonString = response.GallonsUsed.ToString("0.000");
                }
                else if (response.GallonsUsed < 100)
                {
                    gallonString = response.GallonsUsed.ToString("#.00");
                }
                else{
                    gallonString = response.GallonsUsed.ToString("#.0");
                }

                TotalGallons.Text = gallonString;

                var milesString = "0";

                if(response.MilesTravelled < 10)
                {
                    milesString = response.MilesTravelled.ToString("0.00");
                }
                else if(response.MilesTravelled < 100)
                {
                    milesString = response.MilesTravelled.ToString("#.0");
                }
                else
                {
                    milesString = response.MilesTravelled.ToString("#");
                }

                TotalMiles.Text = milesString;

                //Show "DANGER TO MANIFOLD" when driver floors it! xD
                if(DangerToManifold.Visibility == ViewStates.Invisible && response.CurrentThrottlePercentage >= 70)
                {
                    DangerToManifold.Visibility = ViewStates.Visible;
                    var animationDrawable = (AnimationDrawable)DangerToManifold.Background;
                    animationDrawable.Start();
                }
                else if (DangerToManifold.Visibility == ViewStates.Visible && response.CurrentThrottlePercentage < 70)
                {
                    DangerToManifold.Visibility = ViewStates.Invisible;
                    var animationDrawable = (AnimationDrawable)DangerToManifold.Background;
                    animationDrawable.Stop();
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