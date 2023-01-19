using Microsoft.AspNetCore.Components.Forms;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Common
{
    public class NullableInputSelect<T> : InputSelect<T?> where T : struct
    {
        protected override bool TryParseValueFromString(string value, out T? result, out string validationErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
                result = null;
            else if (Enum.TryParse<T>(value, out T itemVal))
                result = itemVal;
            else
            {
                validationErrorMessage = "Invalid Value";
                result = null;
                return false;
            }

            validationErrorMessage = null;
            return true;
        }

        protected override string FormatValueAsString(T? value)
        {
            if (!value.HasValue)
                return "";
            else
                return value.Value.ToString();
        }
    }
}
