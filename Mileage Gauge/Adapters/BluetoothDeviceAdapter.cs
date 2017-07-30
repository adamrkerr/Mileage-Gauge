using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using MileageGauge.CSharp.Abstractions.Models;
using System.Collections.Generic;
using System.Linq;

namespace MileageGauge.Adapters
{
    class BluetoothDeviceAdapter : RecyclerView.Adapter
    {
        public event EventHandler<BluetoothDeviceAdapterClickEventArgs> ItemClick;
        public event EventHandler<BluetoothDeviceAdapterClickEventArgs> ItemLongClick;
        List<BluetoothDeviceModel> items;

        public BluetoothDeviceAdapter(IEnumerable<BluetoothDeviceModel> data)
        {
            items = data.ToList();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.BluetoothDeviceItem;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new BluetoothDeviceAdapterViewHolder(itemView, OnClick, OnLongClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as BluetoothDeviceAdapterViewHolder;
            holder.DeviceName.Text = item.Name;
            holder.DeviceAddress = item.Address;
        }

        public override int ItemCount => items.Count();

        void OnClick(BluetoothDeviceAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnLongClick(BluetoothDeviceAdapterClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class BluetoothDeviceAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView DeviceName { get; set; }

        public string DeviceAddress { get; set; }

        public BluetoothDeviceAdapterViewHolder(View itemView, Action<BluetoothDeviceAdapterClickEventArgs> clickListener,
                            Action<BluetoothDeviceAdapterClickEventArgs> longClickListener) : base(itemView)
        {
            DeviceName = itemView.FindViewById<TextView>(Resource.Id.DeviceName);
            itemView.Click += (sender, e) => clickListener(new BluetoothDeviceAdapterClickEventArgs { DeviceName = DeviceName?.Text, DeviceAddress = DeviceAddress });
            itemView.LongClick += (sender, e) => longClickListener(new BluetoothDeviceAdapterClickEventArgs { DeviceName = DeviceName?.Text, DeviceAddress = DeviceAddress });
        }
    }

    public class BluetoothDeviceAdapterClickEventArgs : EventArgs
    {
        public string DeviceName { get; set; }
        public string DeviceAddress { get; set; }
    }
}