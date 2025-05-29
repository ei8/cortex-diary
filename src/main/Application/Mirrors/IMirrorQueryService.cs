using System.Collections.Generic;
using System.Threading;

namespace ei8.Cortex.Diary.Application.Mirrors
{
    public interface IMirrorQueryService
    {
        IEnumerable<MirrorConfigFile> GetAll(CancellationToken token = default);
    }
}
