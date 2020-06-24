namespace CodeMapper.Metas
{
    /// <summary>
    /// 引用类型属性的处理方式
    /// </summary>
    public enum ReferencePropertyHandle
    {
        /// <summary> 忽略 </summary>
        Ignore,
        /// <summary> 映射到指定深度 </summary>
        Depth,
        /// <summary>  循环映射(通过字典) </summary>
        Loop
    }
}
