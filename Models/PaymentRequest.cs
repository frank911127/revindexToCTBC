namespace RevindexToCTBC.Models
{
    public class PaymentRequest

    {
        public decimal? Amount { get; set; }
        public string BillingCity { get; set; }
        public string BillingCountryCode { get; set; }
        public string BillingDistrict { get; set; } = string.Empty;
        public string BillingEmail { get; set; } = string.Empty;
        public string BillingFirstName { get; set; } = string.Empty;
        public string BillingLastName { get; set; } = string.Empty;
        public string BillingPhone { get; set; } = string.Empty;
        public string BillingPostalCode { get; set; } = string.Empty;
        public string BillingSubdivisionCode { get; set; } = string.Empty;
        public string BillingUnit { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CustomerID { get; set; } = string.Empty;
        public string CustomerIPAddress { get; set; } = string.Empty;
        public string FailureReturnUrl { get; set; } = string.Empty;
        public string NotificationUrl { get; set; } = string.Empty;
        public string OrderID { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ReferenceID { get; set; } = string.Empty;
        public string SuccessReturnUrl { get; set; } = string.Empty;
        public string UICultureCode { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }

}