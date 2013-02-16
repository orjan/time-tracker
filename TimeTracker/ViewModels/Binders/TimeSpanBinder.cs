using System;
using System.Globalization;
using System.Web.Mvc;

namespace TimeTracker.ViewModels.Binders
{
    public class TimeSpanBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            // Ensure there's incomming data
            string key = bindingContext.ModelName;
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(key);

            if (valueProviderResult == null || string.IsNullOrEmpty(valueProviderResult.AttemptedValue))
            {
                return null;
            }

            // Preserve it in case we need to redisplay the form
            bindingContext.ModelState.SetModelValue(key, valueProviderResult);

            return TimeSpan.ParseExact(valueProviderResult.AttemptedValue, "hhmm", CultureInfo.InvariantCulture);
        }
    }
}