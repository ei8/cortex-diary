using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    internal class Helper
    {
        internal static async Task SetStatusOnComplete(Func<Task> value, string completionMessage, IStatusService statusService)
        {
            try
            {
                await value();
                statusService.Message = completionMessage;
            }
            catch (Exception ex)
            {
                statusService.Message = ex.Message;
            }
        }
    }
}
