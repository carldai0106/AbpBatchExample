namespace Abp
{
    public class AbpExtendedConsts
    {
        public const string LocalizationSourceName = "AbpExtended";
        public const string ExchangeName = "Abp.Net.Mail.RabbitMQ";
        public const string RabbitMQEx_ExchangeName = "Abp.RabbitMQEx.Exchange";
        public const string RabbitMQEx_QueueName = "Abp.RabbitMQEx.Queue";

        /// <summary>
        /// Default page size for paged requests.
        /// </summary>
        public const int DefaultPageSize = 10;

        /// <summary>
        /// Maximum allowed page size for paged requests.
        /// </summary>
        public const int MaxPageSize = 1000;
    }
}
