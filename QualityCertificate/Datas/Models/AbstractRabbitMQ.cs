using Microsoft.AspNetCore.Connections;
using Microsoft.EntityFrameworkCore.Metadata;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using IModel = RabbitMQ.Client.IModel;

namespace QualityCertificate.Datas.Models
{
    public abstract class AbstractRabbitMQ
    {
        protected IModel Channel { get; set; }
        public IConnection Connection;
        protected string ExchangeName;
        protected string RootingKey;

        protected bool IsConnected = false;

        protected string HostName;
        protected string Username;
        protected string Password;

        public delegate void ArrivedDataEvents(object sender, object e, string rootingKey);
        public ArrivedDataEvents SubcribleEventsHandler;

        public delegate void ErrorHandler(object sender, string errorMessage);
        public ErrorHandler ErrorEventHandler;
        protected string _exchangeType = ExchangeType.Direct;
        private List<string> _lstConsumerTag = new List<string>();
        /// <summary>
        /// Đẩy message vào exchange
        /// </summary>
        /// <param name="routingKey">Key để quy ước với bên nhận cùng Exchange</param>
        /// <param name="data">Object dữ liệu</param>
        /// <param name="persistent">false: không lưu queue vào hdd; true: không mất dl </param>
        public void Publish(string routingKey, object data, bool persistent = false)

