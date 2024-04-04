using NHibernateProfilerLibrary.Messages;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NHibernateProfilerLibrary
{
    public static class NHibernateProfilerStateManager
    {
        private static ConcurrentQueue<PublishedEvent> _publishedEvents = new ConcurrentQueue<PublishedEvent>();
        private static HttpClient _httpClient;
        internal static bool IsEnabled = false;

        static NHibernateProfilerStateManager()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri("http://localhost:22897");

            var timer = new System.Timers.Timer(300);
            timer.Elapsed += EmptyEventQueueTimer_Elapsed;
            timer.Start();
        }

        public static void Add(PublishedEvent publishedEvent)
        {
            if (!IsEnabled)
            {
                return;
            }
            _publishedEvents.Enqueue(publishedEvent);
        }

        public static IList<PublishedEvent> EmptyQueue()
        {
            var publishedEvents = _publishedEvents.ToList();
            _publishedEvents.Clear();
            return publishedEvents;
        }

        private static void EmptyEventQueueTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
        {
            var publishedEvents = EmptyQueue();
            if (!publishedEvents.Any() || !IsEnabled)
            {
                return;
            }

            SendMessages(publishedEvents);
        }

        // Fire and forget
        private static async void SendMessages(IList<PublishedEvent> publishedEvents)
        {
            var json = JsonSerializer.Serialize(publishedEvents);
            try
            {
                _httpClient.PostAsync("", new StringContent(json));
            }
            catch (Exception e)
            {
                // Shouldn't crash main app
            }
        }
    }
}
