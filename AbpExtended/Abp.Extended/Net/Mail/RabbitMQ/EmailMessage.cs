namespace Abp.Net.Mail.RabbitMQ
{
    internal class EmailMessage
    {
        public string FromAddress { get; set; }
        public string To { get; set; }
        public string Body { get; set; }
        public string Subject { get; set; }
    }
}
