using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Abp.NSwagExtended
{
    public static class JTokenExtension
    {
        public static JToken RemoveFields(this JToken token, params string[] fields)
        {
            var container = token as JContainer;
            if (container == null) return token;

            var removeList = new List<JToken>();
            foreach (var el in container.Children())
            {
                var p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                el.RemoveFields(fields);
            }

            foreach (var el in removeList)
            {
                el.Remove();
            }

            return token;
        }
    }
}
