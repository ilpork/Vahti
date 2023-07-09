﻿using Android.Graphics;
using Android.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.Platform;
using Vahti.Mobile.Forms.Effects;

[assembly: ResolutionGroupName(EffectConstants.GroupName)]
[assembly: ExportEffect(typeof(Vahti.Mobile.Droid.Effects.EditTextLineColorEffect), EffectConstants.EditTextLineColorEffectName)]
namespace Vahti.Mobile.Droid.Effects
{
    /// <summary>
    /// Effect to set the line color of <see cref="EditText"/>
    /// </summary>
    public class EditTextLineColorEffect : PlatformEffect
    {
        protected override void OnAttached()
        {            
            var effect = (LineColorEffect)Element.Effects.FirstOrDefault(e => e is LineColorEffect);
            if (effect != null)
            {
                ((EditText)Control).Background.Mutate().SetColorFilter(effect.Color.ToAndroid(), PorterDuff.Mode.SrcAtop);                    
            }        
        }

        protected override void OnDetached()
        {
        }
    }
}