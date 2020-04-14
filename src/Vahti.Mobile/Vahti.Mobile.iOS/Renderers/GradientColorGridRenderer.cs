using CoreAnimation;
using CoreGraphics;
using Vahti.Mobile.Forms.Views.Custom;
using Vahti.Mobile.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
[assembly: ExportRenderer(typeof(GradientColorGrid), typeof(GradientColorGridRenderer))]

namespace Vahti.Mobile.iOS.Renderers
{
    public class GradientColorGridRenderer : VisualElementRenderer<Grid>
    {
        public override void Draw(CGRect rect)
        {
            base.Draw(rect);
            GradientColorGrid stack = (GradientColorGrid)this.Element;
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
