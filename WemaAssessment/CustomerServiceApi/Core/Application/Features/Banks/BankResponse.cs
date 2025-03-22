namespace CustomerServiceApi.Core.Application.Features.Banks
{
    public class BankResponse
    {
        public List<BankResult> Result { get; set; } = new();
        public string ErrorMessage { get; set; } = string.Empty;
        public List<string> ErrorMessages { get; set; } = new();
        public bool HasError { get; set; }
        public DateTime TimeGenerated { get; set; }
    }

    public class BankResult
    {
        public string BankName { get; set; } = string.Empty;
        public string BankCode { get; set; } = string.Empty;
        public string BankLogo { get; set; } = string.Empty;
    }


}
