# CodeMapper

- 支持递归引用的映射
- 支持部分集合的转换，包含：
```cs
T[]//数组，如果作为目标对象的属性，必须是可写属性
//以下集合，如果目标对象的属性不为null，直接覆盖元素，否则新建一个新的对象赋值到成员
IList<T>
List<T>
ICollection<T>
HashSet<T>
ISet<T>
Queue<T>
Stack<T>
```
- 配置映射规则
- 支持通过成员名称匹配规则映射
- 支持忽略成员
```cs
CodeMapper.Mapper.Config(config =>
{
    //默认true,如果设置为false，需要为类型指定转换规则
    config.BindWhenNeed = true;
    //成员名称匹配规则映射
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
});
```

- 支持通过自定义转换函数映射
```cs
CodeMapper.Mapper.BindCustom<Model1, Model2>(b => new Model2
{
    ID = b.ID,
    ByteInt = 1
}); 
```
- 支持通过表达式映射成员
```cs
CodeMapper.Mapper.Bind<Model1, Model2>(b =>
{
    //表达式映射
    b.Bind(x => x.Name, x => x.Name + "111");
    b.Bind(x => x.Name2, x => "111");
    //成员映射
    b.Bind(x => x.Name3, x => x.Name4);
    //忽略成员
    b.Ignore(x => x.ID);
});
```

```
类型转换时优先使用自定义转换方法
通过Bind方法指定的映射会覆盖全局忽略
通过Bind方法指定映射规则时未指定的目标成员，使用全局成员名称匹配规则
```
```
简单类型通过.Net默认的转换方法
集合对象通过集合操作方法转换
普通类对象的映射通过ExpressionTree构建转换方法
```


部分类和代码是参考的[TinyMapper](https://github.com/TinyMapper/TinyMapper "TinyMapper")
