using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanDisher
{
    public class ServiceProvider
    {
        public HubConnection Connection { get => _connection; set => _connection = value; }

        public Action ReconnectAction { get; set; }
        HubConnection _connection;

        public ServiceProvider()
        {
            _connection = new HubConnectionBuilder()
        .WithUrl("http://119.207.78.147:5001/data")
        //.WithUrl("http://112.144.208.123:5001/data")
        .WithAutomaticReconnect()
        .Build();


            _connection.Reconnected += (connectionId) =>
            {
                ReconnectAction?.Invoke();
                return null;
            };

            _connection.StartAsync();



        }

    }
}
