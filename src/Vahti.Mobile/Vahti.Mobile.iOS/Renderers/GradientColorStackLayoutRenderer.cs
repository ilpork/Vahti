using CoreAnimation;
using CoreGraphics;
using Vahti.Mobile.Forms.Views.Custom;
using Vahti.Mobile.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(GradientColorStackLayout), typeof(GradientColorStackLayoutRenderer))]

namespace Vahti.Mobile.iOS.Renderers
{
    public class GradientColorStackLayoutRenderer : VisualElementRenderer<StackLayout>
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            GradientColorStackLayout stack = (GradientColorStackLayout)this.Element;
            CGColor startColor = stack.StartColor.ToCGColor();

            CGColor endColor = stack.EndColor.ToCGColor();

            var gradientLayer = new CAGradientLayer();
            gradientLayer.Frame = rect;
            gradientLayer.Colors = new CGColor[] { startColor, endColor
        };

            NativeView.Layer.InsertSublayer(gradientLayer, 0);
        }
    }
}
