using System;

using Android.Views;
using Android.Widget;
using Android.Support.V7.Widget;
using System.Collections.Generic;
using System.Linq;

namespace MileageGauge.Adapters
{
    class DiagnosticCodeAdapter : RecyclerView.Adapter
    {
        public event EventHandler<DiagnosticCodeAdapterClickEventArgs> ItemClick;
        List<string> items;

        public DiagnosticCodeAdapter(IEnumerable<string> data)
        {
            items = data.ToList();
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.DiagnosticCodeItem;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new DiagnosticCodeAdapterViewHolder(itemView, OnClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            var item = items[position];

            // Replace the contents of the view with that element
            var holder = viewHolder as DiagnosticCodeAdapterViewHolder;
            holder.CodeText.Text = items[position];
        }

        public override int ItemCount => items.Count();

        void OnClick(DiagnosticCodeAdapterClickEventArgs args) => ItemClick?.Invoke(this, args);        

    }

    public class DiagnosticCodeAdapterViewHolder : RecyclerView.ViewHolder
    {
        public TextView CodeText { get; set; }

        public ImageButton SearchButton { get; set; }


        public DiagnosticCodeAdapterViewHolder(View itemView, Action<DiagnosticCodeAdapterClickEventArgs> clickListener) : base(itemView)
        {
            CodeText = itemView.FindViewById<TextView>(Resource.Id.CodeText);
            SearchButton = itemView.FindViewById<ImageButton>(Resource.Id.SearchButton);
            SearchButton.Click += (sender, e) => clickListener(new DiagnosticCodeAdapterClickEventArgs { Code = CodeText.Text });            
        }
    }

    public class DiagnosticCodeAdapterClickEventArgs : EventArgs
    {
        public string Code { get; set; }
    }
}