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

using DynamicData;
using Splat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using works.ei8.Cortex.Diary.Domain.Model.Neurons;

namespace works.ei8.Cortex.Diary.Application.Neurons
{
    public class NeuronQueryService : INeuronQueryService
    {
        private INeuronGraphQueryClient neuronQueryClient;

        public NeuronQueryService(INeuronGraphQueryClient neuronQueryClient = null)
        {
            this.neuronQueryClient = neuronQueryClient ?? Locator.Current.GetService<INeuronGraphQueryClient>();
        }

        public async Task<IEnumerable<Neuron>> GetNeuronById(string avatarUrl, string id, Neuron central = null, RelativeType type = RelativeType.NotSet, CancellationToken token = default(CancellationToken))
        {
            var neurons = await this.neuronQueryClient.GetNeuronById(avatarUrl, id, central, type, token);
            neurons.ToList().ForEach(n => {
                n.Id = Guid.NewGuid().GetHashCode();
                n.CentralId = central != null ? central.Id : int.MinValue;
            });

            return neurons;
        }

        public async Task<IEnumerable<Neuron>> GetNeurons(string avatarUrl, Neuron central = null, RelativeType type = RelativeType.NotSet, NeuronQuery neuronQuery = null, int? limit = 1000, CancellationToken token = default(CancellationToken))
        {
            var relatives = await this.neuronQueryClient.GetNeurons(avatarUrl, central, type, neuronQuery, limit, token);
            relatives.ToList().ForEach(n => {
                n.Id = Guid.NewGuid().GetHashCode();
                n.CentralId = central != null ? central.Id : int.MinValue;
            });

            if (central == null)
                relatives = relatives.OrderBy(n => n.Tag);
            else
            {
                var posts = relatives.Where(n => n.Type == RelativeType.Postsynaptic);
                var pres = relatives.Where(n => n.Type == RelativeType.Presynaptic);
                posts = posts.ToList().OrderBy(n => n.Tag);
                pres = pres.ToList().OrderBy(n => n.Tag);
                relatives = posts.Concat(pres);
            }

            return relatives;
        }
    }
}
