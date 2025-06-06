using RabbitMQ.Client;

namespace QualityCertificate.Datas.Models
{
    public class RabbitMQClient_Topic : AbstractRabbitMQ
    {
        /// <summary>
        /// Đăng ký Queue
        /// </summary>
        /// <param name="hostName">Tên server RabbitMQ</param>
        /// <param name="userName">Tên đăng nhập vào RabbitMQ</param>
        /// <param name="passWord">Tài khoản đăng nhập vào RabbitMQ</param>
        /// <param name="exchangeName">Tên Exchange của RabbitMQ (xuất hàng dùng hoangthach_delivery_event_bus)</param>

        public RabbitMQClient_Topic(IConfiguration configuration)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = configuration["RabbitMQ:HostName"],
                    UserName = configuration["RabbitMQ:UserName"],
                    Password = configuration["RabbitMQ:Password"]

                };
                this.ExchangeName = configuration["RabbitMQ:ExchangeName_Topic"];
                this._exchangeType = ExchangeType.Topic;
                this.HostName = configuration["RabbitMQ:HostName"];
                this.Username = configuration["RabbitMQ:UserName"];
                this.Password = configuration["RabbitMQ:Password"];

                //factory.RequestedHeartbeat = 5;
                factory.AutomaticRecoveryEnabled = true;
                this.Connection = factory.CreateConnection();
                Channel = Connection.CreateModel();
                Channel.ExchangeDeclare(exchange: this.ExchangeName, type: _exchangeType);
            }
            catch (Exception ex)
            {
                if (ErrorEventHandler != null)
                    ErrorEventHandler(this, ex.Message);
            }
        }


    }
}
