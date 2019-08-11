/*
    This file is part of the d# project.
    Copyright (c) 2016-2018 ei8
    Authors: ei8
     This program is free software; you can redistribute it and/or modify
    it under the terms of the GNU Affero General Public License version 3
    as published by the Free Software Foundation with the addition of the
    following permission added to Section 15 as permitted in Section 7(a):
    FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
    EI8. EI8 DISCLAIMS THE WARRANTY OF NON INFRINGEMENT OF THIRD PARTY RIGHTS
     This program is distributed in the hope that it will be useful, but
    WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
    or FITNESS FOR A PARTICULAR PURPOSE.
    See the GNU Affero General Public License for more details.
    You should have received a copy of the GNU Affero General Public License
    along with this program; if not, see http://www.gnu.org/licenses or write to
    the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
    Boston, MA, 02110-1301 USA, or download the license from the following URL:
    https://github.com/ei8/cortex-diary/blob/master/LICENSE
     The interactive user interfaces in modified source and object code versions
    of this program must display Appropriate Legal Notices, as required under
    Section 5 of the GNU Affero General Public License.
     You can be released from the requirements of the license by purchasing
    a commercial license. Buying such a license is mandatory as soon as you
    develop commercial activities involving the d# software without
    disclosing the source code of your own applications.
     For more information, please contact ei8 at this address: 
     support@ei8.works
 */

using NLog;
using Polly;
using Splat;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Application.RequestProvider;
using works.ei8.Cortex.Diary.Application.Settings;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Neurons
{
    public class NeuronClient : INeuronClient
    {
        private readonly IRequestProvider requestProvider;
        private readonly ISettingsService settingsService;

        private static Policy exponentialRetryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryAsync(
                3,
                attempt => TimeSpan.FromMilliseconds(100 * Math.Pow(2, attempt)),
                (ex, _) => NeuronClient.logger.Error(ex, "Error occurred while communicating with Neurul Cortex. " + ex.InnerException?.Message)
            );

        private static string neuronsPathTemplate = "cortex/neurons/{0}";
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        public NeuronClient(IRequestProvider requestProvider = null, ISettingsService settingsService = null)
        {
            this.requestProvider = requestProvider ?? Locator.Current.GetService<IRequestProvider>();
            this.settingsService = settingsService ?? Locator.Current.GetService<ISettingsService>();
        }
        
        public async Task CreateNeuron(string avatarUrl, string id, string tag, string authorId, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.CreateNeuronInternal(avatarUrl, id, tag, authorId, token).ConfigureAwait(false));

        private async Task CreateNeuronInternal(string avatarUrl, string id, string tag, string authorId, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Tag = tag,
                AuthorId = authorId
            };

            await this.requestProvider.PutAsync(
               $"{avatarUrl}{string.Format(NeuronClient.neuronsPathTemplate, id)}",
               data,
               this.settingsService.AuthAccessToken
               );
        }

        public async Task ChangeNeuronTag(string avatarUrl, string id, string tag, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                    async () => await this.ChangeNeuronTagInternal(avatarUrl, id, tag, authorId, expectedVersion, token).ConfigureAwait(false));

        private async Task ChangeNeuronTagInternal(string avatarUrl, string id, string tag, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                Tag = tag,
                AuthorId = authorId
            };

            await this.requestProvider.PatchAsync(
               $"{avatarUrl}{string.Format(NeuronClient.neuronsPathTemplate, id)}",
               data,
               this.settingsService.AuthAccessToken,
               token,
               new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }

        public async Task DeactivateNeuron(string avatarUrl, string id, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken)) =>
            await NeuronClient.exponentialRetryPolicy.ExecuteAsync(
                async () => await this.DeactivateNeuronInternal(avatarUrl, id, authorId, expectedVersion, token));

        private async Task DeactivateNeuronInternal(string avatarUrl, string id, string authorId, int expectedVersion, CancellationToken token = default(CancellationToken))
        {
            var data = new
            {
                AuthorId = authorId
            };

            await this.requestProvider.DeleteAsync(
               $"{avatarUrl}{string.Format(NeuronClient.neuronsPathTemplate, id)}",
               data,
               this.settingsService.AuthAccessToken,
               token,
               new KeyValuePair<string, string>("ETag", expectedVersion.ToString())
               );
        }
    }
}
