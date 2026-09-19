using FluentValidation;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;

namespace ICMarkets.Blockcypher.Api.Validators;

public class BlockchainHistoryRequestValidator : AbstractValidator<BlockchainHistoryRequest>
{

    public BlockchainHistoryRequestValidator()
    {
        RuleFor(x => x.Chain)
            .NotEmpty()
            .WithMessage("Chain is required")
            .MaximumLength(8)
            .WithMessage("Chain cannot exceed 8 characters")
            .Must(value => Enum.TryParse<Chain>(
                value,
                ignoreCase: true,
                out _
                ))
            .WithMessage("Invalid chain");
        
        RuleFor(x => x.Coin)
            .NotEmpty()
            .WithMessage("Coin is required")
            .MaximumLength(8)
            .WithMessage("Coin cannot exceed 8 characters")
            .Must(value => Enum.TryParse<Coin>(
                value,
                ignoreCase: true,
                out _
            ))
            .WithMessage("Invalid coin");;
    }
}