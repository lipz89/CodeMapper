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
            CodeMapper.Mapper.Config(config =>
            {
                config.AutoMapReferenceProperty = false;
                config.BindWhenNeed = true;
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

                //config.SetObject2String(x => JsonConvert.SerializeObject(x, serializerSettings));

                //config.SetLogger(Console.WriteLine);
            });

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

            CodeMapper.Mapper.BindCustom<Model1, Model2>(b => new Model2
            {
                IDModel = b.ID,
                ByteInt = 1
            });

            CodeMapper.Mapper.Bind<Model1, Model2>(b =>
            {
                b.Bind(x => x.Name, x => x.Name + "111");
                b.Bind(x => x.Name, x => "111");
                b.Bind(x => x.Name, x => x.Name);
                b.Ignore(x => x.IDModel);
            });

            var model2 = CodeMapper.Mapper.Map<Model1, Model2>(model);
            var model1 = CodeMapper.Mapper.Map<Model2, Model1>(model2);

            var list1 = new List<Model1>();
            var j = JsonConvert.SerializeObject(model,new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            for(int i = 0; i < 10000; i++)
            {
                var m = JsonConvert.DeserializeObject<Model1>(j);
                list1.Add(m);
                //Console.WriteLine(ObjectMapper.GetKey(m));
            }
            for(int i = 0; i < 10; i++)
            {
                var st = Stopwatch.StartNew();
                var list2 = CodeMapper.Mapper.Map<Model1, Model2>(list1);
                var list3 = CodeMapper.Mapper.Map<Model2, Model1>(list2);
                st.Stop();
                Console.WriteLine("codemapper映射" + st.ElapsedMilliseconds);
                //break;
            }

            //var listss = new List<MPModel>();
            //for(int i = 0; i < 1000; i++)
            //{
            //    listss.Add(new MPModel());
            //}

            //var st = Stopwatch.StartNew();

            //var listt2 = CodeMapper.Mapper.Map<MPModel, MPModel2>(listss);

            //st.Stop();

            //var st2 = Stopwatch.StartNew();

            //var json = JsonConvert.SerializeObject(listss);
            //var listt3 = JsonConvert.DeserializeObject<List<MPModel2>>(json);

            //st2.Stop();

            //Console.WriteLine("codemapper映射" + st.ElapsedMilliseconds);
            //Console.WriteLine("json序列化映射" + st2.ElapsedMilliseconds);

            Console.WriteLine("all test end");
            Console.Read();
        }
    }
}
