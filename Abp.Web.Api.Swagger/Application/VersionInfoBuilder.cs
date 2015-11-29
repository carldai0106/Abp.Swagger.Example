using System.Collections.Generic;
using System.Linq;
using Abp.Swagger;

namespace Abp.Application
{
    public class VersionInfoBuilder
    {
        private readonly Dictionary<string, InfoBuilder> _versionInfos;

        public VersionInfoBuilder()
        {
            _versionInfos = new Dictionary<string, InfoBuilder>();
        }

        public InfoBuilder Version(string version, string title)
        {
            var infoBuilder = new InfoBuilder(version, title);
            _versionInfos[version] = infoBuilder;
            return infoBuilder;
        }

        public IDictionary<string, Info> Build()
        {
            var dic = _versionInfos.ToDictionary(entry => entry.Key, entry => entry.Value.Build());
            if (!(dic.ContainsKey("V1") || dic.ContainsKey("v1")))
            {
                var infoBuilder = new InfoBuilder("V1", "Default");
                dic.Add("V1", infoBuilder.Build());
            }

            return dic;
        }
    }
}