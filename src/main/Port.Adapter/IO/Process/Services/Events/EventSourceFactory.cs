using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.EventSourcing.Client;
using works.ei8.EventSourcing.Client.Out;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Events
{
    public class EventSourceFactory : IEventSourceFactory
    {
        public EventSource CreateEventSource(string storeUrl)
        {
            var nc = new HttpNotificationClient(storeUrl);
            return new EventSource(storeUrl, null, null, null, nc);
        }
    }
}
