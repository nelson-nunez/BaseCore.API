using BaseUI.Data;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseUI.Helpers
{
    public class LoginInputValidator : AbstractValidator<InputModel>
    {
        public LoginInputValidator()
        {
            RuleFor(AppRoleDTO => AppRoleDTO.UserName).NotEmpty().WithMessage("Ingrese su dirección de email.");
            RuleFor(AppRoleDTO => AppRoleDTO.Password).NotEmpty().WithMessage("Ingrese su contraseña.");
        }
    }
}