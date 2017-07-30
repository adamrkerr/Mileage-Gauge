using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using MileageGauge.CSharp.Abstractions.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace MileageGauge.Adapters
{
    class VehicleHistoryAdapter : RecyclerView.Adapter
    {
        public event EventHandler<VehicleHistoryAdapterClickEventArgs> ItemClick;
        public event EventHandler<VehicleHistoryAdapterClickEventArgs> ItemLongClick;
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
            var id = Resource.Layout.BluetoothDeviceLayout;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new VehicleHistoryAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as VehicleHistoryAdapterViewHolder;
            holder.VehicleText.Text = $"{item.Year} {item.Make} {item.Model}";
            holder.ViewModel = item;
        }

        public override int ItemCount => items.Count();

        void OnClick(VehicleHistoryAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(VehicleHistoryAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class VehicleHistoryAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView VehicleText { get; set; }

        public VehicleModel ViewModel { get; set; }


        public VehicleHistoryAdapterViewHolder(View itemView, Action<VehicleHistoryAdapterClickEventArgs> clickListener,
                            Action<VehicleHistoryAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            //TODO: fix when we have a unique layout
            VehicleText = itemView.FindViewById<TextView>(Resource.Id.DeviceName);
            itemView.Click += (sender, e) => clickListener(new VehicleHistoryAdapterClickEventArgs { ViewModel = ViewModel });
            itemView.LongClick += (sender, e) => longClickListener(new VehicleHistoryAdapterClickEventArgs { ViewModel = ViewModel });
        }
    }

    public class VehicleHistoryAdapterClickEventArgs : EventArgs
    {
        public VehicleModel ViewModel { get; set; }
    }
}