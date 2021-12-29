using Android.Content;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android.AppCompat;
using Microsoft.Maui.Controls.Platform;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace astator.Views;

internal class LabelbuttonRenderer : ButtonRenderer
{
    public LabelbuttonRenderer(Context context) : base(context)
    {
    }

    protected override void OnElementChanged(ElementChangedEventArgs<Button> e)
    {
        base.OnElementChanged(e);
        if (this.Control is not null)
        {
            this.Control.SetPadding(8, 15, 8, 15);
            this.Control.SetMinWidth(0);
            this.Control.SetMinHeight(0);
            this.Control.SetMinimumWidth(0);
            this.Control.SetMinimumHeight(0);
        }
    }
}
