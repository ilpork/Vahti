using Microsoft.Maui.Platform;
using Microsoft.Maui.Controls.Platform;
#if ANDROID
using Android.Graphics;
using Android.Widget;
#endif

namespace Vahti.Mobile.Forms.Effects
{
    /// <summary>
    /// Effect for changing line color of EditText view
    /// </summary>
    public class LineColorEffect : RoutingEffect
    {
        public Microsoft.Maui.Graphics.Color Color { get; set; }

        public LineColorEffect() : base($"{EffectConstants.GroupName}.{EffectConstants.EditTextLineColorEffectName}")
        {

        }
    }

    public class EditTextLineColorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {
#if ANDROID
            var effect = (LineColorEffect)Element.Effects.FirstOrDefault(e => e is LineColorEffect);
            if (effect != null)
            {
                ((EditText)Control).Background.Mutate().SetColorFilter(effect.Color.ToPlatform(), PorterDuff.Mode.SrcAtop);                    
            }        
#endif
        }

        protected override void OnDetached()
        {
        }
    }
}
        
