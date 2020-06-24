using CodeMapper.Mappers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Test.Models;

namespace Test
{
    class Program
    {
        private static JsonSerializerSettings serializerSettings = new JsonSerializerSettings
        {
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            DateFormatString = "yyyy-MM-dd HH:mm:ss",
            Formatting = Formatting.Indented
        };
        private static void Log(object obj, string message)
        {
            var str = JsonConvert.SerializeObject(obj, serializerSettings);
            Console.WriteLine($"{message}:\r\n{str}");
        }
        static void Main(string[] args)
        {
            //TestConfig();

            //TestLoop();
            //TestIgnore();
            //TestDepth1();
            TestDepth10();

            Console.WriteLine("all test end");
            Console.Read();
        }

        static Model1 GetTestModel()
        {
            var model = new Model1
            {
                ID = 1,
                ByteInt = 2,
                ByteString = 3,
                Name = "test",
                M1 = new M1
                {
                    ID = 12,
                    Name = "innermodel"
                },
                Items = new List<Item1>
                {
                    new Item1
                    {
                        ID=2,
                        Name="testitem"
                    },
                    new Item1
                    {
                        ID=3,
                        Name="testitem2"
                    },
                },
                Datas = new List<byte> { 1, 2, 3, 4 }
            };

            model.M1.Parent = model;
            model.Items.ForEach(x => x.Parent = model);
            return model;
        }


        public static void TestConfig()
        {
            CodeMapper.Mapper.Config(config =>
            {
                config.ReferencePropertyHandle = CodeMapper.Metas.ReferencePropertyHandle.Depth;
                config.MaxDepth = 10;
                config.BindWhenNeed = true;
                config.SetNameMatching(NameMatching);

                config.SetObject2String(x => JsonConvert.SerializeObject(x, serializerSettings));

                config.SetLogger(Console.WriteLine);
            });
            //CodeMapper.Mapper.BindCustom<Model1, Model2>(b => new Model2
            //{
            //    IDModel = b.ID,
            //    ByteInt = 1
            //});

            //CodeMapper.Mapper.Bind<Model1, Model2>(b =>
            //{
            //    b.Bind(x => x.Name, x => x.Name + "111");
            //    b.Bind(x => x.Name, x => "111");
            //    b.Bind(x => x.Name, x => x.Name);
            //    b.Ignore(x => x.IDModel);
            //});
            var model = GetTestModel();
            var model2 = CodeMapper.Mapper.Map<Model1, Model2>(model);
            var model1 = CodeMapper.Mapper.Map<Model2, Model1>(model2);
        }

        static bool NameMatching(string x, string y)
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
        }

        static void TestIgnore()
        {
            Console.WriteLine("测试映射效率：ReferencePropertyHandle=Ignore");
            CodeMapper.Mapper.Config(config =>
            {
                config.ReferencePropertyHandle = CodeMapper.Metas.ReferencePropertyHandle.Ignore;
                config.BindWhenNeed = true;
                config.SetNameMatching(NameMatching);
            });
            LoopTest(10000, 20);
        }

        static void TestLoop()
        {
            Console.WriteLine("测试映射效率：ReferencePropertyHandle=Loop");
            CodeMapper.Mapper.Config(config =>
            {
                config.ReferencePropertyHandle = CodeMapper.Metas.ReferencePropertyHandle.Loop;
                config.BindWhenNeed = true;
                config.SetNameMatching(NameMatching);
            });
            LoopTest(10000, 20);
        }

        static void TestDepth10()
        {
            Console.WriteLine("测试映射效率：ReferencePropertyHandle=Depth:10");
            CodeMapper.Mapper.Config(config =>
            {
                config.ReferencePropertyHandle = CodeMapper.Metas.ReferencePropertyHandle.Depth;
                config.MaxDepth = 10;
                config.BindWhenNeed = true;
                config.SetNameMatching(NameMatching);
            });
            LoopTest(10000, 20);
        }

        static void TestDepth1()
        {
            Console.WriteLine("测试映射效率：ReferencePropertyHandle=Depth:1");
            CodeMapper.Mapper.Config(config =>
            {
                config.ReferencePropertyHandle = CodeMapper.Metas.ReferencePropertyHandle.Depth;
                config.MaxDepth = 1;
                config.BindWhenNeed = true;
                config.SetNameMatching(NameMatching);
            });
            LoopTest(10000, 20);
        }

        static void LoopTest(int modelCount, int count)
        {
            Console.WriteLine($"模型数 {modelCount}，循环数{count}");
            var model = GetTestModel();
            var list1 = new List<Model1>();
            var j = JsonConvert.SerializeObject(model, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            for(int i = 0; i < modelCount; i++)
            {
                var m = JsonConvert.DeserializeObject<Model1>(j);
                list1.Add(m);
            }
            for(int i = 0; i < count; i++)
            {
                var st = Stopwatch.StartNew();
                var list2 = CodeMapper.Mapper.Map<Model1, Model2>(list1);
                var list3 = CodeMapper.Mapper.Map<Model2, Model1>(list2);
                st.Stop();
                Console.WriteLine("耗时" + st.ElapsedMilliseconds);
            }
        }
    }
}
