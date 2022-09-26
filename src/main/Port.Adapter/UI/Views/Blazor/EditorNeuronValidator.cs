using ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor.ViewModels;
using ei8.Cortex.Library.Common;
using FluentValidation;
using neurUL.Cortex.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public class EditorNeuronValidator : AbstractValidator<EditorNeuronViewModel>
    {
        public EditorNeuronValidator()
        {
            this.RuleFor(x => x.Tag).NotEmpty().WithMessage("Tag is required.");
        }
    }
}
