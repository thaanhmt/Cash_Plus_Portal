namespace IOITWebApp31.Models
{
    public class DefaultResponse
    {
        public Meta meta { get; set; }
        public object data { get; set; }
        public object metadata { get; set; }
    }

    public class Meta
    {
        public int error_code { get; set; }
        public string error_message { get; set; }

        public Meta()
        { }

        public Meta(int errorCode, string errorMessage)
        {
            this.error_code = errorCode;
            this.error_message = errorMessage;
        }
    }

    public class Metadata
    {
        public int item_count { get; set; }
        public decimal total { get; set; }
        public Metadata()
        { }

        public Metadata(int item_count)
        {
            this.item_count = item_count;
        }
        public Metadata(decimal total)
        {
            this.total = total;
        }

        public Metadata(int item_count, decimal total)
        {
            this.item_count = item_count;
            this.total = total;
        }
    }


    public class MetadataTotal
    {
        public int Count { get; set; }
        public int TotalInit { get; set; }
        public int TotalConfirm { get; set; }
        public int TotalDelivery { get; set; }
        public int TotalDelived { get; set; }
        public int TotalCancel { get; set; }

        public MetadataTotal()
        { }

        public MetadataTotal(int Count, int TotalInit, int TotalConfirm, int TotalDelivery, int TotalDelived, int TotalCancel)
        {
            this.Count = Count;
            this.TotalInit = TotalInit;
            this.TotalConfirm = TotalConfirm;
            this.TotalDelivery = TotalDelivery;
            this.TotalDelived = TotalDelived;
            this.TotalCancel = TotalCancel;
        }

    }


}