using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace ei8.Cortex.Diary.Domain.Model
{
    public class View
    {
        [PrimaryKey]
        public string Url { get; set; }

        public string ParentUrl { get; set; }

        public string Name { get; set; }

        public bool IsDefault { get; set; }

        public int Sequence { get; set; }

        public string Icon { get; set; }
    }
}
