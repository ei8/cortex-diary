using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using ei8.Cortex.Library.Common;
using FluentValidation;
using neurUL.Cortex.Common;

namespace ei8.Cortex.Diary.Port.Adapter.UI.Views.Blazor
{
    public class EditorTerminalValidator : AbstractValidator<EditorTerminalViewModel>
    {
        public EditorTerminalValidator()
        {
            this.RuleFor(x => x.Neuron).NotEmpty().WithMessage("Neuron is required.");
            this.RuleFor(x => x.Type).Must(x => !(x == RelativeType.NotSet || x == null)).WithMessage("Type is required.");
            this.RuleFor(x => x.Effect).Must(x => !(x == NeurotransmitterEffect.NotSet || x == null)).WithMessage("Effect is required.");
            this.RuleFor(x => x.Strength).NotEmpty().WithMessage("Strength is required.");
            this.RuleFor(x => x.Strength).Must(s => s >= 0 && s <= 1).WithMessage("Strength must be between 0 and 1 (inclusive).");
        }
    }
}
