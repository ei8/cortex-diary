using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Domain.Model
{
    public interface ISignInInfoService
    {
        void Add(SignInInfo value);

        void Remove(SignInInfo value);

        IEnumerable<SignInInfo> SignIns 
        { 
            get; 
        }
    }
}
