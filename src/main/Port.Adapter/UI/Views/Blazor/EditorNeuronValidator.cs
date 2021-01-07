using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
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
            this.RuleFor(x => x.Tag).NotEmpty().When(x => x.SelectedOption != ContextMenuOption.LinkRelative).WithMessage("Tag is required.");

            this.RuleFor(x => x.Type).Must(x => !(x == RelativeType.NotSet || x == null)).When(x => !x.IsRoot).WithMessage("Type is required.");
            this.RuleFor(x => x.Effect).Must(x => !(x == NeurotransmitterEffect.NotSet || x == null)).When(x => !x.IsRoot).WithMessage("Effect is required.");
            this.RuleFor(x => x.Strength).NotEmpty().When(x => !x.IsRoot).WithMessage("Strength is required.");
            this.RuleFor(x => x.Strength).Must(s => s >= 0 && s <= 1).When(x => !x.IsRoot).WithMessage("Strength must be between 0 and 1 (inclusive).");
            this.RuleFor(x => x.LinkCandidates).Must(x => x.Count > 0).When(x => !x.IsRoot && x.SelectedOption == ContextMenuOption.LinkRelative).WithMessage("Links required.");
        }
    }
}
