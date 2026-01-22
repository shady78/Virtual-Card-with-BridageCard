namespace PayLi.API.Models
{
    public class RegisterCardholderRequest
    {
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public CardholderAddress address { get; set; } = new();
        public string phone { get; set; } = string.Empty;
        public string email_address { get; set; } = string.Empty;
        public CardholderIdentity identity { get; set; } = new();
        public Dictionary<string, object>? meta_data { get; set; }
    }
    public class CardholderAddress
    {
        public string address { get; set; } = string.Empty;
        public string city { get; set; } = string.Empty;
        public string state { get; set; } = string.Empty;
        public string country { get; set; } = string.Empty;
        public string postal_code { get; set; } = string.Empty;
        public string house_no { get; set; } = string.Empty;
    }

    public class CardholderIdentity
    {
        public string id_type { get; set; } = string.Empty;
        public string? bvn { get; set; }
        public string? id_no { get; set; }
        public string? selfie_image { get; set; }
        public string? id_image { get; set; }
        public string? first_name { get; set; }
        public string? last_name { get; set; }
        public string? middle_name { get; set; }
        public string? date_of_birth { get; set; }
        public string? gender { get; set; }
    }

    // Response Models
    public class BridgeCardApiResponse<T>
    {
        public string status { get; set; } = string.Empty;
        public string message { get; set; } = string.Empty;
        public T? data { get; set; }
    }

    public class RegisterCardholderResponse
    {
        public string cardholder_id { get; set; } = string.Empty;
    }

    public class CardholderDetailsResponse
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

    public class CardholderIdentityDetails
    {
        public bool blacklisted { get; set; }
        public string date_of_birth { get; set; } = string.Empty;
        public string first_name { get; set; } = string.Empty;
        public string last_name { get; set; } = string.Empty;
        public string gender { get; set; } = string.Empty;
        public string id_no { get; set; } = string.Empty;
        public string id_type { get; set; } = string.Empty;
        public string phone { get; set; } = string.Empty;
    }

    // Error Response Model
    public class BridgeCardErrorResponse
    {
        public string message { get; set; } = string.Empty;
    }

    // Identity Types Constants
    public static class IdentityTypes
    {
        // Nigerian Identity Types
        public const string NIGERIAN_BVN_VERIFICATION = "NIGERIAN_BVN_VERIFICATION";
        public const string NIGERIAN_NIN = "NIGERIAN_NIN";
        public const string NIGERIAN_INTERNATIONAL_PASSPORT = "NIGERIAN_INTERNATIONAL_PASSPORT";
        public const string NIGERIAN_PVC = "NIGERIAN_PVC";
        public const string NIGERIAN_DRIVERS_LICENSE = "NIGERIAN_DRIVERS_LICENSE";

        // Ghanaian Identity Types
        public const string GHANIAN_SSNIT = "GHANIAN_SSNIT";
        public const string GHANIAN_VOTERS_ID = "GHANIAN_VOTERS_ID";
        public const string GHANIAN_DRIVERS_LICENSE = "GHANIAN_DRIVERS_LICENSE";
        public const string GHANIAN_INTERNATIONAL_PASSPORT = "GHANIAN_INTERNATIONAL_PASSPORT";
        public const string GHANIAN_GHANA_CARD = "GHANIAN_GHANA_CARD";

        // Ugandan Identity Types
        public const string UGANDA_VOTERS_ID = "UGANDA_VOTERS_ID";
        public const string UGANDA_PASSPORT = "UGANDA_PASSPORT";
        public const string UGANDA_NATIONAL_ID = "UGANDA_NATIONAL_ID";
        public const string UGANDA_DRIVERS_LICENSE = "UGANDA_DRIVERS_LICENSE";

        // Kenyan Identity Types
        public const string KENYAN_VOTERS_ID = "KENYAN_VOTERS_ID";
    }

    // Supported Countries
    public static class SupportedCountries
    {
        public const string NIGERIA = "Nigeria";
        public const string GHANA = "Ghana";
        public const string UGANDA = "Uganda";
        public const string KENYA = "Kenya";
    }
}
