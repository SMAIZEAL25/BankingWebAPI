

using FluentValidation;
namespace BankingWebAPI.FluentValidations
{

    public class DepositCommandValidator : AbstractValidator<DepositCommand>
    {
        public DepositCommandValidator()
        {
            RuleFor(x => x.AccountNumber).NotEmpty().Length(10, 20);
            RuleFor(x => x.Amount).GreaterThan(0);
        }
    }

}
