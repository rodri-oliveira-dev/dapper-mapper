using BenchmarkDotNet.Attributes;
using Dapper;
using DapperMapper.Repositories;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DapperMapper.Console
{
    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        int NumberOfItems = 100;
        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        [Benchmark]
        public string Dapper()
        {
            for (int i = 0; i < NumberOfItems; i++)
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("Campo1", "Campo1");
                parameters.Add("Campo2", "Campo2");
                parameters.Add("Campo3", "Campo3");
                parameters.Add("Campo4", "Campo4");
                parameters.Add("Campo5", "Campo5");
                parameters.Add("Campo6", "Campo6");
                parameters.Add("Campo7", "Campo7");
                parameters.Add("Campo8", "Campo8");
                parameters.Add("Campo9", "Campo9");
                parameters.Add("Campo10", "Campo10");

                var insertCommand = @"INSERT INTO dbo.TabelaTeste (Id, Campo1, Campo2, Campo3, Campo4, Campo5, Campo6, Campo7, Campo8, Campo9, Campo10)
                                    VALUES(@Id, @Campo1, @Campo2, @Campo3, @Campo4, @Campo5, @Campo6, @Campo7, @Campo8, @Campo9, @Campo10); ";

                using IDbConnection db = new SqlConnection(StringConnection);
                db.ExecuteScalar<int>(insertCommand, parameters, commandType: CommandType.Text);
            }
            return "Total Dapper";
        }
        [Benchmark]
        public string DapperMapper()
        {
            var repo = new DapperRepository<TabelaTeste>(StringConnection);

            for (int i = 0; i < NumberOfItems; i++)
            {
                repo.Insert(new TabelaTeste
                {
                    Campo1 = "Campo1",
                    Campo2 = "Campo2",
                    Campo3 = "Campo3",
                    Campo4 = "Campo4",
                    Campo5 = "Campo5",
                    Campo6 = "Campo6",
                    Campo7 = "Campo7",
                    Campo8 = "Campo8",
                    Campo9 = "Campo9",
                    Campo10 = "Campo10"
                });
            }
            return "Total DapperMapper";
        }
    }
}
