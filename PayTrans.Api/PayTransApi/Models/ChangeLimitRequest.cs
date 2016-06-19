namespace PayTransApi.Models
{
    public class ChangeLimitRequest
    {
        public int NewLimit { get; set; } 
        public string ChildId { get; set; }
    }
}