using FluentValidation;
using BaseUI.Data;
using BaseRest.Core.Model.DTO;

namespace BaseUI.Helpers
{
    public class CustomerValidator : AbstractValidator<CustomerDTO>
    {
        public CustomerValidator()
        {
            RuleFor(customer => customer.Name).NotEmpty().WithMessage("* Ingrese la Razón Social.");
            RuleFor(customer => customer.BirthDate).NotEmpty().WithMessage("* Ingrese una fecha de nacimiento.");
            RuleFor(customer => customer.GenderId).NotEmpty().WithMessage("* Seleccione el Sexo.");
            RuleFor(customer => customer.CUIL).NotEmpty().WithMessage("* Ingrese el Nro. de CUIT.");
            //RuleFor(customer => customer.email).NotEmpty().WithMessage("* Ingrese el Email.").EmailAddress().WithMessage("* Ingrese un Email Válido.");
        }
    }
}