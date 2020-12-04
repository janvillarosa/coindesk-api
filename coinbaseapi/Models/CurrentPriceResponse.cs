namespace coinbaseapi.Models
{
    public class Coin
    {
        public float nzd { get; set; }
        public int last_updated_at { get; set; }
    }

    public class CurrentPriceResponse
    {
        public Coin bitcoin { get; set; }
        public Coin ethereum { get; set; }
    }
}