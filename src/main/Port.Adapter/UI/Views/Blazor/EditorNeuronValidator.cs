using ei8.Cortex.Diary.Port.Adapter.UI.ViewModels;
using FluentValidation;

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
