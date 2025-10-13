namespace IOITWebApp31.Models
{
    public class BasePagination
    {
        [System.ComponentModel.DefaultValue(1)]
        public int page { get; set; }
        [System.ComponentModel.DefaultValue(0)]
        public int page_size { get; set; }
        public string order_by { get; set; }
    }
}