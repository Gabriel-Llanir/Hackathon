using Consulta.Models;
using FluentValidation;

namespace Consulta.Validators
{
    public class UsuarioValidator : AbstractValidator<Usuario>
    {
        public UsuarioValidator()
        {
            RuleFor(u => u.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
            RuleFor(u => u.Telefone).NotEmpty().WithMessage("O telefone é obrigatório.")
                                   .Matches(@"^\d{2}\d{8,9}$").WithMessage("O telefone deve estar no formato válido.");
            RuleFor(u => u.Email).NotEmpty().WithMessage("O e-mail é obrigatório.")
                                 .EmailAddress().WithMessage("O e-mail deve estar no formato válido.");
        }
    }
}
