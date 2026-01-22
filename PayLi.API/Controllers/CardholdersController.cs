using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PayLi.API.Models;
using PayLi.API.Services;

namespace PayLi.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class CardholdersController : ControllerBase
    {
        private readonly IBridgeCardService _bridgeCardService;
        private readonly ILogger<CardholdersController> _logger;

        public CardholdersController(IBridgeCardService bridgeCardService, ILogger<CardholdersController> logger)
        {
            _bridgeCardService = bridgeCardService;
            _logger = logger;
        }

        /// <summary>
        /// Register a cardholder synchronously (KYC verification done immediately - takes up to 45 seconds)
        /// </summary>
        /// <param name="request">Cardholder registration details</param>
        /// <returns>Cardholder registration response</returns>
        [HttpPost("register-sync")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<RegisterCardholderResponse>), 201)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<RegisterCardholderResponse>), 400)]
        public async Task<IActionResult> RegisterCardholderSynchronously([FromBody] RegisterCardholderRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to register cardholder synchronously");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate required fields
                var validationResult = ValidateCardholderRequest(request);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return BadRequest(new BridgeCardApiResponse<RegisterCardholderResponse>
                    {
                        status = "error",
                        message = validationResult,
                        data = null
                    });
                }

                var result = await _bridgeCardService.RegisterCardholderSynchronouslyAsync(request);

                if (result.status == "success")
                {
                    return StatusCode(201, result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterCardholderSynchronously");
                return StatusCode(500, new BridgeCardApiResponse<RegisterCardholderResponse>
                {
                    status = "error",
                    message = "Internal server error",
                    data = null
                });
            }
        }

        /// <summary>
        /// Register a cardholder asynchronously (KYC verification done in background - webhook notification)
        /// </summary>
        /// <param name="request">Cardholder registration details</param>
        /// <returns>Cardholder registration response</returns>
        [HttpPost("register-async")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<RegisterCardholderResponse>), 201)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<RegisterCardholderResponse>), 400)]
        public async Task<IActionResult> RegisterCardholderAsynchronously([FromBody] RegisterCardholderRequest request)
        {
            try
            {
                _logger.LogInformation("Received request to register cardholder asynchronously");

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validate required fields
                var validationResult = ValidateCardholderRequest(request);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return BadRequest(new BridgeCardApiResponse<RegisterCardholderResponse>
                    {
                        status = "error",
                        message = validationResult,
                        data = null
                    });
                }

                var result = await _bridgeCardService.RegisterCardholderAsynchronouslyAsync(request);

                if (result.status == "success")
                {
                    return StatusCode(201, result);
                }
                else
                {
                    return BadRequest(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RegisterCardholderAsynchronously");
                return StatusCode(500, new BridgeCardApiResponse<RegisterCardholderResponse>
                {
                    status = "error",
                    message = "Internal server error",
                    data = null
                });
            }
        }

        /// <summary>
        /// Get cardholder details by cardholder ID
        /// </summary>
        /// <param name="cardholderId">The cardholder ID</param>
        /// <returns>Cardholder details</returns>
        [HttpGet("{cardholderId}")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardholderDetailsResponse>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<CardholderDetailsResponse>), 400)]
        public async Task<IActionResult> GetCardholderDetails([FromRoute] string cardholderId)
        {
            try
            {
                _logger.LogInformation($"Received request to get cardholder details for ID: {cardholderId}");

                if (string.IsNullOrWhiteSpace(cardholderId))
                {
                    return BadRequest(new BridgeCardApiResponse<CardholderDetailsResponse>
                    {
                        status = "error",
                        message = "Cardholder ID is required",
                        data = null
                    });
                }

                var result = await _bridgeCardService.GetCardholderDetailsAsync(cardholderId);

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
                _logger.LogError(ex, $"Error in GetCardholderDetails for ID: {cardholderId}");
                return StatusCode(500, new BridgeCardApiResponse<CardholderDetailsResponse>
                {
                    status = "error",
                    message = "Internal server error",
                    data = null
                });
            }
        }

        /// <summary>
        /// Delete a cardholder by cardholder ID
        /// </summary>
        /// <param name="cardholderId">The cardholder ID</param>
        /// <returns>Deletion confirmation</returns>
        [HttpDelete("{cardholderId}")]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 200)]
        [ProducesResponseType(typeof(BridgeCardApiResponse<object>), 400)]
        public async Task<IActionResult> DeleteCardholder([FromRoute] string cardholderId)
        {
            try
            {
                _logger.LogInformation($"Received request to delete cardholder with ID: {cardholderId}");

                if (string.IsNullOrWhiteSpace(cardholderId))
                {
                    return BadRequest(new BridgeCardApiResponse<object>
                    {
                        status = "error",
                        message = "Cardholder ID is required",
                        data = null
                    });
                }

                var result = await _bridgeCardService.DeleteCardholderAsync(cardholderId);

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
                _logger.LogError(ex, $"Error in DeleteCardholder for ID: {cardholderId}");
                return StatusCode(500, new BridgeCardApiResponse<object>
                {
                    status = "error",
                    message = "Internal server error",
                    data = null
                });
            }
        }

        /// <summary>
        /// Get supported identity types for different countries
        /// </summary>
        /// <returns>List of supported identity types</returns>
        [HttpGet("identity-types")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetSupportedIdentityTypes()
        {
            var identityTypes = new
            {
                Nigeria = new[]
                {
                    IdentityTypes.NIGERIAN_BVN_VERIFICATION,
                    IdentityTypes.NIGERIAN_NIN,
                    IdentityTypes.NIGERIAN_INTERNATIONAL_PASSPORT,
                    IdentityTypes.NIGERIAN_PVC,
                    IdentityTypes.NIGERIAN_DRIVERS_LICENSE
                },
                Ghana = new[]
                {
                    IdentityTypes.GHANIAN_SSNIT,
                    IdentityTypes.GHANIAN_VOTERS_ID,
                    IdentityTypes.GHANIAN_DRIVERS_LICENSE,
                    IdentityTypes.GHANIAN_INTERNATIONAL_PASSPORT,
                    IdentityTypes.GHANIAN_GHANA_CARD
                },
                Uganda = new[]
                {
                    IdentityTypes.UGANDA_VOTERS_ID,
                    IdentityTypes.UGANDA_PASSPORT,
                    IdentityTypes.UGANDA_NATIONAL_ID,
                    IdentityTypes.UGANDA_DRIVERS_LICENSE
                },
                Kenya = new[]
                {
                    IdentityTypes.KENYAN_VOTERS_ID
                }
            };

            return Ok(new
            {
                status = "success",
                message = "Supported identity types retrieved successfully",
                data = identityTypes
            });
        }

        /// <summary>
        /// Get supported countries
        /// </summary>
        /// <returns>List of supported countries</returns>
        [HttpGet("supported-countries")]
        [ProducesResponseType(typeof(object), 200)]
        public IActionResult GetSupportedCountries()
        {
            var countries = new[]
            {
                SupportedCountries.NIGERIA,
                SupportedCountries.GHANA,
                SupportedCountries.UGANDA,
                SupportedCountries.KENYA
            };

            return Ok(new
            {
                status = "success",
                message = "Supported countries retrieved successfully",
                data = countries
            });
        }

        private string ValidateCardholderRequest(RegisterCardholderRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.first_name) || request.first_name.Length < 3)
                return "Invalid firstname, a valid name should have a minimum of 3 letters";

            if (string.IsNullOrWhiteSpace(request.last_name) || request.last_name.Length < 3)
                return "Invalid lastname, a valid name should have a minimum of 3 letters";

            if (string.IsNullOrWhiteSpace(request.phone))
                return "Phone number is required";

            if (string.IsNullOrWhiteSpace(request.email_address) || !request.email_address.Contains("@"))
                return "Valid email address is required";

            if (request.address == null)
                return "Address is required";

            if (string.IsNullOrWhiteSpace(request.address.address) || request.address.address.Length < 3)
                return "Invalid address, a valid address should have a minimum of 3 letters";

            if (string.IsNullOrWhiteSpace(request.address.house_no))
                return "House number is required";

            if (string.IsNullOrWhiteSpace(request.address.city))
                return "City is required";

            if (string.IsNullOrWhiteSpace(request.address.state))
                return "State is required";

            if (string.IsNullOrWhiteSpace(request.address.country))
                return "Country is required";

            if (string.IsNullOrWhiteSpace(request.address.postal_code))
                return "Postal code is required";

            if (request.identity == null)
                return "Identity information is required";

            if (string.IsNullOrWhiteSpace(request.identity.id_type))
                return "Identity type is required";

            // Validate BVN for Nigerian identity types that require it
            if (request.identity.id_type == IdentityTypes.NIGERIAN_BVN_VERIFICATION)
            {
                if (string.IsNullOrWhiteSpace(request.identity.bvn) || request.identity.bvn.Length != 11)
                    return "Invalid BVN, a valid BVN is 11 digits long";
            }

            // Validate required fields for different identity types
            if (request.identity.id_type != IdentityTypes.NIGERIAN_BVN_VERIFICATION &&
                string.IsNullOrWhiteSpace(request.identity.id_no))
                return "ID number is required for this identity type";

            return string.Empty;
        }
    }
}
