using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Effects
{
    /// <summary>
    /// Effect for changing line color of EditText view
    /// </summary>
    public class LineColorEffect : RoutingEffect
    {
        public Color Color { get; set; }

        public LineColorEffect() : base($"{EffectConstants.GroupName}.{EffectConstants.EditTextLineColorEffectName}")
        {

        }
    }
}
