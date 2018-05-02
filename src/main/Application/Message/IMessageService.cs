using System;
using System.Collections.Generic;
using System.Text;

namespace works.ei8.Cortex.Diary.Application.Message
{
    public interface IMessageService
    {
        void LongAlert(string message);
        void ShortAlert(string message);
    }
}
