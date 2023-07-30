using FluentValidation;
using jConverter.Application.Models;

namespace jConverter.Application.Validators
{
    public class XmlFileValidator : AbstractValidator<InputRequestModel>
    {
        public XmlFileValidator()
        {        
            RuleFor(x => x)
                .NotNull()
                .WithMessage("Please upload a valid xml file with an .xml extension.");

            RuleFor(x => x.FileName)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .Must(file => file.EndsWith(".xml", StringComparison.OrdinalIgnoreCase));
        }
    }
}