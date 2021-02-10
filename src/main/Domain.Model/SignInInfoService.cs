using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Domain.Model
{
    public class SignInInfoService : ISignInInfoService
    {
        private List<SignInInfo> signIns;

        public SignInInfoService()
        {
            this.signIns = new List<SignInInfo>();
        }

        public IEnumerable<SignInInfo> SignIns => this.signIns.ToArray();

        public void Add(SignInInfo value)
        {
            this.signIns.Add(value);
        }

        public void Remove(SignInInfo value)
        {
            this.signIns.Remove(value);
        }
    }
}
