using Microsoft.AspNetCore.Mvc;
using PayLi.API.Models;
using PayLi.API.Services;

namespace PayLi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class MiscController : ControllerBase
    {
        private readonly IBridgeCardService _bridgeCardService;
        private readonly ILogger<MiscController> _logger;

        public MiscController(IBridgeCardService bridgeCardService, ILogger<MiscController> logger)
        {
            _bridgeCardService = bridgeCardService;
            _logger = logger;
        }

        /// <summary>
        /// Get all cardholders with pagination
        /// </summary>
        /// <param name="page">Page number (required, minimum 1)</param>
        /// <returns>List of all cardholders</returns>
        [HttpGet("cardholders")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<AllCardholdersResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<AllCardholdersResponse>), 400)]
        public async Task<IActionResult> GetAllCardholders([FromQuery] int page = 1)
        {
            try
            {
                _logger.LogInformation("Received request to get all cardholders, page: {Page}", page);

                if (page < 1)
                    return BadRequest(CreateError<AllCardholdersResponse>("Page must be greater than 0"));

                var result = await _bridgeCardService.GetAllCardholdersAsync(page);

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
                _logger.LogError(ex, "Error in GetAllCardholders");
                return StatusCode(500, CreateError<AllCardholdersResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get all issued cards with pagination
        /// </summary>
        /// <param name="page">Page number (required, minimum 1)</param>
        /// <returns>List of all issued cards</returns>
        [HttpGet("cards")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<AllCardsResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<AllCardsResponse>), 400)]
        public async Task<IActionResult> GetAllCards([FromQuery] int page = 1)
        {
            try
            {
                _logger.LogInformation("Received request to get all cards, page: {Page}", page);

                if (page < 1)
                    return BadRequest(CreateError<AllCardsResponse>("Page must be greater than 0"));

                var result = await _bridgeCardService.GetAllCardsAsync(page);

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
                _logger.LogError(ex, "Error in GetAllCards");
                return StatusCode(500, CreateError<AllCardsResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Fund issuing wallet (SANDBOX ONLY - for testing with fake money)
        /// </summary>
        /// <param name="request">Fund wallet request</param>
        /// <param name="currency">Currency (USD or NGN)</param>
        /// <returns>Funding confirmation</returns>
        [HttpPatch("issuing-wallet/fund")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 400)]
        public async Task<IActionResult> FundIssuingWallet([FromBody] FundIssuingWalletRequest request, [FromQuery] string currency = "USD")
        {
            try
            {
                _logger.LogInformation("Received request to fund issuing wallet with currency: {Currency}", currency);

                // Validate currency
                if (currency != SupportedCurrencies.USD && currency != SupportedCurrencies.NGN)
                    return BadRequest(CreateError<object>("Currency must be either 'USD' or 'NGN'"));

                if (string.IsNullOrWhiteSpace(request.amount))
                    return BadRequest(CreateError<object>("Amount is required"));

                if (!decimal.TryParse(request.amount, out var amount) || amount < 0)
                    return BadRequest(CreateError<object>("Amount must be a valid non-negative number"));

                var result = await _bridgeCardService.FundIssuingWalletAsync(request, currency);

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
                _logger.LogError(ex, "Error in FundIssuingWallet");
                return StatusCode(500, CreateError<object>("Internal server error"));
            }
        }

        /// <summary>
        /// Get issuing wallet balance
        /// </summary>
        /// <returns>Wallet balance information</returns>
        [HttpGet("issuing-wallet/balance")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<IssuingWalletBalanceResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<IssuingWalletBalanceResponse>), 400)]
        public async Task<IActionResult> GetIssuingWalletBalance()
        {
            try
            {
                _logger.LogInformation("Received request to get issuing wallet balance");

                var result = await _bridgeCardService.GetIssuingWalletBalanceAsync();

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
                _logger.LogError(ex, "Error in GetIssuingWalletBalance");
                return StatusCode(500, CreateError<IssuingWalletBalanceResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get foreign exchange rates (rate limited: once per minute)
        /// </summary>
        /// <returns>FX rates from local currencies to USD</returns>
        [HttpGet("fx-rates")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FxRateResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<FxRateResponse>), 400)]
        public async Task<IActionResult> GetFxRates()
        {
            try
            {
                _logger.LogInformation("Received request to get FX rates");

                var result = await _bridgeCardService.GetFxRateAsync();

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
                _logger.LogError(ex, "Error in GetFxRates");
                return StatusCode(500, CreateError<FxRateResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get all states for a specific country
        /// </summary>
        /// <param name="country">Country name (Nigeria, Ghana, Uganda, Kenya)</param>
        /// <returns>List of states for the country</returns>
        [HttpGet("states")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<StatesResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<StatesResponse>), 400)]
        public async Task<IActionResult> GetAllStates([FromQuery] string country)
        {
            try
            {
                _logger.LogInformation("Received request to get states for country: {Country}", country);

                if (string.IsNullOrWhiteSpace(country))
                    return BadRequest(CreateError<StatesResponse>("Country parameter is required"));

                // Validate supported countries
                var supportedCountries = new[]
                {
                    CountriesWithStates.NIGERIA,
                    CountriesWithStates.GHANA,
                    CountriesWithStates.UGANDA,
                    CountriesWithStates.KENYA,
                    CountriesWithStates.EGYPT
                };

                if (!supportedCountries.Contains(country, StringComparer.OrdinalIgnoreCase))
                    return BadRequest(CreateError<StatesResponse>($"Country must be one of: {string.Join(", ", supportedCountries)}"));

                var result = await _bridgeCardService.GetAllStatesAsync(country);

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
                _logger.LogError(ex, "Error in GetAllStates");
                return StatusCode(500, CreateError<StatesResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Generate a secure token for card details
        /// </summary>
        /// <param name="cardId">Card ID to generate token for</param>
        /// <returns>Generated token for secure card details access</returns>
        [HttpGet("cards/{cardId}/token")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardTokenResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardTokenResponse>), 400)]
        public async Task<IActionResult> GenerateCardToken(string cardId)
        {
            try
            {
                _logger.LogInformation("Received request to generate card token for card: {CardId}", cardId);

                if (string.IsNullOrWhiteSpace(cardId))
                    return BadRequest(CreateError<CardTokenResponse>("Card ID is required"));

                var result = await _bridgeCardService.GenerateCardTokenAsync(cardId);

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
                _logger.LogError(ex, "Error in GenerateCardToken for card: {CardId}", cardId);
                return StatusCode(500, CreateError<CardTokenResponse>("Internal server error"));
            }
        }

        /// <summary>
        /// Get decrypted card details using a secure token
        /// </summary>
        /// <param name="token">Secure token for card details access</param>
        /// <returns>Decrypted card details</returns>
        [HttpGet("cards/details-by-token")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<DecryptedCardDetailsResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<DecryptedCardDetailsResponse>), 400)]
        public async Task<IActionResult> GetCardDetailsFromToken([FromQuery] string token)
        {
            try
            {
                _logger.LogInformation("Received request to get card details from token");

                if (string.IsNullOrWhiteSpace(token))
                    return BadRequest(CreateError<DecryptedCardDetailsResponse>("Token parameter is required"));

                var result = await _bridgeCardService.GetCardDetailsFromTokenAsync(token);

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
                _logger.LogError(ex, "Error in GetCardDetailsFromToken");
                return StatusCode(500, CreateError<DecryptedCardDetailsResponse>("Internal server error"));
            }
        }

        #region Helper Methods

        /// <summary>
        /// Helper method to create standardized error responses
        /// </summary>
        /// <typeparam name="T">Response data type</typeparam>
        /// <param name="message">Error message</param>
        /// <returns>Standardized error response</returns>
        private BridgeCardApiResponse<T> CreateError<T>(string message)
        {
            return new BridgeCardApiResponse<T>
            {
                status = "error",
                message = message,
                data = default(T)
            };
        }

        #endregion
    }
}
