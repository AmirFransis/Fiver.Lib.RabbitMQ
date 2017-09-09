using Fiver.Lib.RabbitMQ.Common;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace Fiver.Lib.RabbitMQ
{
    public sealed class RabbitMqService<T> : IRabbitMqService<T> where T : class
    {
        #region " Public "
        
        public RabbitMqService(RabbitMqSettings settings)
        {
            this.settings = settings;
            Init();
        }

        public void Send(T item)
        {
            Send(item, null);
        }

        public void Send(T item, Dictionary<string, object> headers)
        {
            // Get Message Bytes
            var json = JsonConvert.SerializeObject(item);
            var stream = Encoding.UTF8.GetBytes(json);

            // Create Message Properties
            var props = channel.CreateBasicProperties();
            props.Persistent = true;
            props.ContentType = "text/plain";
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    props.Headers.Add(header.Key, header.Value);
                }
            }

            // Create Address to Publish
            var address = new PublicationAddress(settings.Type,
                                    settings.Exchange, settings.RoutingKey);

            // Send Message
            channel.BasicPublish(address, props, stream);

            // Close
            channel.Close();
            connection.Close();
        }

        public void Receive(
            Func<T, MessageProcessResponse> onProcess,
            Action<Exception> onError,
            Action onWait)
        {
            //Set Quality of Service (QOS)
            channel.BasicQos(0, 1, false);

            //Receive
            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (sender, args) =>
            {
                try
                {
                    // Process message from queue.
                    var stream = args.Body;
                    var data = Encoding.UTF8.GetString(stream);

                    // Deserialize
                    T item = JsonConvert.DeserializeObject<T>(data);
                    var result = onProcess(item);

                    // Remove message from queue or put back
                    if (result == MessageProcessResponse.Complete)
                        channel.BasicAck(args.DeliveryTag, false);
                    else if (result == MessageProcessResponse.Dead
                            || result == MessageProcessResponse.Abandon)
                        channel.BasicReject(args.DeliveryTag, false); 
                    
                    // Wait
                    onWait();
                }
                catch (Exception ex)
                {
                    channel.BasicReject(args.DeliveryTag, false);
                    onError(ex);
                }
            };

            channel.BasicConsume(settings.Queue, false, consumer);
        }
        
        #endregion

        #region " Private "

        private RabbitMqSettings settings;
        private IConnection connection;
        private IModel channel;

        private void Init()
        {
            // Create Connection
            ConnectionFactory connectionFactory = new ConnectionFactory();
            connectionFactory.HostName = settings.Hostname;
            connectionFactory.UserName = settings.Username;
            connectionFactory.Password = settings.Password;
            connectionFactory.Port = settings.Port;
            connection = connectionFactory.CreateConnection();

            // Create Channel
            channel = connection.CreateModel();

            // Create Exchange
            channel.ExchangeDeclare(settings.Exchange, settings.Type, true, false, null);

            // Create Queue 
            // Topic/Fanout : default queue in case on one is listening when we send
            channel.QueueDeclare(settings.Queue, true, false, false, null);

            // Create Binding
            channel.QueueBind(settings.Queue, settings.Exchange, settings.RoutingKey);
        }

        #endregion
    }
}
