
using Microsoft.Extensions.DependencyInjection;
using MudBlazor;
using Newtonsoft.Json.Linq;
using QualityCertificate.Datas.Entity;
using System.Collections.Concurrent;
using System.ComponentModel.DataAnnotations;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace QualityCertificate.Datas.Models
{
    public class ReadCard
    {
        private static ReadCard? instance;

        public IServiceProvider ServiceProvider;
        public ReadCard()
        {

        }

        public static ReadCard getInstance()
        {
            if (instance == null)
            {
                instance = new ReadCard();
            }
            return instance;
        }

        /// <summary>
        /// Ds skien thẻ nhận được
        /// </summary>
        public ConcurrentQueue<CardEventMQ> DanhSachThe = new();

        public int Location34;
        public int LocationP3;

        public long Card34;
        public long CardP3;
        public async void SuLySuKienThe()
        {
            CardEventMQ? cardEvent = new();
            try
            {
                using var scope = ServiceProvider?.CreateScope();
                var wSMDbContext = scope!.ServiceProvider.GetService<WSMDbContext>();

                //Lấy sk thẻ trong queue xử lý
                while (DanhSachThe.TryDequeue(out cardEvent))
                {
                    if (cardEvent.LocationId == Location34 || cardEvent.LocationId == LocationP3)
                    {
                        if (cardEvent == null || (cardEvent?.CardNumber ?? 0) <= 0) return;

                        if (!string.IsNullOrWhiteSpace(cardEvent?.CardNumber.ToString()))
                        {
                            if (cardEvent.LocationId == Location34)
                            {
                                Card34 = cardEvent.CardNumber;
                            }

                            if (cardEvent.LocationId == LocationP3)
                            {
                                CardP3 = cardEvent.CardNumber;
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error processing card event: {ex.Message} - {ex.StackTrace}");
            }

        }
    }
}
