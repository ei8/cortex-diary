using ei8.Cortex.Diary.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application
{
    public interface IViewApplicationService
    {
        Task<IEnumerable<View>> GetAll(CancellationToken token = default);
    }
}
