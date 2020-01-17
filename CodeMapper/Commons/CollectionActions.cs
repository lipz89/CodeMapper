using System;
using System.Collections.Generic;

namespace CodeMapper.Commons
{
    /// <summary>
    /// 集合操作方法
    /// </summary>
    /// <typeparam name="TT"></typeparam>
    /// <typeparam name="TItem"></typeparam>
    public class CollectionActions<TT, TItem> where TT : IEnumerable<TItem>
    {
        public static Action<TT, TItem> AddItem { get; private set; }
        public static Action<TT> Clear { get; private set; }
        public static Func<List<TItem>, TT> New { get; private set; }

        static CollectionActions()
        {
            CollectionActions<IList<TItem>, TItem>.AddItem = (l, i) => l.Add(i);
            CollectionActions<IList<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<IList<TItem>, TItem>.New = l => l;

            CollectionActions<List<TItem>, TItem>.AddItem = (l, i) => l.Add(i);
            CollectionActions<List<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<List<TItem>, TItem>.New = l => l;

            CollectionActions<ICollection<TItem>, TItem>.AddItem = (l, i) => l.Add(i);
            CollectionActions<ICollection<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<ICollection<TItem>, TItem>.New = l => l;

            CollectionActions<HashSet<TItem>, TItem>.AddItem = (l, i) => l.Add(i);
            CollectionActions<HashSet<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<HashSet<TItem>, TItem>.New = l => new HashSet<TItem>(l);

            CollectionActions<ISet<TItem>, TItem>.AddItem = (l, i) => l.Add(i);
            CollectionActions<ISet<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<ISet<TItem>, TItem>.New = l => new HashSet<TItem>(l);

            CollectionActions<Queue<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<Queue<TItem>, TItem>.AddItem = (l, i) => l.Enqueue(i);
            CollectionActions<Queue<TItem>, TItem>.New = l => new Queue<TItem>(l);

            CollectionActions<Stack<TItem>, TItem>.AddItem = (l, i) => l.Push(i);
            CollectionActions<Stack<TItem>, TItem>.Clear = l => l.Clear();
            CollectionActions<Stack<TItem>, TItem>.New = l => new Stack<TItem>(l);
        }
    }
}
