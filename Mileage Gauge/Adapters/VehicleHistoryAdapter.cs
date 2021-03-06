﻿using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using MileageGauge.CSharp.Abstractions.ViewModels;
using System.Collections.Generic;
using System.Linq;
using Android.Support.V7.Content.Res;

namespace MileageGauge.Adapters
{
    class VehicleHistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<VehicleHistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<VehicleHistoryAdapterClickEventArgs> MileageRequest;
        public event EventHandler<VehicleHistoryAdapterClickEventArgs> DiagnosticRequest;
        public event EventHandler<VehicleHistoryAdapterClickEventArgs> DeleteRequest;

        List<VehicleModel> items;

        public VehicleHistoryAdapter(IEnumerable<VehicleModel> data)
        {
            items = data.ToList();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            //TODO: Unique layout
            var id = Resource.Layout.VehicleHistoryItem;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new VehicleHistoryAdapterViewHolder(itemView, OnClick, OnMileageRequest, OnDiagnosticRequest, OnDeleteRequest);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as VehicleHistoryAdapterViewHolder;
            holder.VehicleText.Text = item.Description;
            holder.VehicleVin.Text = item.VIN;
            holder.VehicleMpg.Text = $"{item.CityMPG} / {item.CombinedMPG} / {item.HighwayMPG}";
            holder.VehicleEngine.Text = item.Option;

            holder.ViewModel = item;
        }

        public override int ItemCount => items.Count();

        void OnClick(VehicleHistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnMileageRequest(VehicleHistoryAdapterClickEventArgs args) => MileageRequest?.Invoke(this, args);

        void OnDiagnosticRequest(VehicleHistoryAdapterClickEventArgs args) => DiagnosticRequest?.Invoke(this, args);

        void OnDeleteRequest(VehicleHistoryAdapterClickEventArgs args) => DeleteRequest?.Invoke(this, args);


    }

    public class VehicleHistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        private Action<VehicleHistoryAdapterClickEventArgs> onClick;
        private EventHandler<VehicleHistoryAdapterClickEventArgs> mileageRequest;
        private EventHandler<VehicleHistoryAdapterClickEventArgs> diagnosticRequest;
        private EventHandler<VehicleHistoryAdapterClickEventArgs> deleteRequest;

        public TextView VehicleText { get; set; }

        public TextView VehicleMpg { get; set; }
        public TextView VehicleVin { get; set; }
        public TextView VehicleEngine { get; set; }

        public VehicleModel ViewModel { get; set; }

        ImageButton MileageButton { get; set; }

        ImageButton DiagnosticButton { get; set; }

        ImageButton DeleteButton { get; set; }

        LinearLayout OptionsLayout { get; set; }


        public VehicleHistoryAdapterViewHolder(View itemView, Action<VehicleHistoryAdapterClickEventArgs> onClick, Action<VehicleHistoryAdapterClickEventArgs> mileageRequest,
            Action<VehicleHistoryAdapterClickEventArgs> diagnosticRequest, Action<VehicleHistoryAdapterClickEventArgs> deleteRequest) : base(itemView)
        {
            VehicleText = itemView.FindViewById<TextView>(Resource.Id.VehicleDescription);

            VehicleMpg = itemView.FindViewById<TextView>(Resource.Id.MileageText);
            VehicleVin = itemView.FindViewById<TextView>(Resource.Id.VinText);
            VehicleEngine = itemView.FindViewById<TextView>(Resource.Id.EngineText);

            MileageButton = itemView.FindViewById<ImageButton>(Resource.Id.MileageButton);
            DiagnosticButton = itemView.FindViewById<ImageButton>(Resource.Id.DiagnosticButton);
            DeleteButton = itemView.FindViewById<ImageButton>(Resource.Id.DeleteButton);
            OptionsLayout = itemView.FindViewById<LinearLayout>(Resource.Id.OptionsLayout);


            itemView.Click += (sender, e) =>
            {
                itemView.Background = AppCompatResources.GetDrawable(itemView.Context, Resource.Color.background_floating_material_dark);
                onClick(new VehicleHistoryAdapterClickEventArgs { ViewModel = ViewModel });
            };

            itemView.LongClick += ItemView_LongClick;
            MileageButton.Click += (sender, e) => mileageRequest(new VehicleHistoryAdapterClickEventArgs { ViewModel = ViewModel });
            DiagnosticButton.Click += (sender, e) => diagnosticRequest(new VehicleHistoryAdapterClickEventArgs { ViewModel = ViewModel });
            DeleteButton.LongClick += (sender, e) => deleteRequest(new VehicleHistoryAdapterClickEventArgs { ViewModel = ViewModel });
        }


        private void ItemView_LongClick(object sender, View.LongClickEventArgs e)
        {
            if (OptionsLayout.Visibility != ViewStates.Gone)
            {
                OptionsLayout.Visibility = ViewStates.Gone;
            }
            else
            {
                OptionsLayout.Visibility = ViewStates.Visible;
            }
        }
    }

    public class VehicleHistoryAdapterClickEventArgs : EventArgs
    {
        public VehicleModel ViewModel { get; set; }
    }
}