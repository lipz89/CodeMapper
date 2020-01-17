using CodeMapper.Metas;

namespace CodeMapper.Builders
{
    internal abstract class MapperBuilder
    {
        public MappingMemberBuilder MemberBuilder { get; }

        public MapperBuilder(IMapperConfig config)
        {
            MemberBuilder = new MappingMemberBuilder(config);
        }

        public void Build(TypePair typePair)
        {
            BuildCore(typePair, null);
        }

        public void Build(TypePair typePair, BindingConfig bindingConfig)
        {
            BuildCore(typePair, bindingConfig);
        }

        protected abstract void BuildCore(TypePair typePair, BindingConfig bindingConfig);

        public static MapperBuilder GetMapperBuilder(IMapperConfig config)
        {
            return new ExpressionBuilder(config);
        }
    }
}
