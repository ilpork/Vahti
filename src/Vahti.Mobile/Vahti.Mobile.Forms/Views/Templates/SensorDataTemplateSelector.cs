using Vahti.Mobile.Forms.Models;
using Xamarin.Forms;

namespace Vahti.Mobile.Forms.Views.Templates
{
    /// <summary>
    /// Select <see cref="DataTemplate"/> for <see cref="Measurement"/>
    /// </summary>
    public class MeasurementDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public DataTemplate MowerTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {           
            if (((Measurement)item).SensorClass == Shared.Enum.SensorClass.LastCharged)
            {
                return MowerTemplate;
            }
            return DefaultTemplate;
        }
    }
}
