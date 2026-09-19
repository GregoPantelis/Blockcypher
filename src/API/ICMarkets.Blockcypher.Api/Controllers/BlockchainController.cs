using FluentValidation.Results;
using ICMarkets.Blockcypher.Api.Contracts.Requests;
using ICMarkets.Blockcypher.Api.Contracts.Responses;
using ICMarkets.Blockcypher.Api.Helpers;
using ICMarkets.Blockcypher.Api.Mappers;
using ICMarkets.Blockcypher.Application.DataObjects.Dtos;
using ICMarkets.Blockcypher.Application.DataObjects.Enums;
using ICMarkets.Blockcypher.Application.DataObjects.Filters;
using ICMarkets.Blockcypher.Application.Interfaces.Services;
using ICMarkets.Blockcypher.Domain.Types.OperationalResult;
using ICMarkets.Blockcypher.Domain.Types.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace ICMarkets.Blockcypher.Api.Controllers
{
    [ApiController]
    [Route("api/blockchains")]
    public class BlockchainController : ControllerBase
    {
        private readonly ILogger<BlockchainController> _logger;
        private readonly IBlockchainService _blockchainService;
        private readonly IValidator<BlockchainSnaphotRequest> _blockchainSnapReqValidator;
        private readonly IValidator<BlockchainHistoryRequest> _blockchainHistReqValidator;

        public BlockchainController(
            ILogger<BlockchainController> logger,
            IBlockchainService blockchainService,
            IValidator<BlockchainSnaphotRequest> blockchainSnapReqValidator,
            IValidator<BlockchainHistoryRequest> blockchainHistReqValidator)
        {
            _logger = logger;
            _blockchainService = blockchainService;
            _blockchainSnapReqValidator = blockchainSnapReqValidator;
            _blockchainHistReqValidator = blockchainHistReqValidator;
        }

        [HttpPost("snapshot", Name = "snapshot")]
        [Consumes("application/json")]
        [Authorize]
        public async Task<IActionResult> GetBlockchainData([FromBody] BlockchainSnaphotRequest request, CancellationToken cancellationToken)
        {
            // Validate the request here, maybe we will use FluentValidation. To be implemented later.
            ValidationResult validationResult = await _blockchainSnapReqValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ErrorResponse()
                {
                    Error = OperationResults.Api.ApiInvalidInput.ToString(),
                    ErrorMessage = string.Join(',', validationResult.Errors)
                });
            }
            
            OperationResult<BlockchainData> result = await _blockchainService.CaptureBlockchainDataAsync(new BlockchainDataFilter()
            {
                Chain = Enum.Parse<Chain>(request.Chain, true),
                Coin = Enum.Parse<Coin>(request.Coin, true)
            });

            if (!result.IsSuccessful)
            {
                return this.ToActionResult(result);
            }

            return Ok(result.Data?.MapToBlockchainSnapshotResponse());
        }


        [HttpGet("{coin}/{chain}/history")]
        [Authorize]
        public async Task<IActionResult> GetHistory([FromRoute] BlockchainHistoryRequest request, CancellationToken cancellationToken)
        {
            // Reads saved snapshots from SQLite.
            ValidationResult validationResult = await _blockchainHistReqValidator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                return BadRequest(new ErrorResponse()
                {
                    Error = OperationResults.Api.ApiInvalidInput.ToString(),
                    ErrorMessage = string.Join(',', validationResult.Errors)
                });
            }

            OperationResult<IEnumerable<BlockchainData>> result = await _blockchainService.GetBlockchainHistoryAsync(new BlockchainDataFilter()
            {
                Chain = Enum.Parse<Chain>(request.Chain, true),
                Coin = Enum.Parse<Coin>(request.Coin, true)
            });

            if (!result.IsSuccessful)
            {
                return this.ToActionResult(result);
            }

            return Ok(result.Data?.MapToBlockchainHistoryResponse());
        }

    }
}
