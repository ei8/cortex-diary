using Newtonsoft.Json;
using org.neurul.Common;
using org.neurul.Common.Constants;
using org.neurul.Common.Domain.Model;
using org.neurul.Common.Events;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Notifications;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Notifications
{
    public class NotificationClient : INotificationClient
    {
        private static string getEventsPathTemplate = "{0}cortex/notifications/{1}";
        private static readonly Dictionary<string, HttpClient> clients = new Dictionary<string, HttpClient>();

        private static HttpClient GetCreateClient(string url)
        {
            Uri uri = null;
            AssertionConcern.AssertArgumentValid<string>(u => Uri.TryCreate(url, UriKind.RelativeOrAbsolute, out uri), url, "Specified URL must be valid", nameof(url));
            var baseUrl = uri.GetLeftPart(UriPartial.Authority);
            if (!NotificationClient.clients.ContainsKey(baseUrl))
                NotificationClient.clients.Add(baseUrl, new HttpClient() { BaseAddress = new Uri(baseUrl) });
            return NotificationClient.clients[baseUrl];
        }

        public async Task<NotificationLog> GetNotificationLog(string avatarUrl, string notificationLogId, CancellationToken token = default(CancellationToken))
        {
            var response = await NotificationClient.GetCreateClient(avatarUrl).GetAsync(
                string.Format(NotificationClient.getEventsPathTemplate, avatarUrl, notificationLogId)
                ).ConfigureAwait(false);

            response.EnsureSuccessStatusCode();
            var eventInfoItems = JsonConvert.DeserializeObject<Notification[]>(
                await response.Content.ReadAsStringAsync().ConfigureAwait(false)
                );
            var linkHeader = response.Headers.GetValues(Response.Header.Link.Key).First();

            AssertionConcern.AssertStateTrue(linkHeader != null, "'Link' header is missing in server response.");

            NotificationLogId.TryParse(
                NotificationClient.GetLogId(linkHeader, Response.Header.Link.Relation.Self),
                out NotificationLogId selfLogId
                );
            NotificationLogId.TryParse(
                NotificationClient.GetLogId(linkHeader, Response.Header.Link.Relation.First),
                out NotificationLogId firstLogId
                );
            NotificationLogId.TryParse(
                NotificationClient.GetLogId(linkHeader, Response.Header.Link.Relation.Next),
                out NotificationLogId nextLogId
                );
            NotificationLogId.TryParse(
                NotificationClient.GetLogId(linkHeader, Response.Header.Link.Relation.Previous),
                out NotificationLogId previousLogId
                );
            return new NotificationLog(
                selfLogId,
                firstLogId,
                nextLogId,
                previousLogId,
                eventInfoItems,
                nextLogId != null
                );
        }


        private static string GetLogId(string linkHeader, Response.Header.Link.Relation relation)
        {
            string result = string.Empty;
            if (ResponseHelper.Header.Link.TryGet(linkHeader, relation, out string link))
            {
                link = link.TrimEnd('/');
                result = link.Substring(link.LastIndexOf('/') + 1);
            }
            return result;
        }
    }
}
