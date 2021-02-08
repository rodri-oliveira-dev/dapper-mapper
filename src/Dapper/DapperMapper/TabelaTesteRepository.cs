using Dapper;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperMapper.Console
{
    public class TabelaTesteRepository
    {
        private readonly string _scriptSelect = @"SELECT [Id] ,[Campo1],[Campo2],[Campo3],[Campo4],[Campo5],[Campo6],[Campo7] ,[Campo8],[Campo9],[Campo10]
            FROM [dbo].[TabelaTeste];";

        private readonly string _scriptInsert = @"INSERT INTO dbo.TabelaTeste (Id, Campo1, Campo2, Campo3, Campo4, Campo5, Campo6, Campo7, Campo8, Campo9, Campo10)
            VALUES (@Id, @Campo1, @Campo2, @Campo3, @Campo4, @Campo5, @Campo6, @Campo7, @Campo8, @Campo9, @Campo10);";

        private readonly string _scriptUpdate = @"UPDATE [dbo].[TabelaTeste]
           SET [Campo1] = @Campo1
              ,[Campo2] = @Campo2
              ,[Campo3] = @Campo3
              ,[Campo4] = @Campo4
              ,[Campo5] = @Campo5
              ,[Campo6] = @Campo6
              ,[Campo7] = @Campo7
              ,[Campo8] = @Campo8
              ,[Campo9] = @Campo9
              ,[Campo10] = @Campo10
            WHERE[Id] = @Id;";

        private readonly string _scriptDelete = "DELETE FROM [dbo].[TabelaTeste] WHERE [Id] = @Id;";

        private readonly string _connectionString;

        public TabelaTesteRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public List<TabelaTeste> GetAll()
        {
            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<TabelaTeste>(_scriptSelect, null, commandType: CommandType.Text).ToList();
        }

        public void Insert(TabelaTeste entity)
        {
            var parameters = new DynamicParameters();

            parameters.Add("Id", entity.Id);
            parameters.Add("Campo1", entity.Campo1);
            parameters.Add("Campo2", entity.Campo2);
            parameters.Add("Campo3", entity.Campo3);
            parameters.Add("Campo4", entity.Campo4);
            parameters.Add("Campo5", entity.Campo5);
            parameters.Add("Campo6", entity.Campo6);
            parameters.Add("Campo7", entity.Campo7);
            parameters.Add("Campo8", entity.Campo8);
            parameters.Add("Campo9", entity.Campo9);
            parameters.Add("Campo10", entity.Campo10);

            using IDbConnection db = new SqlConnection(_connectionString);
            db.Execute(_scriptInsert, parameters, commandType: CommandType.Text);
        }

        public void Update(TabelaTeste entity)
        {
            var parameters = new DynamicParameters();

            parameters.Add("Id", entity.Id);
            parameters.Add("Campo1", entity.Campo1);
            parameters.Add("Campo2", entity.Campo2);
            parameters.Add("Campo3", entity.Campo3);
            parameters.Add("Campo4", entity.Campo4);
            parameters.Add("Campo5", entity.Campo5);
            parameters.Add("Campo6", entity.Campo6);
            parameters.Add("Campo7", entity.Campo7);
            parameters.Add("Campo8", entity.Campo8);
            parameters.Add("Campo9", entity.Campo9);
            parameters.Add("Campo10", entity.Campo10);

            using IDbConnection db = new SqlConnection(_connectionString);
            db.Execute(_scriptUpdate, parameters, commandType: CommandType.Text);
        }

        public void Delete(TabelaTeste entity)
        {
            var parameters = new DynamicParameters();

            parameters.Add("Id", entity.Id);

            using IDbConnection db = new SqlConnection(_connectionString);
            db.Execute(_scriptDelete, parameters, commandType: CommandType.Text);
        }
    }
}