        {

            // return;


            try
            {
                RootingKey = routingKey;
                var message = JsonConvert.SerializeObject(data);
                var byteBody = Encoding.UTF8.GetBytes(message);
                if (Connection.IsOpen)
                {
                    var properties = Channel.CreateBasicProperties();
                    properties.Persistent = persistent;
                    Channel.BasicPublish(exchange: ExchangeName,
                            routingKey: routingKey ?? RootingKey, basicProperties: properties,
                            body: byteBody);
                }
            }
            catch (Exception ex)
            {
                if (ErrorEventHandler != null)
                    ErrorEventHandler(this, ex.Message);
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">dữ liệu của sự kiện</typeparam>
        /// <param name="rootingKey"></param>
        /// <param name="queueName">tên queue, nếu null thì đặt tên ngẫu nhiên</param>
        /// <param name="durableQueue">true: không xoá queue khi restart, không xoá;  </param>
        public void Subcrible<T>(string rootingKey, string queueName = null, bool durableQueue = false, bool autoDelete = false, Dictionary<string, object> arguments = null)
        {
            try
            {
                if (Channel == null) return;
                Channel.ExchangeDeclare(exchange: this.ExchangeName,
                    type: _exchangeType);

                if (string.IsNullOrEmpty(queueName))
                    queueName = Channel.QueueDeclare(arguments: arguments).QueueName;
                else
                {
                    if (!durableQueue) // if durable queue remove queue and add again
                    {
                        Channel.QueueDeleteNoWait(queue: queueName,
                            ifUnused: true,
                            ifEmpty: false);
                    }
                    //Channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                    Channel.QueueDeclare(queue: queueName, durable: durableQueue, exclusive: false, autoDelete: autoDelete, arguments: arguments);
                    if (durableQueue) Channel.QueuePurge(queue: queueName);
                }

                Channel.QueueBind(queue: queueName,
                    exchange: this.ExchangeName,
                    routingKey: rootingKey);

                var consumer = new EventingBasicConsumer(Channel);
                consumer.Received += (model, ea) =>
                {
                    //var body = ea.Body;
                    var body = ea.Body.ToArray();
                    if (body == null || body?.Length <= 0) return;

                    var message = Encoding.UTF8.GetString(body);

                    try
                    {
                        if (SubcribleEventsHandler != null)
                        {
                            T obj = JsonConvert.DeserializeObject<T>(message);
                            SubcribleEventsHandler(this, obj, rootingKey);
                        }
                    }
                    catch (Exception ex)
                    {
                        //Debug.WriteLine(string.Format("Lỗi {0} ####({1})", ex.Message, message));
                        if (ErrorEventHandler != null)
                            ErrorEventHandler(this, ex.Message);
                    }
                };

                string ct = Channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                if (!string.IsNullOrWhiteSpace(ct) && !_lstConsumerTag.Any(a => a == ct))
                    _lstConsumerTag.Add(ct);
            }
            catch (Exception ex)
            {
                if (ErrorEventHandler != null)
                    ErrorEventHandler(this, ex.Message);
            }
        }
        /// <summary>
        /// 20240703 Subcrible nhiều routing key trong 1 queue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="routingKeys">Danh sách rooting key</param>
        /// <param name="queueName"></param>
        /// <param name="durableQueue"></param>
        /// <param name="autoDelete"></param>
        /// <param name="arguments"></param>
        public void Subcrible<T>(string[] routingKeys, string queueName = null, bool durableQueue = false, bool autoDelete = false, Dictionary<string, object> arguments = null)
        {
            if (routingKeys == null || routingKeys?.Length <= 0) return;

            try
            {
                if (Channel == null) return;
                Channel.ExchangeDeclare(exchange: this.ExchangeName,
                    type: _exchangeType);

                if (string.IsNullOrEmpty(queueName))
                    queueName = Channel.QueueDeclare(arguments: arguments).QueueName;
                else
                {
                    if (!durableQueue) // if durable queue remove queue and add again
                    {
                        Channel.QueueDeleteNoWait(queue: queueName,
                            ifUnused: true,
                            ifEmpty: false);
                    }
                    //Channel.QueueDeclare(queue: queueName, durable: true, exclusive: false, autoDelete: false, arguments: arguments);
                    Channel.QueueDeclare(queue: queueName, durable: durableQueue, exclusive: false, autoDelete: autoDelete, arguments: arguments);
                    if (durableQueue) Channel.QueuePurge(queue: queueName);
                }

                foreach (var routingKey in routingKeys)
                {

                    Channel.QueueBind(queue: queueName,
                        exchange: this.ExchangeName,
                        routingKey: routingKey);

                    var consumer = new EventingBasicConsumer(Channel);
                    consumer.Received += (model, ea) =>
                    {
                        //var body = ea.Body;
                        var body = ea.Body.ToArray();
                        if (body == null || body?.Length <= 0) return;

                        var message = Encoding.UTF8.GetString(body);

                        try
                        {
                            if (SubcribleEventsHandler != null)
                            {
                                T obj = JsonConvert.DeserializeObject<T>(message);
                                SubcribleEventsHandler(this, obj, routingKey);
                            }
                        }
                        catch (Exception ex)
                        {
                            //Debug.WriteLine(string.Format("Lỗi {0} ####({1})", ex.Message, message));
                            if (ErrorEventHandler != null)
                                ErrorEventHandler(this, ex.Message);
                        }
                    };

                    string ct = Channel.BasicConsume(queue: queueName, autoAck: true, consumer: consumer);
                    if (!string.IsNullOrWhiteSpace(ct) && !_lstConsumerTag.Any(a => a == ct))
                        _lstConsumerTag.Add(ct);
                }
            }
            catch (Exception ex)
            {
                if (ErrorEventHandler != null)
                    ErrorEventHandler(this, ex.Message);
            }
        }

        /// <summary>
        /// Đóng kết nối
        /// </summary>
        public string Disconnect()
        {
            string ret = "";
            try
            {
                foreach (var ct in _lstConsumerTag)
                {
                    Channel.BasicCancel(ct);
                }
                Channel.Close();
                Connection.Close();
                _lstConsumerTag.Clear();
            }
            catch (Exception ex) { ret = ex.Message; }
            return ret;
        }
        /// <summary>
        /// Khởi tạo lại kết nối
        /// </summary>
        public string ReConnect()
        {
            string ret = "";
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = HostName,
                    UserName = Username,
                    Password = Password
                };
                //this.ExchangeName = ExchangeName;
                //this._exchangeType = _exchangeType;

                factory.AutomaticRecoveryEnabled = true;
                this.Connection = factory.CreateConnection();
                Channel = Connection.CreateModel();
                Channel.ExchangeDeclare(exchange: this.ExchangeName, type: _exchangeType);
            }
            catch (Exception ex)
            {
                ret = ex.Message;
                if (ErrorEventHandler != null)
                    ErrorEventHandler(this, ex.Message);
            }
            return ret;
        }
        protected void InitialMQ(string hostName, string userName, string password, string exchangeName, string exchangeType)
        {
            try
            {
                var factory = new ConnectionFactory()
                {
                    HostName = hostName,
                    UserName = userName,
                    Password = password

                };
                this.ExchangeName = exchangeName;
                this._exchangeType = exchangeType;
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
