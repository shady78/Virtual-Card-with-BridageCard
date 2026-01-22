using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayLi.API.Models;
using PayLi.API.Services;

namespace PayLi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CardsController : ControllerBase
    {
        private readonly IBridgeCardService _bridgeCardService;
        private readonly IEncryptionService _encryptionService;
        private readonly ILogger<CardsController> _logger;

        public CardsController(IBridgeCardService bridgeCardService, IEncryptionService encryptionService, ILogger<CardsController> logger)
        {
            _bridgeCardService = bridgeCardService;
            _encryptionService = encryptionService;
            _logger = logger;
        }

        /// <summary>
        /// Create a new USD card for a verified cardholder
        /// </summary>
        /// <param name="request">Card creation details</param>
        /// <returns>Card creation response</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CreateCardResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CreateCardResponse>), 400)]
        public async Task<IActionResult> CreateCard([FromBody] CreateCardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to create card for cardholder: {CardholderId}", request.cardholder_id);

                // Validate request
                var validationResult = ValidateCreateCardRequest(request);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return BadRequest(new BridgeCardApiResponse<CreateCardResponse>
                    {
                        status = "error",
                        message = validationResult,
                        data = null
                    });
                }

                var result = await _bridgeCardService.CreateCardAsync(request);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateCard");
                return StatusCode(500, new BridgeCardApiResponse<CreateCardResponse>
                {
                    status = "error",
                    message = "Internal server error",
                    data = null
                });
            }
        }

        /// <summary>
        /// Activate a physical card
        /// </summary>
        /// <param name="request">Physical card activation details</param>
        /// <returns>Card activation response</returns>
        [HttpPost("activate-physical")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CreateCardResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CreateCardResponse>), 400)]
        public async Task<IActionResult> ActivatePhysicalCard([FromBody] ActivatePhysicalCardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to activate physical card for cardholder: {CardholderId}", request.cardholder_id);

                // Validate request
                if (string.IsNullOrWhiteSpace(request.cardholder_id))
                    return BadRequest(CreateError<CreateCardResponse>("Cardholder ID is required"));

                if (string.IsNullOrWhiteSpace(request.card_token_number))
                    return BadRequest(CreateError<CreateCardResponse>("Card token number is required"));

                var result = await _bridgeCardService.ActivatePhysicalCardAsync(request);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ActivatePhysicalCard");
                return StatusCode(500, CreateError<CreateCardResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get card details (encrypted by default, use decrypted=true for sensitive data)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <param name="decrypted">Whether to use decrypted endpoint for sensitive data</param>
        /// <returns>Card details</returns>
        [HttpGet("{cardId}")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardDetailsResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardDetailsResponse>), 400)]
        public async Task<IActionResult> GetCardDetails([FromRoute] string cardId, [FromQuery] bool decrypted = false)
        {
            try
            {
                _logger.LogInformation("Received request to get card details for: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardDetailsResponse>("Card ID is required"));

                var result = await _bridgeCardService.GetCardDetailsAsync(cardId, decrypted);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCardDetails for card: {CardId}", cardId);
                return StatusCode(500, CreateError<CardDetailsResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get card balance
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <returns>Card balance information</returns>
        [HttpGet("{cardId}/balance")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardBalanceResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardBalanceResponse>), 400)]
        public async Task<IActionResult> GetCardBalance([FromRoute] string cardId)
        {
            try
            {
                _logger.LogInformation("Received request to get balance for card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardBalanceResponse>("Card ID is required"));

                var result = await _bridgeCardService.GetCardBalanceAsync(cardId);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCardBalance for card: {CardId}", cardId);
                return StatusCode(500, CreateError<CardBalanceResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Fund a card (asynchronous operation - listen for webhook)
        /// </summary>
        /// <param name="request">Fund card details</param>
        /// <returns>Fund card response</returns>
        [HttpPatch("fund")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FundCardResponse>), 202)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FundCardResponse>), 400)]
        public async Task<IActionResult> FundCard([FromBody] FundCardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to fund card: {CardId}", request.card_id);

                // Validate request
                var validationResult = ValidateFundCardRequest(request);
                if (!string.IsNullOrEmpty(validationResult))
                    return BadRequest(CreateError<FundCardResponse>(validationResult));

                var result = await _bridgeCardService.FundCardAsync(request);

                if (result.status == "success")
                {
                    return StatusCode(202, result); // 202 Accepted for async operation
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FundCard");
                return StatusCode(500, CreateError<FundCardResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Unload funds from a card (asynchronous operation - listen for webhook)
        /// </summary>
        /// <param name="request">Unload card details</param>
        /// <returns>Unload card response</returns>
        [HttpPatch("unload")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FundCardResponse>), 202)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FundCardResponse>), 400)]
        public async Task<IActionResult> UnloadCard([FromBody] UnloadCardRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to unload card: {CardId}", request.card_id);

                // Validate request
                var validationResult = ValidateUnloadCardRequest(request);
                if (!string.IsNullOrEmpty(validationResult))
                    return BadRequest(CreateError<FundCardResponse>(validationResult));

                var result = await _bridgeCardService.UnloadCardAsync(request);

                if (result.status == "success")
                {
                    return StatusCode(202, result); // 202 Accepted for async operation
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnloadCard");
                return StatusCode(500, CreateError<FundCardResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Create a mock debit transaction for testing (sandbox only)
        /// </summary>
        /// <param name="request">Mock transaction details</param>
        /// <returns>Mock transaction response</returns>
        [HttpPatch("mock-debit")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FundCardResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FundCardResponse>), 400)]
        public async Task<IActionResult> MockDebitTransaction([FromBody] MockDebitTransactionRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to create mock debit transaction for card: {CardId}", request.card_id);

                if (string.IsNullOrWhiteSpace(request.card_id))
                    return BadRequest(CreateError<FundCardResponse>("Card ID is required"));

                var result = await _bridgeCardService.MockDebitTransactionAsync(request);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in MockDebitTransaction");
                return StatusCode(500, CreateError<FundCardResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get card transactions with pagination and optional date filtering
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <param name="page">Page number (required)</param>
        /// <param name="startDate">Start date filter (format: 2023-03-01 12:10:00)</param>
        /// <param name="endDate">End date filter (format: 2023-03-01 12:10:00)</param>
        /// <returns>Card transactions</returns>
        [HttpGet("{cardId}/transactions")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardTransactionsResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardTransactionsResponse>), 400)]
        public async Task<IActionResult> GetCardTransactions([FromRoute] string cardId, [FromQuery] int page, [FromQuery] string? startDate = null, [FromQuery] string? endDate = null)
        {
            try
            {
                _logger.LogInformation("Received request to get transactions for card: {CardId}, page: {Page}", cardId, page);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardTransactionsResponse>("Card ID is required"));

                if (page < 1)
                    return BadRequest(CreateError<CardTransactionsResponse>("Page must be greater than 0"));

                var result = await _bridgeCardService.GetCardTransactionsAsync(cardId, page, startDate, endDate);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCardTransactions");
                return StatusCode(500, CreateError<CardTransactionsResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get a specific card transaction by reference ID
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <param name="clientTransactionReference">Client transaction reference</param>
        /// <param name="bridgecardTransactionReference">BridgeCard transaction reference</param>
        /// <returns>Transaction details</returns>
        [HttpGet("{cardId}/transactions/by-reference")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardTransaction>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardTransaction>), 400)]
        public async Task<IActionResult> GetCardTransactionByReference([FromRoute] string cardId, [FromQuery] string? clientTransactionReference = null, [FromQuery] string? bridgecardTransactionReference = null)
        {
            try
            {
                _logger.LogInformation("Received request to get transaction by reference for card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardTransaction>("Card ID is required"));

                if (string.IsNullOrWhiteSpace(clientTransactionReference) && string.IsNullOrWhiteSpace(bridgecardTransactionReference))
                    return BadRequest(CreateError<CardTransaction>("Either client or bridgecard transaction reference is required"));

                var result = await _bridgeCardService.GetCardTransactionByIdAsync(cardId, clientTransactionReference, bridgecardTransactionReference);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCardTransactionByReference");
                return StatusCode(500, CreateError<CardTransaction>("Internal server error"));
            }
        }

        /// <summary>
        /// Get transaction status (PENDING, SUCCESSFUL, FAILED)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <param name="clientTransactionReference">Client transaction reference</param>
        /// <returns>Transaction status</returns>
        [HttpGet("{cardId}/transactions/{clientTransactionReference}/status")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<TransactionStatusResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<TransactionStatusResponse>), 400)]
        public async Task<IActionResult> GetTransactionStatus([FromRoute] string cardId, [FromRoute] string clientTransactionReference)
        {
            try
            {
                _logger.LogInformation("Received request to get transaction status for card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<TransactionStatusResponse>("Card ID is required"));

                if (string.IsNullOrWhiteSpace(clientTransactionReference))
                    return BadRequest(CreateError<TransactionStatusResponse>("Client transaction reference is required"));

                var result = await _bridgeCardService.GetCardTransactionStatusAsync(cardId, clientTransactionReference);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetTransactionStatus");
                return StatusCode(500, CreateError<TransactionStatusResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Freeze a card (sets is_active to false)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <returns>Freeze confirmation</returns>
        [HttpPatch("{cardId}/freeze")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardActionResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardActionResponse>), 400)]
        public async Task<IActionResult> FreezeCard([FromRoute] string cardId)
        {
            try
            {
                _logger.LogInformation("Received request to freeze card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardActionResponse>("Card ID is required"));

                var result = await _bridgeCardService.FreezeCardAsync(cardId);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in FreezeCard");
                return StatusCode(500, CreateError<CardActionResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Unfreeze a card (sets is_active to true)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <returns>Unfreeze confirmation</returns>
        [HttpPatch("{cardId}/unfreeze")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardActionResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardActionResponse>), 400)]
        public async Task<IActionResult> UnfreezeCard([FromRoute] string cardId)
        {
            try
            {
                _logger.LogInformation("Received request to unfreeze card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardActionResponse>("Card ID is required"));

                var result = await _bridgeCardService.UnfreezeCardAsync(cardId);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UnfreezeCard");
                return StatusCode(500, CreateError<CardActionResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get all cards for a cardholder
        /// </summary>
        /// <param name="cardholderId">Cardholder ID</param>
        /// <returns>List of cardholder's cards</returns>
        [HttpGet("cardholder/{cardholderId}")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<AllCardholderCardsResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<AllCardholderCardsResponse>), 400)]
        public async Task<IActionResult> GetAllCardholderCards([FromRoute] string cardholderId)
        {
            try
            {
                _logger.LogInformation("Received request to get all cards for cardholder: {CardholderId}", cardholderId);

                if (string.IsNullOrWhiteSpace(cardholderId))
                    return BadRequest(CreateError<AllCardholderCardsResponse>("Cardholder ID is required"));

                var result = await _bridgeCardService.GetAllCardholderCardsAsync(cardholderId);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetAllCardholderCards");
                return StatusCode(500, CreateError<AllCardholderCardsResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Delete a card (unload funds first to avoid loss)
        /// </summary>
        /// <param name="cardId">Card ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{cardId}")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 400)]
        public async Task<IActionResult> DeleteCard([FromRoute] string cardId)
        {
            try
            {
                _logger.LogInformation("Received request to delete card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<object>("Card ID is required"));

                var result = await _bridgeCardService.DeleteCardAsync(cardId);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCard");
                return StatusCode(500, CreateError<object>("Internal server error"));
            }
        }

        /// <summary>
        /// Update card PIN (requires AES 256 encrypted 4-digit PIN)
        /// </summary>
        /// <param name="request">Update PIN request</param>
        /// <returns>Update confirmation</returns>
        [HttpPut("update-pin")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 400)]
        public async Task<IActionResult> UpdateCardPin([FromBody] UpdateCardPinRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to update PIN for card: {CardId}", request.card_id);

                if (string.IsNullOrWhiteSpace(request.card_id))
                    return BadRequest(CreateError<object>("Card ID is required"));

                if (string.IsNullOrWhiteSpace(request.card_pin))
                    return BadRequest(CreateError<object>("Encrypted card PIN is required"));

                var result = await _bridgeCardService.UpdateCardPinAsync(request);

                if (result.status == "success")
                {
                    return Ok(result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateCardPin");
                return StatusCode(500, CreateError<object>("Internal server error"));
            }
        }

        /// <summary>
        /// Get supported card types, brands, currencies, and limits
        /// </summary>
        /// <returns>Supported card configuration</returns>
        [HttpGet("supported-options")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetSupportedCardOptions()
        {
            var options = new
            {
                card_types = new[] { CardTypes.VIRTUAL, CardTypes.PHYSICAL },
                card_brands = new[] { CardBrands.MASTERCARD },
                card_currencies = new[] { CardCurrencies.USD },
                card_limits = new
                {
                    limit_5k = new { value = CardLimits.LIMIT_5K, min_funding = CardLimits.MIN_FUNDING_5K, description = "$5,000 limit" },
                    limit_10k = new { value = CardLimits.LIMIT_10K, min_funding = CardLimits.MIN_FUNDING_10K, description = "$10,000 limit" }
                },
                transaction_types = new[] { TransactionTypes.CREDIT, TransactionTypes.DEBIT },
                transaction_status = new[] { TransactionStatus.PENDING, TransactionStatus.SUCCESSFUL, TransactionStatus.FAILED }
            };

            return Ok(new
            {
                status = "success",
                message = "Supported card options retrieved successfully",
                data = options
            });
        }

        // Helper methods
        private BridgeCardApiResponse<T> CreateError<T>(string message)
        {
            return new BridgeCardApiResponse<T>
            {
                status = "error",
                message = message,
                data = default(T)
            };
        }

        private string ValidateCreateCardRequest(CreateCardRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.cardholder_id))
                return "Cardholder ID is required";

            if (string.IsNullOrWhiteSpace(request.card_type) ||
                (request.card_type != CardTypes.VIRTUAL && request.card_type != CardTypes.PHYSICAL))
                return "Card type must be 'virtual' or 'physical'";

            if (string.IsNullOrWhiteSpace(request.card_brand) || request.card_brand != CardBrands.MASTERCARD)
                return "Card brand must be 'Mastercard'";

            if (string.IsNullOrWhiteSpace(request.card_currency) || request.card_currency != CardCurrencies.USD)
                return "Card currency must be 'USD'";

            if (string.IsNullOrWhiteSpace(request.card_limit) ||
                (request.card_limit != CardLimits.LIMIT_5K && request.card_limit != CardLimits.LIMIT_10K))
                return "Card limit must be '500000' ($5,000) or '1000000' ($10,000)";

            if (string.IsNullOrWhiteSpace(request.funding_amount))
                return "Funding amount is required";

            if (!int.TryParse(request.funding_amount, out var fundingAmount))
                return "Funding amount must be a valid number";

            // Validate minimum funding based on card limit
            var minFunding = request.card_limit == CardLimits.LIMIT_5K ? CardLimits.MIN_FUNDING_5K : CardLimits.MIN_FUNDING_10K;
            if (fundingAmount < minFunding)
                return $"Minimum funding amount is ${minFunding / 100.0:F2} for this card limit";

            if (string.IsNullOrWhiteSpace(request.pin))
                return "Encrypted PIN is required";

            return string.Empty;
        }

        private string ValidateFundCardRequest(FundCardRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.card_id))
                return "Card ID is required";

            if (string.IsNullOrWhiteSpace(request.amount))
                return "Amount is required";

            if (!int.TryParse(request.amount, out var amount) || amount <= 0)
                return "Amount must be a valid positive number";

            if (string.IsNullOrWhiteSpace(request.transaction_reference))
                return "Transaction reference is required";

            if (string.IsNullOrWhiteSpace(request.currency) || request.currency != CardCurrencies.USD)
                return "Currency must be 'USD'";

            return string.Empty;
        }

        private string ValidateUnloadCardRequest(UnloadCardRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.card_id))
                return "Card ID is required";

            if (string.IsNullOrWhiteSpace(request.amount))
                return "Amount is required";

            if (!int.TryParse(request.amount, out var amount) || amount < 0)
                return "Amount must be a valid non-negative number";

            if (string.IsNullOrWhiteSpace(request.transaction_reference))
                return "Transaction reference is required";

            if (string.IsNullOrWhiteSpace(request.currency) || request.currency != CardCurrencies.USD)
                return "Currency must be 'USD'";

            return string.Empty;
        }
    }
}
