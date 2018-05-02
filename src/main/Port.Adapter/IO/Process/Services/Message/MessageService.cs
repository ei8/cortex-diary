using System;
using System.Collections.Generic;
using System.Text;
using works.ei8.Cortex.Diary.Application.Dependency;
using works.ei8.Cortex.Diary.Application.Message;

namespace works.ei8.Cortex.Diary.Port.Adapter.IO.Process.Services.Message
{
    public class MessageService : IMessageService
    {
        private IMessageServiceImplementation messageServiceImplementation;

        public MessageService(IDependencyService dependencyService)
        {
            this.messageServiceImplementation = dependencyService.Get<IMessageServiceImplementation>();
        }

        public void LongAlert(string message)
        {
            this.messageServiceImplementation.LongAlert(message);
        }

        public void ShortAlert(string message)
        {
            this.messageServiceImplementation.ShortAlert(message);
        }
    }
}
