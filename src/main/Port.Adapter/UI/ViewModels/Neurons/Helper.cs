using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace works.ei8.Cortex.Diary.Port.Adapter.UI.ViewModels.Neurons
{
    internal class Helper
    {
        internal static async Task SetStatusOnComplete(Func<Task<bool>> value, string completionMessage, IStatusService statusService, string cancellationMessage = null)
        {
            try
            {
                if (await value())
                    statusService.Message = completionMessage;
                else
                    statusService.Message = cancellationMessage;
            }
            catch (Exception ex)
            {
                statusService.Message = ex.Message;
            }
        }
    }
}
