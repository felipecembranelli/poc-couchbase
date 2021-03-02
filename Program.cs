using System;
using System.Threading.Tasks;
using Couchbase;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Collections.Generic;

namespace poc_couchbase
{
    class Program
    {
        static Random random = new Random();
        static List<string> list = new List<string>{ "Other Lage","British Ale","German Lager",
                                        "Belgian and French Ale", "German Ale", "North American Lager"};

        static async Task Main(string[] args)
        {
            
            for(int i=0; i < 10000; i++)
            {
                try
                {
                    await Run();
                }
                catch (System.Exception)
                {
                    //continue
                }
                
            }
           

        }

        static async Task Run() {
            
            int index = random.Next(list.Count);
            //Console.WriteLine(list[index]);

            await GetCouchbase(list[index]);

            //GetMongodb(list[index]);

        }
        static async Task GetCouchbase(string beerCategory)
        {
            var cluster = await Cluster.ConnectAsync("couchbase://localhost", "Administrator", "password");
            var bucket = await cluster.BucketAsync("beer-sample");
            var collection = bucket.DefaultCollection();

            // var upsertResult = await collection.UpsertAsync("my-document-key", new { Name = "Ted", Age = 31 });
            // var getResult = await collection.GetAsync("my-document-key");
            // Console.WriteLine(getResult.ContentAs<dynamic>());

            //var queryResult = await cluster.QueryAsync<dynamic>("select * from `beer-sample`", new Couchbase.Query.QueryOptions());

             var queryResult = await cluster.QueryAsync<dynamic>(
                "SELECT t.* FROM `beer-sample` t WHERE t.category=$1 limit 1",
                options => options.Parameter("beerCategory")
            );

            await foreach (var row in queryResult) {
                Console.WriteLine(row["name"]);
            }
        }

         static void GetMongodb(string beerCategory)
        {
            var dbClient = new MongoClient("mongodb://127.0.0.1:27017");

            IMongoDatabase db = dbClient.GetDatabase("local");
            var cars = db.GetCollection<BsonDocument>("beer-sample");

            var filter = Builders<BsonDocument>.Filter.Eq("category", beerCategory);

            var doc = cars.Find(filter).FirstOrDefault();

            //var count = cars.Find(new BsonDocument()).ToList();

            //Console.WriteLine(count.Count);

            Console.WriteLine(doc.ToString());
        }
    }
}
