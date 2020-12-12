using ei8.Cortex.Library.Common;
using Microsoft.AspNetCore.Components.Forms;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.Shared
{
    public class NullableRelativeTypeSelect : InputSelect<RelativeType?>
    {
        protected override bool TryParseValueFromString(string value, out RelativeType? result, out string validationErrorMessage)
        {
            if (string.IsNullOrWhiteSpace(value))
                result = null;
            else if (Enum.TryParse<RelativeType>(value, out RelativeType itemVal))
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

        protected override string FormatValueAsString(RelativeType? value)
        {
            if (!value.HasValue)
                return "";
            else
                return value.Value.ToString();
        }
    }
}
