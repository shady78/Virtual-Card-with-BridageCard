using Microsoft.AspNetCore.Http;
using PayLi.API.Models;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PayLi.API.Services
{
    public class BridgeCardService : IBridgeCardService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<BridgeCardService> _logger;
        private readonly string _baseUrl;
        private readonly string _bearerToken;

        public BridgeCardService(HttpClient httpClient, IConfiguration configuration, ILogger<BridgeCardService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
            _baseUrl = _configuration["BridgeCard:BaseUrl"] ?? "https://issuecards.api.bridgecard.co/v1/issuing/sandbox";
            _bearerToken = _configuration["BridgeCard:BearerToken"] ?? "";

            // Set timeout for synchronous requests (45 seconds as recommended)
            _httpClient.Timeout = TimeSpan.FromSeconds(50);
        }

        public async Task<BridgeCardApiResponse<RegisterCardholderResponse>> RegisterCardholderSynchronouslyAsync(RegisterCardholderRequest request)
        {
            try
            {
                _logger.LogInformation("Registering cardholder synchronously");

                var endpoint = $"{_baseUrl}/cardholder/register_cardholder_synchronously";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = content
                };

                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<BridgeCardApiResponse<RegisterCardholderResponse>>(responseContent, options);
                    return result ?? new BridgeCardApiResponse<RegisterCardholderResponse>();
                }
                else
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, options);
                    return new BridgeCardApiResponse<RegisterCardholderResponse>
                    {
                        status = "error",
                        message = errorResponse?.message ?? "Unknown error occurred",
                        data = null
                    };
                }
            }
            catch (TaskCanceledException ex)
            {
                _logger.LogError(ex, "Request timeout while registering cardholder synchronously");
                return new BridgeCardApiResponse<RegisterCardholderResponse>
                {
                    status = "error",
                    message = "Request timeout. Please try again.",
                    data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering cardholder synchronously");
                return new BridgeCardApiResponse<RegisterCardholderResponse>
                {
                    status = "error",
                    message = "An error occurred while processing your request",
                    data = null
                };
            }
        }

        public async Task<BridgeCardApiResponse<RegisterCardholderResponse>> RegisterCardholderAsynchronouslyAsync(RegisterCardholderRequest request)
        {
            try
            {
                _logger.LogInformation("Registering cardholder asynchronously");

                var endpoint = $"{_baseUrl}/cardholder/register_cardholder";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint)
                {
                    Content = content
                };

                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<BridgeCardApiResponse<RegisterCardholderResponse>>(responseContent, options);
                    return result ?? new BridgeCardApiResponse<RegisterCardholderResponse>();
                }
                else
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, options);
                    return new BridgeCardApiResponse<RegisterCardholderResponse>
                    {
                        status = "error",
                        message = errorResponse?.message ?? "Unknown error occurred",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error registering cardholder asynchronously");
                return new BridgeCardApiResponse<RegisterCardholderResponse>
                {
                    status = "error",
                    message = "An error occurred while processing your request",
                    data = null
                };
            }
        }

        public async Task<BridgeCardApiResponse<CardholderDetailsResponse>> GetCardholderDetailsAsync(string cardholderId)
        {
            try
            {
                _logger.LogInformation($"Getting cardholder details for ID: {cardholderId}");

                var endpoint = $"{_baseUrl}/cardholder/get_cardholder?cardholder_id={cardholderId}";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<BridgeCardApiResponse<CardholderDetailsResponse>>(responseContent, options);
                    return result ?? new BridgeCardApiResponse<CardholderDetailsResponse>();
                }
                else
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, options);
                    return new BridgeCardApiResponse<CardholderDetailsResponse>
                    {
                        status = "error",
                        message = errorResponse?.message ?? "Unknown error occurred",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cardholder details for ID: {cardholderId}");
                return new BridgeCardApiResponse<CardholderDetailsResponse>
                {
                    status = "error",
                    message = "An error occurred while processing your request",
                    data = null
                };
            }
        }

        public async Task<BridgeCardApiResponse<object>> DeleteCardholderAsync(string cardholderId)
        {
            try
            {
                _logger.LogInformation($"Deleting cardholder with ID: {cardholderId}");

                var endpoint = $"{_baseUrl}/cardholder/delete_cardholder/{cardholderId}";

                var httpRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };
                    var result = JsonSerializer.Deserialize<BridgeCardApiResponse<object>>(responseContent, options);
                    return result ?? new BridgeCardApiResponse<object>();
                }
                else
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, options);
                    return new BridgeCardApiResponse<object>
                    {
                        status = "error",
                        message = errorResponse?.message ?? "Unknown error occurred",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting cardholder with ID: {cardholderId}");
                return new BridgeCardApiResponse<object>
                {
                    status = "error",
                    message = "An error occurred while processing your request",
                    data = null
                };
            }
        }



        public async Task<BridgeCardApiResponse<CreateCardResponse>> CreateCardAsync(CreateCardRequest request)
        {
            try
            {
                _logger.LogInformation("Creating card for cardholder: {CardholderId}", request.cardholder_id);

                var endpoint = $"{_baseUrl}/cards/create_card";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CreateCardResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating card");
                return CreateErrorResponse<CreateCardResponse>("An error occurred while creating the card");
            }
        }

        public async Task<BridgeCardApiResponse<CreateCardResponse>> ActivatePhysicalCardAsync(ActivatePhysicalCardRequest request)
        {
            try
            {
                _logger.LogInformation("Activating physical card for cardholder: {CardholderId}", request.cardholder_id);

                var endpoint = $"{_baseUrl}/cards/activate_physical_card";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CreateCardResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating physical card");
                return CreateErrorResponse<CreateCardResponse>("An error occurred while activating the physical card");
            }
        }

        public async Task<BridgeCardApiResponse<CardDetailsResponse>> GetCardDetailsAsync(string cardId, bool decryptedEndpoint = false)
        {
            try
            {
                _logger.LogInformation("Getting card details for card: {CardId}", cardId);

                var baseEndpoint = decryptedEndpoint
                    ? "https://issuecards-api-bridgecard-co.relay.evervault.com/v1/issuing/sandbox"
                    : _baseUrl;
                var endpoint = $"{baseEndpoint}/cards/get_card_details?card_id={cardId}";

                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardDetailsResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card details for card: {CardId}", cardId);
                return CreateErrorResponse<CardDetailsResponse>("An error occurred while getting card details");
            }
        }

        public async Task<BridgeCardApiResponse<CardBalanceResponse>> GetCardBalanceAsync(string cardId)
        {
            try
            {
                _logger.LogInformation("Getting card balance for card: {CardId}", cardId);

                var endpoint = $"{_baseUrl}/cards/get_card_balance?card_id={cardId}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardBalanceResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card balance for card: {CardId}", cardId);
                return CreateErrorResponse<CardBalanceResponse>("An error occurred while getting card balance");
            }
        }

        public async Task<BridgeCardApiResponse<FundCardResponse>> FundCardAsync(FundCardRequest request)
        {
            try
            {
                _logger.LogInformation("Funding card: {CardId} with amount: {Amount}", request.card_id, request.amount);

                var endpoint = $"{_baseUrl}/cards/fund_card_asynchronously";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<FundCardResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error funding card: {CardId}", request.card_id);
                return CreateErrorResponse<FundCardResponse>("An error occurred while funding the card");
            }
        }

        public async Task<BridgeCardApiResponse<FundCardResponse>> UnloadCardAsync(UnloadCardRequest request)
        {
            try
            {
                _logger.LogInformation("Unloading card: {CardId} with amount: {Amount}", request.card_id, request.amount);

                var endpoint = $"{_baseUrl}/cards/unload_card_asynchronously";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<FundCardResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unloading card: {CardId}", request.card_id);
                return CreateErrorResponse<FundCardResponse>("An error occurred while unloading the card");
            }
        }

        public async Task<BridgeCardApiResponse<FundCardResponse>> MockDebitTransactionAsync(MockDebitTransactionRequest request)
        {
            try
            {
                _logger.LogInformation("Creating mock debit transaction for card: {CardId}", request.card_id);

                var endpoint = $"{_baseUrl}/cards/mock_debit_transaction";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<FundCardResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating mock debit transaction for card: {CardId}", request.card_id);
                return CreateErrorResponse<FundCardResponse>("An error occurred while creating mock debit transaction");
            }
        }

        public async Task<BridgeCardApiResponse<CardTransactionsResponse>> GetCardTransactionsAsync(string cardId, int page, string? startDate = null, string? endDate = null)
        {
            try
            {
                _logger.LogInformation("Getting transactions for card: {CardId}, page: {Page}", cardId, page);

                var queryParams = new List<string> { $"card_id={cardId}", $"page={page}" };
                if (!string.IsNullOrEmpty(startDate)) queryParams.Add($"start_date={startDate}");
                if (!string.IsNullOrEmpty(endDate)) queryParams.Add($"end_date={endDate}");

                var endpoint = $"{_baseUrl}/cards/get_card_transactions?{string.Join("&", queryParams)}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardTransactionsResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transactions for card: {CardId}", cardId);
                return CreateErrorResponse<CardTransactionsResponse>("An error occurred while getting card transactions");
            }
        }

        public async Task<BridgeCardApiResponse<CardTransaction>> GetCardTransactionByIdAsync(string cardId, string? clientTransactionReference = null, string? bridgecardTransactionReference = null)
        {
            try
            {
                _logger.LogInformation("Getting transaction by ID for card: {CardId}", cardId);

                var queryParams = new List<string> { $"card_id={cardId}" };
                if (!string.IsNullOrEmpty(clientTransactionReference))
                    queryParams.Add($"client_transaction_reference={clientTransactionReference}");
                if (!string.IsNullOrEmpty(bridgecardTransactionReference))
                    queryParams.Add($"bridgecard_transaction_reference={bridgecardTransactionReference}");

                var endpoint = $"{_baseUrl}/cards/get_card_transaction_by_id?{string.Join("&", queryParams)}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardTransaction>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction by ID for card: {CardId}", cardId);
                return CreateErrorResponse<CardTransaction>("An error occurred while getting transaction details");
            }
        }

        public async Task<BridgeCardApiResponse<TransactionStatusResponse>> GetCardTransactionStatusAsync(string cardId, string clientTransactionReference)
        {
            try
            {
                _logger.LogInformation("Getting transaction status for card: {CardId}, reference: {Reference}", cardId, clientTransactionReference);

                var endpoint = $"{_baseUrl}/cards/get_card_transaction_status?card_id={cardId}&client_transaction_reference={clientTransactionReference}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<TransactionStatusResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting transaction status for card: {CardId}", cardId);
                return CreateErrorResponse<TransactionStatusResponse>("An error occurred while getting transaction status");
            }
        }

        public async Task<BridgeCardApiResponse<CardActionResponse>> FreezeCardAsync(string cardId)
        {
            try
            {
                _logger.LogInformation("Freezing card: {CardId}", cardId);

                var endpoint = $"{_baseUrl}/cards/freeze_card?card_id={cardId}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Patch, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardActionResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error freezing card: {CardId}", cardId);
                return CreateErrorResponse<CardActionResponse>("An error occurred while freezing the card");
            }
        }

        public async Task<BridgeCardApiResponse<CardActionResponse>> UnfreezeCardAsync(string cardId)
        {
            try
            {
                _logger.LogInformation("Unfreezing card: {CardId}", cardId);

                var endpoint = $"{_baseUrl}/cards/unfreeze_card?card_id={cardId}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Patch, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardActionResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfreezing card: {CardId}", cardId);
                return CreateErrorResponse<CardActionResponse>("An error occurred while unfreezing the card");
            }
        }

        public async Task<BridgeCardApiResponse<AllCardholderCardsResponse>> GetAllCardholderCardsAsync(string cardholderId)
        {
            try
            {
                _logger.LogInformation("Getting all cards for cardholder: {CardholderId}", cardholderId);

                var endpoint = $"{_baseUrl}/cards/get_all_cardholder_cards?cardholder_id={cardholderId}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<AllCardholderCardsResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cards for cardholder: {CardholderId}", cardholderId);
                return CreateErrorResponse<AllCardholderCardsResponse>("An error occurred while getting cardholder cards");
            }
        }

        public async Task<BridgeCardApiResponse<object>> DeleteCardAsync(string cardId)
        {
            try
            {
                _logger.LogInformation("Deleting card: {CardId}", cardId);

                var endpoint = $"{_baseUrl}/cards/delete_card/{cardId}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Delete, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<object>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting card: {CardId}", cardId);
                return CreateErrorResponse<object>("An error occurred while deleting the card");
            }
        }

        public async Task<BridgeCardApiResponse<object>> UpdateCardPinAsync(UpdateCardPinRequest request)
        {
            try
            {
                _logger.LogInformation("Updating PIN for card: {CardId}", request.card_id);

                var endpoint = $"{_baseUrl}/cards/update_card_pin";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Post, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<object>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating card PIN for card: {CardId}", request.card_id);
                return CreateErrorResponse<object>("An error occurred while updating the card PIN");
            }
        }

        // PIN encryption utility method
        public string EncryptPin(string pin, string secretKey)
        {
            try
            {
                // This is a placeholder for AES-256 encryption
                // You need to install the AES-Everywhere NuGet package or implement AES-256 encryption
                // For now, returning a placeholder
                _logger.LogWarning("PIN encryption not implemented. Install AES-Everywhere or implement AES-256 encryption.");
                return pin; // Replace with actual encryption
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error encrypting PIN");
                throw new InvalidOperationException("Failed to encrypt PIN", ex);
            }
        }

        // Helper methods that should be implemented in the base service class
        //private async Task<BridgeCardApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response, string responseContent)
        //{
        //    if (response.IsSuccessStatusCode)
        //    {
        //        try
        //        {
        //            var apiResponse = JsonSerializer.Deserialize<BridgeCardApiResponse<T>>(responseContent, new JsonSerializerOptions
        //            {
        //                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        //                PropertyNameCaseInsensitive = true
        //            });

        //            return apiResponse ?? CreateErrorResponse<T>("Failed to deserialize response");
        //        }
        //        catch (JsonException ex)
        //        {
        //            _logger.LogError(ex, "Failed to deserialize BridgeCard API response: {ResponseContent}", responseContent);
        //            return CreateErrorResponse<T>("Invalid response format from BridgeCard API");
        //        }
        //    }
        //    else
        //    {
        //        try
        //        {
        //            var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, new JsonSerializerOptions
        //            {
        //                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
        //                PropertyNameCaseInsensitive = true
        //            });

        //            return CreateErrorResponse<T>(errorResponse?.message ?? $"API call failed with status {response.StatusCode}");
        //        }
        //        catch (JsonException)
        //        {
        //            return CreateErrorResponse<T>($"API call failed with status {response.StatusCode}: {responseContent}");
        //        }
        //    }
        //}

        //private BridgeCardApiResponse<T> CreateErrorResponse<T>(string errorMessage)
        //{
        //    return new BridgeCardApiResponse<T>
        //    {
        //        status = "error",
        //        message = errorMessage,
        //        data = default(T)
        //    };
        //}






        // Misc methods implementation
        public async Task<BridgeCardApiResponse<AllCardholdersResponse>> GetAllCardholdersAsync(int page)
        {
            try
            {
                _logger.LogInformation("Getting all cardholders, page: {Page}", page);

                var endpoint = $"{_baseUrl}/cards/get_all_cardholder?page={page}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<AllCardholdersResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cardholders");
                return CreateErrorResponse<AllCardholdersResponse>("An error occurred while getting all cardholders");
            }
        }

        public async Task<BridgeCardApiResponse<AllCardsResponse>> GetAllCardsAsync(int page)
        {
            try
            {
                _logger.LogInformation("Getting all cards, page: {Page}", page);

                var endpoint = $"{_baseUrl}/cards/get_all_cards?page={page}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<AllCardsResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all cards");
                return CreateErrorResponse<AllCardsResponse>("An error occurred while getting all cards");
            }
        }

        public async Task<BridgeCardApiResponse<object>> FundIssuingWalletAsync(FundIssuingWalletRequest request, string currency)
        {
            try
            {
                _logger.LogInformation("Funding issuing wallet with currency: {Currency}, amount: {Amount}", currency, request.amount);

                var endpoint = $"{_baseUrl}/cards/fund_issuing_wallet?currency={currency}";
                var jsonContent = JsonSerializer.Serialize(request, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
                    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
                });

                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var httpRequest = new HttpRequestMessage(HttpMethod.Patch, endpoint) { Content = content };
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<object>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error funding issuing wallet");
                return CreateErrorResponse<object>("An error occurred while funding the issuing wallet");
            }
        }

        public async Task<BridgeCardApiResponse<IssuingWalletBalanceResponse>> GetIssuingWalletBalanceAsync()
        {
            try
            {
                _logger.LogInformation("Getting issuing wallet balance");

                var endpoint = $"{_baseUrl}/cards/get_issuing_wallet_balance";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<IssuingWalletBalanceResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting issuing wallet balance");
                return CreateErrorResponse<IssuingWalletBalanceResponse>("An error occurred while getting issuing wallet balance");
            }
        }

        public async Task<BridgeCardApiResponse<FxRateResponse>> GetFxRateAsync()
        {
            try
            {
                _logger.LogInformation("Getting FX rates");

                // Note: FX rate endpoint is in production URL format, not sandbox
                var endpoint = "https://issuecards.api.bridgecard.co/v1/issuing/cards/fx-rate";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                // FX rate response has different structure, need custom processing
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var rawResponse = JsonSerializer.Deserialize<JsonElement>(responseContent, options);

                    var result = new BridgeCardApiResponse<FxRateResponse>
                    {
                        status = rawResponse.GetProperty("status").GetString() ?? "success",
                        message = rawResponse.GetProperty("message").GetString() ?? "Rate fetched successfully",
                        data = new FxRateResponse()
                    };

                    // Parse the rates from the data object
                    if (rawResponse.TryGetProperty("data", out var dataElement))
                    {
                        foreach (var property in dataElement.EnumerateObject())
                        {
                            if (decimal.TryParse(property.Value.GetString(), out var rate))
                            {
                                result.data.rates[property.Name] = rate;
                            }
                        }
                    }

                    return result;
                }
                else
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, options);
                    return new BridgeCardApiResponse<FxRateResponse>
                    {
                        status = "error",
                        message = errorResponse?.message ?? "Unknown error occurred",
                        data = null
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting FX rates");
                return CreateErrorResponse<FxRateResponse>("An error occurred while getting FX rates");
            }
        }

        public async Task<BridgeCardApiResponse<StatesResponse>> GetAllStatesAsync(string country)
        {
            try
            {
                _logger.LogInformation("Getting all states for country: {Country}", country);

                var endpoint = $"{_baseUrl}/cardholder/get_all_states?country={Uri.EscapeDataString(country)}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<StatesResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting states for country: {Country}", country);
                return CreateErrorResponse<StatesResponse>("An error occurred while getting states");
            }
        }

        public async Task<BridgeCardApiResponse<CardTokenResponse>> GenerateCardTokenAsync(string cardId)
        {
            try
            {
                _logger.LogInformation("Generating card token for card: {CardId}", cardId);

                // Token generation is in production URL format
                var endpoint = $"https://issuecards.api.bridgecard.co/v1/issuing/cards/generate_token_for_card_details?card_id={cardId}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                httpRequest.Headers.Add("token", $"Bearer {_bearerToken}");

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<CardTokenResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating card token for card: {CardId}", cardId);
                return CreateErrorResponse<CardTokenResponse>("An error occurred while generating card token");
            }
        }

        public async Task<BridgeCardApiResponse<DecryptedCardDetailsResponse>> GetCardDetailsFromTokenAsync(string token)
        {
            try
            {
                _logger.LogInformation("Getting card details from token");

                // Token-based card details endpoint (relay URL)
                var endpoint = $"https://issuecards-api-bridgecard-co.relay.evervault.com/v1/issuing/cards/get_card_details_from_token?token={token}";
                var httpRequest = new HttpRequestMessage(HttpMethod.Get, endpoint);
                // Note: No token header needed for this endpoint

                var response = await _httpClient.SendAsync(httpRequest);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"BridgeCard API response: {response.StatusCode}");

                return await ProcessResponse<DecryptedCardDetailsResponse>(response, responseContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting card details from token");
                return CreateErrorResponse<DecryptedCardDetailsResponse>("An error occurred while getting card details from token");
            }
        }

        // Helper methods
        private async Task<BridgeCardApiResponse<T>> ProcessResponse<T>(HttpResponseMessage response, string responseContent)
        {
            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                var result = JsonSerializer.Deserialize<BridgeCardApiResponse<T>>(responseContent, options);
                return result ?? new BridgeCardApiResponse<T>();
            }
            else
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var errorResponse = JsonSerializer.Deserialize<BridgeCardErrorResponse>(responseContent, options);
                return new BridgeCardApiResponse<T>
                {
                    status = "error",
                    message = errorResponse?.message ?? "Unknown error occurred",
                    data = default(T)
                };
            }
        }

        private BridgeCardApiResponse<T> CreateErrorResponse<T>(string message)
        {
            return new BridgeCardApiResponse<T>
            {
                status = "error",
                message = message,
                data = default(T)
            };
        }



    }
}