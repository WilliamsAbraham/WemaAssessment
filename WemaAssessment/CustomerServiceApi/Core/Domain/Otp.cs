﻿namespace CustomerServiceApi.Core.Domain
{
    public class Otp
    {
        public int Id { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
