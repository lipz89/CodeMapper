using CodeMapper;
using CodeMapper.Commons;
using CodeMapper.Mappers;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using Test.Models;

namespace Test
{
    [Export(typeof(IClassMapper))]
    class Mapper_Model1_Model2 : ClassMapper
    {
        protected override object InnerMap(object source, object target)
        {
            return Map((Model1)source, (Model2)target);
        }

        private Model2 Map(Model1 source, Model2 target)
        {
            if(source == null)
                return null;
            if(target == null)
                target = new Model2();

            target.IDModel = source.ID;
            target.Name = source.Name;
            return target;
        }

        public override void MapObjects(object source, object target)
        {
            MapObjects((Model1)source, (Model2)target);
        }

        public void MapObjects(Model1 source, Model2 target)
        {
            //var items = Main.Map<Item1, Item2>(source.Items);
            //target.ItemModels = CollectionActions<ICollection<Item2>, Item2>.MoveCollection(items, target.ItemModels);
            var datas = Main.Map<byte, int>(source.Datas);
            target.DataModels = CollectionActions<ICollection<int>, int>.MoveCollection(datas, target.DataModels);
        }
    }
}
