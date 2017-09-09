using System;
using System.Collections.Generic;
using Fiver.Lib.RabbitMQ.Common;

namespace Fiver.Lib.RabbitMQ
{
    public interface IRabbitMqService<T>
    {
        void Send(T item);
        void Send(T item, Dictionary<string, object> headers);
        void Receive(
            Func<T, MessageProcessResponse> onProcess, 
            Action<Exception> onError, 
            Action onWait);
    }
}