using CommonServiceLocator;
using Vahti.Mobile.Forms.ViewModels;

namespace MobileClient.ViewModels
{
    /// <summary>
    /// Helper class used to get reference to a view model
    /// </summary>
    public static class ViewModelLocator
    {
        static ViewModelLocator()
        {            
        }

        public static LocationListViewModel Items
        {
            get { return ServiceLocator.Current.GetInstance<LocationListViewModel>(); }
        }

        public static OptionsGeneralViewModel OptionsGeneral
        {
            get { return ServiceLocator.Current.GetInstance<OptionsGeneralViewModel>(); }
        }

        public static OptionsSummaryViewModel OptionsSummary
        {
            get { return ServiceLocator.Current.GetInstance<OptionsSummaryViewModel>(); }
        }

        public static OptionsSortingViewModel OptionsSorting
        {
            get { return ServiceLocator.Current.GetInstance<OptionsSortingViewModel>(); }
        }

        public static OptionsWidgetViewModel OptionsWidget
        {
            get { return ServiceLocator.Current.GetInstance<OptionsWidgetViewModel>(); }
        }

        public static AboutViewModel About
        {
            get { return ServiceLocator.Current.GetInstance<AboutViewModel>(); }
        }
        public static LocationDetailsViewModel Details
        {
            get { return ServiceLocator.Current.GetInstance<LocationDetailsViewModel>(); }
        }
        public static LocationGraphViewModel Graph
        {
            get { return ServiceLocator.Current.GetInstance<LocationGraphViewModel>(); }
        }
    }
}
