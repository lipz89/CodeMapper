using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using CodeMapper.Commons;
using CodeMapper.Metas;

namespace CodeMapper.Builders
{
    internal class ExpressionBuilder : MapperBuilder
    {
        private static readonly Func<object, object, object> defaultMapper = (x, y) => y;

        private static readonly MethodInfo MapCoreMethod = typeof(MapperUtil).GetMethod("MapCore", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo MapCoresMethod = typeof(MapperUtil).GetMethod("MapCores", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo MoveCollectionMethod = typeof(MapperUtil).GetMethod("MoveCollection", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo GetExpressionResultMethod = typeof(MapperUtil).GetMethod("GetExpressionResult", BindingFlags.NonPublic | BindingFlags.Static);

        public ExpressionBuilder(IMapperConfig config) : base(config)
        {
        }

        protected override void BuildCore(TypePair typePair, BindingConfig bindingConfig)
        {
            var members = MemberBuilder.Build(typePair).Where(x => !x.Ignored).ToList();
            var equalMembers = new List<MappingMember>();
            var mappingMembers = new List<MappingMember>();
            var refMembers = new List<MappingMember>();
            var collectionMembers = new List<MappingMember>();
            var expressionMembers = new List<MappingMember>();

            foreach(var item in members)
            {
                if(item.IsExpressionMapping)
                {
                    expressionMembers.Add(item);
                }
                else
                {
                    var memberTypePair = TypePair.Create(item.Source.GetMemberType(), item.Target.GetMemberType());
                    if(memberTypePair.IsEqualTypes)
                    {
                        equalMembers.Add(item);
                    }
                    else if(memberTypePair.IsBaseTypes)
                    {
                        mappingMembers.Add(item);
                    }
                    else if(memberTypePair.IsEnumerableTypes)
                    {
                        collectionMembers.Add(item);
                    }
                    else
                    {
                        refMembers.Add(item);
                    }
                }
            }

            var func = CreateMapper(typePair, equalMembers, mappingMembers);
            Cache<TypePair, Func<object, object, object>>.Add(typePair, func);
            var action = CreateMapperRef(typePair, refMembers, collectionMembers, expressionMembers);
            if(action != null)
            {
                Cache<TypePair, Action<object, object>>.Add(typePair, action);
            }
        }

        private Func<object, object, object> CreateMapper(TypePair typePair, List<MappingMember> equals, List<MappingMember> mappers)
        {
            if(equals.Count + mappers.Count == 0)
            {
                return defaultMapper;
            }
            return CreateMapperExpression(typePair, equals, mappers);
        }
        private Func<object, object, object> CreateMapperExpression(TypePair typePair, List<MappingMember> equals, List<MappingMember> mappers)
        {
            var typeObject = typeof(object);
            var labelTarget = Expression.Label(typeObject, "result");
            var pSource = Expression.Parameter(typeObject, "source");
            var pTarget = Expression.Parameter(typeObject, "target");
            var vResult = Expression.Variable(typeObject, "r");
            var vSource = Expression.Variable(typePair.Source, "s");
            var vTarget = Expression.Variable(typePair.Target, "t");
            var nullValue = Expression.Constant(null);
            var targeEqualNull = Expression.Equal(pTarget, nullValue);
            var newValue = Expression.New(typePair.Target);
            var convertPar = Expression.Convert(pTarget, typePair.Target);
            var setTargetNewValue = Expression.Assign(vTarget, newValue);
            var setTargetConvertPar = Expression.Assign(vTarget, convertPar);
            var setTarget = Expression.IfThenElse(targeEqualNull, setTargetNewValue, setTargetConvertPar);
            var convertSource = Expression.Convert(pSource, typePair.Source);
            var setSource = Expression.Assign(vSource, convertSource);
            var setTargetToResult = Expression.Assign(vResult, vTarget);
            var listExps = new List<Expression>();
            listExps.Add(setSource);
            listExps.Add(setTarget);
            if(equals.Any())
            {
                listExps.AddRange(SetEquals(vSource, vTarget, equals));
            }
            if(mappers.Any())
            {
                listExps.AddRange(SetMappers(vSource, vTarget, mappers));
            }
            listExps.Add(setTargetToResult);

            var list = Expression.Block(listExps);

            var sourcEqualNull = Expression.Equal(pSource, nullValue);
            var setResultNull = Expression.Assign(vResult, nullValue);
            var checkSourceNull = Expression.IfThenElse(sourcEqualNull, setResultNull, list);

            var returnResult = Expression.Return(labelTarget, vResult, typeObject);

            var vars = new List<ParameterExpression> { vResult, vSource, vTarget };

            var label = Expression.Label(labelTarget, vResult);

            var body = Expression.Block(typeObject, vars, checkSourceNull, returnResult, label);

            Expression<Func<object, object, object>> lambda = Expression.Lambda<Func<object, object, object>>(body, pSource, pTarget);
            return lambda.Compile();
        }

        private Action<object, object> CreateMapperRef(TypePair typePair, List<MappingMember> refs, List<MappingMember> colls, List<MappingMember> exps)
        {
            if(refs.Count + colls.Count == 0)
            {
                return null;
            }
            return CreateMapperRefExpression(typePair, refs, colls, exps);
        }
        private Action<object, object> CreateMapperRefExpression(TypePair typePair, List<MappingMember> refs, List<MappingMember> colls, List<MappingMember> exps)
        {
            var typeObject = typeof(object);
            var pSource = Expression.Parameter(typeObject, "source");
            var pTarget = Expression.Parameter(typeObject, "target");
            var vSource = Expression.Variable(typePair.Source, "s");
            var vTarget = Expression.Variable(typePair.Target, "t");
            var nullValue = Expression.Constant(null);
            var convertPar = Expression.Convert(pTarget, typePair.Target);
            var setTarget = Expression.Assign(vTarget, convertPar);
            var convertSource = Expression.Convert(pSource, typePair.Source);
            var setSource = Expression.Assign(vSource, convertSource);
            var listExps = new List<Expression>();
            listExps.Add(setSource);
            listExps.Add(setTarget);
            if(refs.Any())
            {
                listExps.AddRange(SetMappers(vSource, vTarget, refs));
            }
            if(colls.Any())
            {
                listExps.AddRange(SetCollections(vSource, vTarget, colls));
            }
            if(exps.Any())
            {
                listExps.AddRange(SetExpressions(vSource, vTarget, exps));
            }

            var list = Expression.Block(listExps);

            var sourceNotNull = Expression.NotEqual(pSource, nullValue);
            var targetNotNull = Expression.NotEqual(pTarget, nullValue);
            var allNotNull = Expression.AndAlso(sourceNotNull, targetNotNull);
            var checkSourceNull = Expression.IfThen(allNotNull, list);

            var vars = new List<ParameterExpression> { vSource, vTarget };

            var body = Expression.Block(vars, checkSourceNull);

            Expression<Action<object, object>> lambda = Expression.Lambda<Action<object, object>>(body, pSource, pTarget);
            return lambda.Compile();
        }

        private List<Expression> SetEquals(Expression source, Expression target, List<MappingMember> equals)
        {
            var list = new List<Expression>();
            foreach(var item in equals)
            {
                var sMember = Expression.MakeMemberAccess(source, item.Source);
                var tMember = Expression.MakeMemberAccess(target, item.Target);
                list.Add(Expression.Assign(tMember, sMember));
            }
            return list;
        }
        private List<Expression> SetMappers(Expression source, Expression target, List<MappingMember> mappers)
        {
            var list = new List<Expression>();
            foreach(var item in mappers)
            {
                var sMember = Expression.MakeMemberAccess(source, item.Source);
                var tMember = Expression.MakeMemberAccess(target, item.Target);
                var method = MapCoreMethod.MakeGenericMethod(item.Source.GetMemberType(), item.Target.GetMemberType());
                var convert = Expression.Call(method, sMember, tMember);
                if(item.Target.IsWritable())
                {
                    list.Add(Expression.Assign(tMember, convert));
                }
                else
                {
                    list.Add(convert);
                }
            }
            return list;
        }

        private List<Expression> SetCollections(Expression source, Expression target, List<MappingMember> mappers)
        {
            var list = new List<Expression>();
            foreach(var item in mappers)
            {
                var sourceType = item.Source.GetMemberType();
                var targetType = item.Target.GetMemberType();
                var sourceItemType = sourceType.GetCollectionItemType();
                var targetItemType = targetType.GetCollectionItemType();
                var sMember = Expression.MakeMemberAccess(source, item.Source);
                var tMember = Expression.MakeMemberAccess(target, item.Target);
                Expression expList = sMember;
                if(sourceItemType != targetItemType)
                {
                    var method = MapCoresMethod.MakeGenericMethod(sourceItemType, targetItemType);
                    expList = Expression.Call(method, sMember);
                }

                var moveMethod = MoveCollectionMethod.MakeGenericMethod(targetType, targetItemType);
                var result = Expression.Call(moveMethod, expList, tMember);
                if(item.Target.IsWritable())
                {
                    list.Add(Expression.Assign(tMember, result));
                }
                else if(!targetType.IsArray)
                {
                    list.Add(result);
                }
            }
            return list;
        }

        private List<Expression> SetExpressions(Expression source, Expression target, List<MappingMember> equals)
        {
            var list = new List<Expression>();
            foreach(var item in equals)
            {
                var tMember = Expression.MakeMemberAccess(target, item.Target);
                var targetMemberName = item.Target.Name;
                var expName = Expression.Constant(targetMemberName, typeof(string));
                var method = GetExpressionResultMethod.MakeGenericMethod(source.Type, target.Type, tMember.Type);
                var result = Expression.Call(method, expName, source);
                if(item.Target.IsWritable())
                {
                    list.Add(Expression.Assign(tMember, result));
                }
            }
            return list;
        }
    }
}
