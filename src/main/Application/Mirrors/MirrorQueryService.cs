using ei8.Cortex.Diary.Application.Settings;
using neurUL.Common.Domain.Model;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;

namespace ei8.Cortex.Diary.Application.Mirrors
{
    public class MirrorQueryService : IMirrorQueryService
    {
        private readonly ISettingsService settingsService;

        public MirrorQueryService(ISettingsService settingsService)
        {
            AssertionConcern.AssertArgumentNotNull(settingsService, nameof(settingsService));

            this.settingsService = settingsService;
        }
        public IEnumerable<MirrorConfigFile> GetAll(CancellationToken token = default)
        {
            var result = new List<MirrorConfigFile>();

            foreach (var file in this.settingsService.MirrorConfigFiles)
            {
                var configFile = JsonSerializer.Deserialize<MirrorConfigFile>(File.ReadAllText(file));
                configFile.Path = file;
                result.Add(configFile);
            }

            return result;
        }
    }
}
