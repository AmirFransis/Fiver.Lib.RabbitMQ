using RabbitMQ.Client;
using System;

namespace Fiver.Lib.RabbitMQ
{
    public sealed class RabbitMqSettings
    {
        private RabbitMqSettings(string hostname, string username, 
            string password, int port, string exchange, string queue, 
            string exchangeType, string routingKey)
        {
            if (string.IsNullOrEmpty(hostname))
                throw new ArgumentNullException("hostname");

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password");

            if (port == 0)
                throw new ArgumentNullException("port");

            if (string.IsNullOrEmpty(exchange))
                throw new ArgumentNullException("exchange");

            if (string.IsNullOrEmpty(queue))
                throw new ArgumentNullException("queue");

            if (string.IsNullOrEmpty(exchangeType))
                throw new ArgumentNullException("exchangeType");
            
            this.Hostname = hostname;
            this.Username = username;
            this.Password = password;
            this.Port = port;
            this.Exchange = exchange;
            this.Queue = queue;
            this.Type = exchangeType;
            this.RoutingKey = routingKey;
        }

        public string Hostname { get; }
        public string Username { get; }
        public string Password { get; }
        public int Port { get; }
        public string Exchange { get; }
        public string Queue { get; }
        public string Type { get; }
        public string RoutingKey { get; }

        public static RabbitMqSettings ForQueue(string hostname, string username,
            string password, int port, string exchange, string queue)
        {
            return new RabbitMqSettings(hostname, username, password, 
                port, exchange, queue, ExchangeType.Direct, "");
        }

        public static RabbitMqSettings ForTopic(string hostname, string username,
            string password, int port, string exchange, string queue, string routingKey)
        {
            return new RabbitMqSettings(hostname, username, password,
                port, exchange, queue, ExchangeType.Topic, routingKey);
        }

        public static RabbitMqSettings ForFanout(string hostname, string username,
            string password, int port, string exchange, string queue)
        {
            return new RabbitMqSettings(hostname, username, password,
                port, exchange, queue, ExchangeType.Fanout, "");
        }
    }
}
