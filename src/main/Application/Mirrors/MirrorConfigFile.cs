using ei8.Cortex.Coding;
using System.Collections.Generic;

namespace ei8.Cortex.Diary.Application.Mirrors
{
    public class MirrorConfigFile
    {
        public MirrorConfigFile()
        {
        }

        public string Path { get; set; }

        public IEnumerable<MirrorConfig> Mirrors { get; set; }
    }
}
