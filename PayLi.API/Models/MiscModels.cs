namespace PayLi.API.Models
{
    // Misc Request Models
    public class FundIssuingWalletRequest
    {
        public string amount { get; set; } = string.Empty;
    }

    // Misc Response Models
    public class AllCardholdersResponse
    {
        public List<CardholderSummary> cardholders { get; set; } = new();
        public PaginationMeta meta { get; set; } = new();
    }

    public class CardholderSummary
    {
        public CardholderAddress address { get; set; } = new();
        public string cardholder_id { get; set; } = string.Empty;
        public long created_at { get; set; }
        public string email_address { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
        public bool is_active { get; set; }
        public bool is_id_verified { get; set; }
        public string issuing_app_id { get; set; } = string.Empty;
        public CardholderIdentityDetails identity_details { get; set; } = new();
        public Dictionary<string, object>? meta_data { get; set; }
    }

    public class AllCardsResponse
    {
        public List<CardSummary> cards { get; set; } = new();
        public PaginationMeta meta { get; set; } = new();
    }

    public class CardSummary
    {
        public BillingAddress billing_address { get; set; } = new();
        public string brand { get; set; } = string.Empty;
        public string card_currency { get; set; } = string.Empty;
        public string card_id { get; set; } = string.Empty;
        public string card_name { get; set; } = string.Empty;
        public string card_number { get; set; } = string.Empty; // encrypted
        public string card_type { get; set; } = string.Empty;
        public string cardholder_id { get; set; } = string.Empty;
        public long created_at { get; set; }
        public string cvv { get; set; } = string.Empty; // encrypted
        public string expiry_month { get; set; } = string.Empty; // encrypted
        public string expiry_year { get; set; } = string.Empty; // encrypted
        public bool is_active { get; set; }
        public string issuing_app_id { get; set; } = string.Empty;
        public string last_4 { get; set; } = string.Empty;
        public bool livemode { get; set; }
        public Dictionary<string, object>? meta_data { get; set; }
    }

    public class PaginationMeta
    {
        public int total { get; set; }
        public int pages { get; set; }
        public string? previous { get; set; }
        public string? next { get; set; }
    }

    public class IssuingWalletBalanceResponse
    {
        public string issuing_balance_USD { get; set; } = string.Empty;
        public string? issuing_balance_NGN { get; set; }
    }

    public class FxRateResponse
    {
        public Dictionary<string, decimal> rates { get; set; } = new();
    }

    public class StatesResponse
    {
        public List<string> states { get; set; } = new();
    }

    public class CardTokenResponse
    {
        public string token { get; set; } = string.Empty;
    }

    public class DecryptedCardDetailsResponse
    {
        public BillingAddress billing_address { get; set; } = new();
        public string brand { get; set; } = string.Empty;
        public string card_currency { get; set; } = string.Empty;
        public string card_id { get; set; } = string.Empty;
        public string card_name { get; set; } = string.Empty;
        public string card_number { get; set; } = string.Empty; // decrypted
        public string card_type { get; set; } = string.Empty;
        public string cardholder_id { get; set; } = string.Empty;
        public long created_at { get; set; }
        public string cvv { get; set; } = string.Empty; // decrypted
        public string expiry_month { get; set; } = string.Empty; // decrypted
        public string expiry_year { get; set; } = string.Empty; // decrypted
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public string issuing_app_id { get; set; } = string.Empty;
        public string last_4 { get; set; } = string.Empty;
        public bool livemode { get; set; }
        public Dictionary<string, object>? meta_data { get; set; }
        public string balance { get; set; } = string.Empty;
        public string available_balance { get; set; } = string.Empty;
        public string book_balance { get; set; } = string.Empty;
        public bool blocked_due_to_fraud { get; set; }
        public bool pin_3ds_activated { get; set; }
    }

    // Constants for supported currencies
    public static class SupportedCurrencies
    {
        public const string USD = "USD";
        public const string NGN = "NGN";
    }

    // Constants for supported countries for states
    public static class CountriesWithStates
    {
        public const string NIGERIA = "Nigeria";
        public const string GHANA = "Ghana";
        public const string UGANDA = "Uganda";
        public const string KENYA = "Kenya";
        public const string EGYPT = "Egypt";
    }
}
