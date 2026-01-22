namespace PayLi.API.Models
{
    // Card Request Models
    public class CreateCardRequest
    {
        public string cardholder_id { get; set; } = string.Empty;
        public string card_type { get; set; } = string.Empty; // "virtual" or "physical"
        public string card_brand { get; set; } = string.Empty; // "Mastercard"
        public string card_currency { get; set; } = string.Empty; // "USD"
        public string card_limit { get; set; } = string.Empty; // "500000" or "1000000"
        public string? transaction_reference { get; set; }
        public string funding_amount { get; set; } = string.Empty; // minimum $3 for 5k limit, $4 for 10k limit
        public string pin { get; set; } = string.Empty; // AES 256 encrypted 4 digit pin
        public Dictionary<string, object>? meta_data { get; set; }
    }

    public class ActivatePhysicalCardRequest
    {
        public string cardholder_id { get; set; } = string.Empty;
        public string card_type { get; set; } = "physical";
        public string card_brand { get; set; } = "Mastercard";
        public string card_currency { get; set; } = "USD";
        public string card_token_number { get; set; } = string.Empty; // 13 digit number from card envelope
        public Dictionary<string, object>? meta_data { get; set; }
    }

    public class FundCardRequest
    {
        public string card_id { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty; // in cents
        public string transaction_reference { get; set; } = string.Empty; // must be unique
        public string currency { get; set; } = "USD";
    }

    public class UnloadCardRequest
    {
        public string card_id { get; set; } = string.Empty;
        public string amount { get; set; } = string.Empty; // in cents
        public string transaction_reference { get; set; } = string.Empty; // must be unique
        public string currency { get; set; } = "USD";
    }

    public class MockDebitTransactionRequest
    {
        public string card_id { get; set; } = string.Empty;
    }

    public class UpdateCardPinRequest
    {
        public string card_id { get; set; } = string.Empty;
        public string card_pin { get; set; } = string.Empty; // AES 256 encrypted 4 digit pin
    }

    // Card Response Models
    public class CreateCardResponse
    {
        public string card_id { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
    }

    public class CardDetailsResponse
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

    public class BillingAddress
    {
        public string billing_address1 { get; set; } = string.Empty;
        public string billing_city { get; set; } = string.Empty;
        public string billing_country { get; set; } = string.Empty;
        public string billing_zip_code { get; set; } = string.Empty;
        public string country_code { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string state_code { get; set; } = string.Empty;
    }

    public class CardBalanceResponse
    {
        public string card_id { get; set; } = string.Empty;
        public string balance { get; set; } = string.Empty;
        public string settled_available_balance { get; set; } = string.Empty;
        public string settled_book_balance { get; set; } = string.Empty;
    }

    public class FundCardResponse
    {
        public string card_id { get; set; } = string.Empty;
        public string transaction_reference { get; set; } = string.Empty;
    }

    public class CardTransactionsResponse
    {
        public List<CardTransaction> transactions { get; set; } = new();
        public TransactionMeta meta { get; set; } = new();
    }

    public class CardTransaction
    {
        public string amount { get; set; } = string.Empty;
        public string bridgecard_transaction_reference { get; set; } = string.Empty;
        public string card_id { get; set; } = string.Empty;
        public string card_transaction_type { get; set; } = string.Empty; // "DEBIT" or "CREDIT"
        public string cardholder_id { get; set; } = string.Empty;
        public string client_transaction_reference { get; set; } = string.Empty;
        public string currency { get; set; } = string.Empty;
        public string description { get; set; } = string.Empty;
        public string issuing_app_id { get; set; } = string.Empty;
        public bool livemode { get; set; }
        public string transaction_date { get; set; } = string.Empty;
        public long transaction_timestamp { get; set; }
        public string? merchant_category_code { get; set; }
        public EnrichedData? enriched_data { get; set; }
        public string? partner_interchange_fee { get; set; }
        public string? interchange_revenue { get; set; }
        public string? partner_interchange_fee_refund { get; set; }
        public string? interchange_revenue_refund { get; set; }
    }

    public class EnrichedData
    {
        public bool is_recurring { get; set; }
        public string? merchant_city { get; set; }
        public string? merchant_code { get; set; }
        public string? merchant_logo { get; set; }
        public string? merchant_name { get; set; }
        public string? merchant_website { get; set; }
        public string? transaction_category { get; set; }
        public string? transaction_group { get; set; }
    }

    public class TransactionMeta
    {
        public int total { get; set; }
        public int pages { get; set; }
        public string? previous { get; set; }
        public string? next { get; set; }
    }

    public class TransactionStatusResponse
    {
        public string transaction_status { get; set; } = string.Empty; // "PENDING", "SUCCESSFUL"
    }

    public class CardActionResponse
    {
        public string card_id { get; set; } = string.Empty;
    }

    public class AllCardholderCardsResponse
    {
        public List<CardDetailsResponse> cards { get; set; } = new();
        public int total { get; set; }
    }

    // Constants
    public static class CardTypes
    {
        public const string VIRTUAL = "virtual";
        public const string PHYSICAL = "physical";
    }

    public static class CardBrands
    {
        public const string MASTERCARD = "Mastercard";
    }

    public static class CardCurrencies
    {
        public const string USD = "USD";
    }

    public static class CardLimits
    {
        public const string LIMIT_5K = "500000"; // $5,000
        public const string LIMIT_10K = "1000000"; // $10,000
        public const int MIN_FUNDING_5K = 300; // $3 in cents
        public const int MIN_FUNDING_10K = 400; // $4 in cents
    }

    public static class TransactionTypes
    {
        public const string DEBIT = "DEBIT";
        public const string CREDIT = "CREDIT";
    }

    public static class TransactionStatus
    {
        public const string PENDING = "PENDING";
        public const string SUCCESSFUL = "SUCCESSFUL";
        public const string FAILED = "FAILED";
    }
}
