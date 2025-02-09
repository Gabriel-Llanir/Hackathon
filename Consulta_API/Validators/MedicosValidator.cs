using Consulta.Models;
using FluentValidation;

namespace Consulta.Validators
{
    public class MedicosValidator : AbstractValidator<Medico>
    {
        public MedicosValidator()
        {
            RuleFor(u => u.Nome).NotEmpty().WithMessage("O nome é obrigatório.");
            RuleFor(u => u.CPF).NotEmpty().WithMessage("O CPF é obrigatório.").Matches(@"^\d{3}\.\d{3}\.\d{3}-\d{2}$").WithMessage("O CPF deve estar no formato válido.");
            RuleFor(u => u.CRM).NotEmpty().WithMessage("O CRM é obrigatório.");
            RuleFor(u => u.Email).NotEmpty().WithMessage("O e-mail é obrigatório.").EmailAddress().WithMessage("O e-mail deve estar no formato válido.");
            RuleFor(u => u.Senha).NotEmpty().WithMessage("A senha é obrigatória.");
        }
    }
}
