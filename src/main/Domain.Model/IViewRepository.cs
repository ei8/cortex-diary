using System;
using System.Collections.Generic;
using System.Text;          
using System.Threading.Tasks;

namespace ei8.Cortex.Diary.Domain.Model
{
    public interface IViewRepository
    {
        Task<IEnumerable<View>> GetAll();

        Task Initialize();
    }
}
