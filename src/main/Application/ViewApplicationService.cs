using ei8.Cortex.Diary.Domain.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Application
{
    public class ViewApplicationService : IViewApplicationService
    {
        private readonly IViewRepository viewRepository;
        public ViewApplicationService(IViewRepository viewRepository)
        {
            this.viewRepository = viewRepository;
        }

        public async Task<IEnumerable<View>> GetAll(CancellationToken token = default)
        {
            await this.viewRepository.Initialize();
            return await this.viewRepository.GetAll();
        }
    }
}