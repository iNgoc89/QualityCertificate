
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace QualityCertificate.Datas.Models
{
    public class BackgroundTask
    {
        private static BackgroundTask instance;

        public static BackgroundTask getInstance()
        {
            if (instance is null) instance = new BackgroundTask();
            return instance;
        }
        private WebApplication? _app;
        private IConfiguration? _configuration;

        public void Init(WebApplication app,
             IConfiguration configuration
             )
        {
            _app = app;
            _configuration = configuration;
            using var scope = app.Services.CreateScope();
            ReadCard.getInstance().ServiceProvider = app.Services;
            ReadCard.getInstance().Location34= int.Parse(_configuration["LocationPrintQR:34"]);
            ReadCard.getInstance().LocationP3 = int.Parse(_configuration["LocationPrintQR:P3"]);
            
            RegisterRabbitMQ_Topic(app.Services);
        }
        public void RegisterRabbitMQ_Topic(IServiceProvider serviceProvider)
        {
            RabbitMQClient_Topic rabbitMQ = serviceProvider.GetRequiredService<RabbitMQClient_Topic>();

            rabbitMQ.SubcribleEventsHandler += new AbstractRabbitMQ.ArrivedDataEvents(Topic_EventBusData_Handler);

            Dictionary<string, object> arguments = new Dictionary<string, object>();
            arguments.Add("x-max-length", 10);
            arguments.Add("x-message-ttl", 10000);


            rabbitMQ.Subcrible<CardEventMQ>(rootingKey: _configuration["RabbitMQ:PrintQR_RootingKey"], queueName: "CardReader_PrintQR", durableQueue: true, autoDelete: false, arguments: arguments);


            // Thay thế Timer bằng Task chạy ngầm
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        // Xử lý các sự kiện thẻ
                         ReadCard.getInstance().SuLySuKienThe();
                    }
                    catch (Exception ex)
                    {
                        // Xử lý lỗi nếu có
                        Console.WriteLine($"Error: {ex.Message}");
                    }

                    // Đợi 1 giây trước khi tiếp tục
                    await Task.Delay(2000);
                }
            });

    
        }
        private void Topic_EventBusData_Handler(object sender, object e, string rootingKey)
        {
            try
            {
                if (e == null) return;
                CardEventMQ cardEvent = e as CardEventMQ;
                if (cardEvent == null || (cardEvent?.CardNumber ?? 0) <= 0) return;

                ReadCard myoption = ReadCard.getInstance();
                if (myoption.DanhSachThe.Count(c => c.ReaderId == cardEvent.ReaderId && c.CardNumber == cardEvent.CardNumber) == 0)
                    myoption.DanhSachThe.Enqueue(cardEvent);

            }
            catch (Exception ex)
            {
                string message = ex.Message.ToString();
            }
        }
    }
}
