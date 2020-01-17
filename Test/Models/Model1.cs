using System.Collections.Generic;

namespace Test.Models
{
    public class Model1
    {
        public string Name { get; set; }
        public int ID { get; set; }

        public byte ByteInt { get; set; }
        public byte ByteString { get; set; }
        public M1 M1 { get; set; }
        public List<Item1> Items { get; set; }
        public List<byte> Datas { get; set; }
    }
    public class Model2
    {
        public string Name { get; set; }
        public int IDModel { get; set; }
        public int ByteInt { get; set; }
        public string ByteString { get; set; }
        public M1Model M1Model { get; set; }
        public ICollection<Item2> ItemModels { get; set; }
        public int[] DataModels { get; set; }
    }

    public class Item1
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Model1 Parent { get; set; }
    }

    public class Item2
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Model2 ParentModel { get; set; }
    }
    public class M1
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Model1 Parent { get; set; }
    }

    public class M1Model
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public Model2 ParentModel { get; set; }
    }
}
