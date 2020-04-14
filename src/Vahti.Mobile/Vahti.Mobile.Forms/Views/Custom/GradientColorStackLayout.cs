using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Views.Custom
{
    /// <summary>
    /// <see cref="StackLayout"/> with gradient background color
    /// </summary>
    public class GradientColorStackLayout : StackLayout
    {
        public static readonly BindableProperty StartColorProperty = BindableProperty.Create(nameof(StartColor), typeof(Color), typeof(GradientColorStackLayout));
        public static readonly BindableProperty EndColorProperty = BindableProperty.Create(nameof(EndColor), typeof(Color), typeof(GradientColorStackLayout));

        public Color StartColor
        {
            get { return (Color)GetValue(StartColorProperty); }
            set { SetValue(StartColorProperty, value); }
        }

        public Color EndColor
        {
            get { return (Color)GetValue(EndColorProperty); }
            set { SetValue(EndColorProperty, value); }
        }
        
    }
}
