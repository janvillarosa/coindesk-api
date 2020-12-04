using System;

namespace coinbaseapi.Models
{
    public class Price
    {
        public float Value { get; set; }
        public DateTime Date { get; set; }
        public string Currency { get; set; }
    }
}
