namespace CodeMapper.Metas
{
    /// <summary>
    /// 发现多个匹配的映射源成员时的处理方式
    /// </summary>
    public enum MultiMatchHandle
    {
        /// <summary>
        /// 忽略该成员的映射
        /// </summary>
        Ignore,
        /// <summary>
        /// 映射第一个匹配的成员
        /// </summary>
        First,
        /// <summary>
        /// 报错
        /// </summary>
        Error,
    }
}
