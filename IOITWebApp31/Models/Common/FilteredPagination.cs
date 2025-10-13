namespace IOITWebApp31.Models
{
    public class FilteredPagination : BasePagination
    {
        [System.ComponentModel.DefaultValue("")]
        public string query { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string select { get; set; }

        [System.ComponentModel.DefaultValue("")]
        public string search { get; set; }
    }
}