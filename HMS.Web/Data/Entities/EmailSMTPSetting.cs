namespace HMS.Web.Data.Entities
{
    public class EmailSMTPSetting
    {
        public Guid Id { get; set; }
        public string Host { get; set; }
        public bool IsEnableSSL { get; set; }
        public int Port { get; set; }
        public bool IsDefault { get; set; }
    }
}
