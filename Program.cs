using System;
using System.Threading.Tasks;
using MassTransit;

namespace MassTransit7RabbitMQ
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var bus = Bus.Factory.CreateUsingRabbitMq(sbc =>
            {
                //sbc.Host("rabbitmq://localhost:5672");

                sbc.Host("localhost", 5673, "notified-integration", hc =>
                {
                    hc.Username("notified_test");
                    hc.Password("secret");
                });

                sbc.ReceiveEndpoint("test_queue", ep =>
                {
                    ep.Handler<TestCommand>(context => Console.Out.WriteLineAsync($"Received: {context.Message.Text}"));
                });
            });

            await bus.StartAsync();

            await bus.Publish(new TestCommand { Text = "Run!" });

            Console.WriteLine("Press the any key");
            await Task.Run(Console.ReadKey);

            await bus.StopAsync();
        }
    }
}
