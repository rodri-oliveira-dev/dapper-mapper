using BenchmarkDotNet.Attributes;
using Dapper;
using System;
using System.Data;
using System.Data.SqlClient;

namespace DapperMapper.Console
{
    [MemoryDiagnoser]
    public class MemoryBenchmarkerDemo
    {
        private readonly int NumberOfItems = 100;
        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        [Benchmark]
        public string Dapper()
        {
            for (int i = 0; i < NumberOfItems; i++)
            {
                var parameters = new DynamicParameters();

                parameters.Add("Id", Guid.NewGuid());
                parameters.Add("CodigoBarra", "987654321");
                parameters.Add("Descricao", "Teste 2");
                parameters.Add("Valor", 25.9);
                parameters.Add("Ativo", true);
                parameters.Add("DataCadastro", DateTime.Now);

                var insertCommand = @"INSERT INTO dbo.Produtos (Id, CodigoBarra, Descricao, Valor, Ativo, DataCadastro)
                                    VALUES(@Id, @CodigoBarra, @Descricao, @Valor, @Ativo, @DataCadastro); ";

                using IDbConnection db = new SqlConnection(StringConnection);
                db.ExecuteScalar<int>(insertCommand, parameters, commandType: CommandType.Text);
            }
            return "Total Dapper";
        }
        [Benchmark]
        public string DapperMapper()
        {
            var repo = new ProdutosRepository(StringConnection);

            for (int i = 0; i < NumberOfItems; i++)
            {
                repo.Insert(new Produtos
                {
                    Descricao = "Teste 1",
                    CodigoIdentificacao = "123456789",
                    Ativo = true,
                    Valor = 15.9
                });
            }
            return "Total DapperMapper";
        }
    }
}
