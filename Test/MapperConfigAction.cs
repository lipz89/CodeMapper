using CodeMapper.Metas;
using System;
using System.ComponentModel.Composition;

namespace Test
{
    [Export(typeof(IMapperConfigAction))]
    class MapperConfigAction : IMapperConfigAction
    {
        public void Config(IMapperConfig config)
        {
            config.SetNameMatching((x, y) =>
            {
                if(x.Equals(y, StringComparison.OrdinalIgnoreCase))
                    return true;
                if(x + "Model" == y)
                    return true;
                if(y + "Model" == x)
                    return true;
                if(x.Replace("Models", "s") == y)
                    return true;
                if(y.Replace("Models", "s") == x)
                    return true;

                return false;
            });
        }
    }
}
