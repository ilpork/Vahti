using System;
using Android.Content;
using Vahti.Mobile.Droid.Renderers;
using Vahti.Mobile.Forms.Views.Custom;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(GradientColorStackLayout), typeof(GradientColorStackLayoutRenderer))]
namespace Vahti.Mobile.Droid.Renderers
{
    /// <summary>
    /// Provides renderer for <see cref="StackLayout"/> with gradient background color
    /// </summary>
    public class GradientColorStackLayoutRenderer : VisualElementRenderer<StackLayout>
    {            
        private GradientColorStackLayout _stack;

        public GradientColorStackLayoutRenderer(Context context) : base(context)
        {            
        }

        protected override void DispatchDraw(global::Android.Graphics.Canvas canvas)
        {     
            var gradient = new Android.Graphics.LinearGradient(0, 0, 0, Height,
                   _stack.StartColor.ToAndroid(),
                   _stack.EndColor.ToAndroid(),
                   Android.Graphics.Shader.TileMode.Mirror);

            var paint = new Android.Graphics.Paint()
            {
                Dither = true,
            };

            paint.SetShader(gradient);
            canvas.DrawPaint(paint);
            base.DispatchDraw(canvas);
        }
        
        protected override void OnElementChanged(ElementChangedEventArgs<StackLayout> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null || Element == null)
            {
                return;
            }
            try
            {
                _stack = e.NewElement as GradientColorStackLayout;                
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(@"ERROR:", ex.Message);
            }
        }
    }
}