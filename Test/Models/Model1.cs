using System.Collections.Generic;

namespace Test.Models
{
    public class MPModel
    {
        public string Name { get; set; } = "1111";
        public string Name2 { get; set; } = "1111";
        public string Name3 { get; set; } = "1111";
        public string Name4 { get; set; } = "1111";
        public string Name5 { get; set; } = "1111";
        public string Name6 { get; set; } = "1111";
        public string Name7 { get; set; } = "1111";
        public string Name8 { get; set; } = "1111";
        public string Name9 { get; set; } = "1111";
        public decimal? Dec { get; set; } = 10;
        public decimal? Dec2 { get; set; } = 10;
        public decimal? Dec3 { get; set; } = 10;
        public decimal? Dec4{ get; set; } = 10;
        public decimal? Dec5 { get; set; } = 10;
        public decimal? Dec6 { get; set; } = 10;
        public decimal? Dec7 { get; set; } = 10;
        public decimal? Dec8 { get; set; } = 10;
        public decimal? Dec9 { get; set; } = 10;
        public decimal? Int { get; set; } = 10;
        public decimal? Int2 { get; set; } = 10;
        public decimal? Int3 { get; set; } = 10;
        public decimal? Int4 { get; set; } = 10;
        public decimal? Int5 { get; set; } = 10;
        public decimal? Int6 { get; set; } = 10;
        public decimal? Int7 { get; set; } = 10;
        public decimal? Int8 { get; set; } = 10;
        public decimal? Int9 { get; set; } = 10;
    }
    public class MPModel2
    {
        public string Name { get; set; } = "1111";
        public string Name2 { get; set; } = "1111";
        public string Name3 { get; set; } = "1111";
        public string Name4 { get; set; } = "1111";
        public string Name5 { get; set; } = "1111";
        public string Name6 { get; set; } = "1111";
        public string Name7 { get; set; } = "1111";
        public string Name8 { get; set; } = "1111";
        public string Name9 { get; set; } = "1111";
        public decimal? Dec { get; set; } = 10;
        public decimal? Dec2 { get; set; } = 10;
        public decimal? Dec3 { get; set; } = 10;
        public decimal? Dec4 { get; set; } = 10;
        public decimal? Dec5 { get; set; } = 10;
        public decimal? Dec6 { get; set; } = 10;
        public decimal? Dec7 { get; set; } = 10;
        public decimal? Dec8 { get; set; } = 10;
        public decimal? Dec9 { get; set; } = 10;
        public decimal? Int { get; set; } = 10;
        public decimal? Int2 { get; set; } = 10;
        public decimal? Int3 { get; set; } = 10;
        public decimal? Int4 { get; set; } = 10;
        public decimal? Int5 { get; set; } = 10;
        public decimal? Int6 { get; set; } = 10;
        public decimal? Int7 { get; set; } = 10;
        public decimal? Int8 { get; set; } = 10;
        public decimal? Int9 { get; set; } = 10;
    }

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
