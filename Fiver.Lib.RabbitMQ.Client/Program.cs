using Fiver.Lib.RabbitMQ.Common;
using System;


namespace Fiver.Lib.RabbitMQ.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // Queue
                //Queue_Send();
                //Queue_Receive();

                // Topic
                //Topic_Send();
                //Topic_Receive();

                // Fanout
                //Fanout_Send();
                //Fanout_Receive();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.ReadLine();
            }
        }

        #region " Queue "

        private static void Queue_Send()
        {
            var settings = RabbitMqSettings.ForQueue(
                hostname: "localhost",
                username: "guest",
                password: "guest",
                port: 5672,
                exchange: "e.direct",
                queue: "q.direct");

            var message = new Message { Text = "Hello Queue" };

            IRabbitMqService<Message> sender = new RabbitMqService<Message>(settings);
            sender.Send(message);

            Console.WriteLine("Sent");
        }

        private static void Queue_Receive()
        {
            var settings = RabbitMqSettings.ForQueue(
                hostname: "localhost",
                username: "guest",
                password: "guest",
                port: 5672,
                exchange: "e.direct",
                queue: "q.direct");

            IRabbitMqService<Message> receiver = new RabbitMqService<Message>(settings);
            receiver.Receive(
                message =>
                {
                    Console.WriteLine(message.Text);
                    return MessageProcessResponse.Complete;
                },
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Waiting..."));
        }

        #endregion

        #region " Topic "

        private static void Topic_Send()
        {
            var settings = RabbitMqSettings.ForTopic(
                hostname: "localhost",
                username: "guest",
                password: "guest",
                port: 5672,
                exchange: "e.topic",
                queue: "q.topic",
                routingKey: "#");

            var message = new Message { Text = "Hello Topic" };

            IRabbitMqService<Message> sender = new RabbitMqService<Message>(settings);
            sender.Send(message);

            Console.WriteLine("Sent");
        }

        private static void Topic_Receive()
        {
            var settings = RabbitMqSettings.ForTopic(
                hostname: "localhost",
                username: "guest",
                password: "guest",
                port: 5672,
                exchange: "e.topic",
                queue: "q.topic",
                routingKey: "#");

            IRabbitMqService<Message> receiver = new RabbitMqService<Message>(settings);
            receiver.Receive(
                message =>
                {
                    Console.WriteLine(message.Text);
                    return MessageProcessResponse.Complete;
                },
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Waiting..."));
        }

        #endregion

        #region " Fanout "

        private static void Fanout_Send()
        {
            var settings = RabbitMqSettings.ForFanout(
                hostname: "localhost",
                username: "guest",
                password: "guest",
                port: 5672,
                exchange: "e.fanout",
                queue: "q1.fanout");

            var message = new Message { Text = "Hello Fanout" };

            IRabbitMqService<Message> sender = new RabbitMqService<Message>(settings);
            sender.Send(message);

            Console.WriteLine("Sent");
        }

        private static void Fanout_Receive()
        {
            var settings = RabbitMqSettings.ForFanout(
                hostname: "localhost",
                username: "guest",
                password: "guest",
                port: 5672,
                exchange: "e.fanout",
                queue: "q1.fanout");

            IRabbitMqService<Message> receiver = new RabbitMqService<Message>(settings);
            receiver.Receive(
                message =>
                {
                    Console.WriteLine(message.Text);
                    return MessageProcessResponse.Complete;
                },
                ex => Console.WriteLine(ex.Message),
                () => Console.WriteLine("Waiting..."));
        }

        #endregion
    }
}
