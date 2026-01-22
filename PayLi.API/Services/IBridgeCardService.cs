using PayLi.API.Models;

namespace PayLi.API.Services
{
    public interface IBridgeCardService
    {
        Task<BridgeCardApiResponse<RegisterCardholderResponse>> RegisterCardholderSynchronouslyAsync(RegisterCardholderRequest request);
        Task<BridgeCardApiResponse<RegisterCardholderResponse>> RegisterCardholderAsynchronouslyAsync(RegisterCardholderRequest request);
        Task<BridgeCardApiResponse<CardholderDetailsResponse>> GetCardholderDetailsAsync(string cardholderId);
        Task<BridgeCardApiResponse<object>> DeleteCardholderAsync(string cardholderId);



        // Card management methods
        Task<BridgeCardApiResponse<CreateCardResponse>> CreateCardAsync(CreateCardRequest request);
        Task<BridgeCardApiResponse<CreateCardResponse>> ActivatePhysicalCardAsync(ActivatePhysicalCardRequest request);
        Task<BridgeCardApiResponse<CardDetailsResponse>> GetCardDetailsAsync(string cardId, bool decryptedEndpoint = false);
        Task<BridgeCardApiResponse<CardBalanceResponse>> GetCardBalanceAsync(string cardId);
        Task<BridgeCardApiResponse<FundCardResponse>> FundCardAsync(FundCardRequest request);
        Task<BridgeCardApiResponse<FundCardResponse>> UnloadCardAsync(UnloadCardRequest request);
        Task<BridgeCardApiResponse<FundCardResponse>> MockDebitTransactionAsync(MockDebitTransactionRequest request);
        Task<BridgeCardApiResponse<CardTransactionsResponse>> GetCardTransactionsAsync(string cardId, int page, string? startDate = null, string? endDate = null);
        Task<BridgeCardApiResponse<CardTransaction>> GetCardTransactionByIdAsync(string cardId, string? clientTransactionReference = null, string? bridgecardTransactionReference = null);
        Task<BridgeCardApiResponse<TransactionStatusResponse>> GetCardTransactionStatusAsync(string cardId, string clientTransactionReference);
        Task<BridgeCardApiResponse<CardActionResponse>> FreezeCardAsync(string cardId);
        Task<BridgeCardApiResponse<CardActionResponse>> UnfreezeCardAsync(string cardId);
        Task<BridgeCardApiResponse<AllCardholderCardsResponse>> GetAllCardholderCardsAsync(string cardholderId);
        Task<BridgeCardApiResponse<object>> DeleteCardAsync(string cardId);
        Task<BridgeCardApiResponse<object>> UpdateCardPinAsync(UpdateCardPinRequest request);



        // Misc methods
        Task<BridgeCardApiResponse<AllCardholdersResponse>> GetAllCardholdersAsync(int page);
        Task<BridgeCardApiResponse<AllCardsResponse>> GetAllCardsAsync(int page);
        Task<BridgeCardApiResponse<object>> FundIssuingWalletAsync(FundIssuingWalletRequest request, string currency);
        Task<BridgeCardApiResponse<IssuingWalletBalanceResponse>> GetIssuingWalletBalanceAsync();
        Task<BridgeCardApiResponse<FxRateResponse>> GetFxRateAsync();
        Task<BridgeCardApiResponse<StatesResponse>> GetAllStatesAsync(string country);
        Task<BridgeCardApiResponse<CardTokenResponse>> GenerateCardTokenAsync(string cardId);
        Task<BridgeCardApiResponse<DecryptedCardDetailsResponse>> GetCardDetailsFromTokenAsync(string token);


    }
}
