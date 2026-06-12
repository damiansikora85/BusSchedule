using Microsoft.Maui.Controls;

namespace BusSchedule.Core.UI.ViewModels
{
    public class RouteStopTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }
        public DataTemplate OptionalTemplate { get; set; }
        public DataTemplate OptionalToOptionalTemplate { get; set; }
        public DataTemplate DefaultToOptionalTemplate { get; set; }
        public DataTemplate OptionalToDefaultTemplate { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (item is RouteStopViewModel vm)
            {
                return vm.StopType switch
                {
                    RouteStopViewModel.RouteStopType.OptionalFirst => OptionalToOptionalTemplate,
                    RouteStopViewModel.RouteStopType.OptionalLast => OptionalToDefaultTemplate,
                    RouteStopViewModel.RouteStopType.Optional => DefaultToOptionalTemplate,
                    _ => vm.IsOptional ? OptionalTemplate : DefaultTemplate,
                };
            }

            return DefaultTemplate;
        }
    }
}