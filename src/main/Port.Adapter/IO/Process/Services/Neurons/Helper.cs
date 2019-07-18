using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons
{
    public class Helper
    {
        internal static void AppendAuthorId(string authorId, StringBuilder sb)
        {
            Helper.AppendCommaIfPrecedingExists(sb);
            sb.Append($"\"AuthorId\": \"{authorId}\"");
        }

        internal static void AppendCommaIfPrecedingExists(StringBuilder sb)
        {
            if (sb.ToString().EndsWith("\""))
                sb.Append(", ");
        }

        internal static async Task SendRequest(string method, string avatarUrl, string requestUri, StringBuilder sb, CancellationToken token = default(CancellationToken), params KeyValuePair<string, string>[] headers)
        {
            var httpClient = new HttpClient()
            {
                BaseAddress = new Uri(avatarUrl)
            };

            HttpRequestMessage msg = new HttpRequestMessage
            {
                Method = new HttpMethod(method),
                RequestUri = new Uri(requestUri, UriKind.Relative)
            };
            headers.ToList().ForEach(h => msg.Headers.Add(h.Key, h.Value));            
            msg.Content = new StringContent(sb.ToString(), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.SendAsync(msg, token);
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException(await response.Content.ReadAsStringAsync());
        }
    }
}
