using Dapper;
using DapperMapper.Attributes;
using DapperMapper.CommandMapper;
using DapperMapper.Enums;
using DapperMapper.Mapper;
using DapperMapper.Results;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace DapperMapper.Repositories
{
    public class DapperRepository<T> where T : class, new()
    {
        private readonly string _connectionString;
        private readonly List<MappedEntityProperty<T>> _entityMap;

        public DapperRepository(string connectionString)
        {
            if (!typeof(T).GetCustomAttributes(typeof(DapperTable), true).Any())
            {
                throw new InvalidOperationException($"Entidade do tipo {typeof(T).FullName} não possui o atibuto mapper.");
            }

            _entityMap = EntityMap.ReturnEntityMap<T>();

            if (!_entityMap.Any(p => p.DapperColumn.PrimaryKey))
            {
                throw new InvalidOperationException($"Entidade do tipo {typeof(T).FullName} não possui uma chave primaria.");
            }

            _connectionString = connectionString;
        }

        public T GetById(dynamic id)
        {
            DynamicParameters parms = ParametersMapper.RetornaParametroPrimaryKeyUnica<T>(id, _entityMap);
            var selectAllCommand = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.SelectById);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<T>(selectAllCommand, parms, commandType: CommandType.Text).FirstOrDefault();
        }

        public T GetById(T entity)
        {
            var parms = ParametersMapper.RetornaParametros(entity, _entityMap, QueryType.SelectById);
            var selectByIdCommand = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.SelectById);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<T>(selectByIdCommand, parms, commandType: CommandType.Text).FirstOrDefault();
        }

        public List<T> GetAll()
        {
            var selectAllCommand = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.SelectAll);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<T>(selectAllCommand, null, commandType: CommandType.Text).ToList();
        }

        public int QuantidadeRegistros()
        {
            var selectCountCommand = $"SELECT COUNT(*) FROM {QueryMapper.GetTableName(typeof(T))};";

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.ExecuteScalar<int>(selectCountCommand, null, commandType: CommandType.Text);
        }

        public bool Insert(T entity)
        {
            var parms = ParametersMapper.RetornaParametros(entity, _entityMap, QueryType.Insert);
            var insertCommand = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.InsertWithCount);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.ExecuteScalar<int>(insertCommand, parms, commandType: CommandType.Text) > 0;
        }

        public void Insert(T entity, IDbConnection cnn, IDbTransaction trans)
        {
            if (cnn == null)
            {
                throw new ArgumentNullException(nameof(cnn), "Conexão não pode ser nula");
            }

            if (trans == null)
            {
                throw new ArgumentNullException(nameof(trans), "Transação não pode ser nula");
            }

            var parms = ParametersMapper.RetornaParametros<T>(entity, _entityMap, QueryType.Insert);
            var insertCommand = QueryMapper.ReturnSqlQuery<T>(_entityMap, QueryType.Insert);

            cnn.Query(insertCommand, parms, trans, commandType: CommandType.Text);
        }

        public QueryResult Insert(List<T> entityList)
        {
            var parms = ParametersMapper.RetornaParametros(entityList, _entityMap, QueryType.InsertMultiple);
            var insertCommand = QueryMapper.ReturnSqlQuery(entityList.Count, _entityMap, QueryType.InsertMultiple);

            using IDbConnection db = new SqlConnection(_connectionString);
            var queryReturn = db.Query(insertCommand, parms, commandType: CommandType.Text).FirstOrDefault();

            return GetSqlTransactionReturn(queryReturn);
        }

        public bool Update(T entity)
        {
            var parms = ParametersMapper.RetornaParametros(entity, _entityMap, QueryType.Update);
            var updateCommand = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.UpdateWithCount);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.ExecuteScalar<int>(updateCommand, parms, commandType: CommandType.Text) > 0;
        }

        public void Update(T entity, IDbConnection cnn, IDbTransaction trans)
        {
            if (cnn == null)
            {
                throw new ArgumentNullException(nameof(cnn), "Conexão não pode ser nula");
            }

            if (trans == null)
            {
                throw new ArgumentNullException(nameof(trans), "Transação não pode ser nula");
            }

            var parms = ParametersMapper.RetornaParametros(entity, _entityMap, QueryType.Update);
            var insertCommand = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.Update);

            cnn.Execute(insertCommand, parms, trans, commandType: CommandType.Text);
        }

        public bool Update(List<T> entityList)
        {
            var parms = ParametersMapper.RetornaParametros(entityList, _entityMap, QueryType.UpdateMultiple);
            var updateCommand = QueryMapper.ReturnSqlQuery(entityList.Count, _entityMap, QueryType.UpdateMultiple);

            using IDbConnection db = new SqlConnection(_connectionString);
            var queryReturn = db.Query(updateCommand, parms, commandType: CommandType.Text).FirstOrDefault();

            return GetSqlTransactionReturn(queryReturn);
        }

        public bool Delete(T entity)
        {
            var parms = ParametersMapper.RetornaParametros(entity, _entityMap, QueryType.Delete);
            var deleteSql = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.DeleteWithCount);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.ExecuteScalar<int>(deleteSql, parms, commandType: CommandType.Text) > 0;
        }

        public void Delete(T entity, IDbConnection cnn, IDbTransaction trans)
        {
            if (cnn == null)
            {
                throw new ArgumentNullException("cnn", "Conexão não pode ser nula");
            }

            if (trans == null)
            {
                throw new ArgumentNullException("trans", "Transação não pode ser nula");
            }

            var parms = ParametersMapper.RetornaParametros(entity, _entityMap, QueryType.Delete);
            var deleteSql = QueryMapper.ReturnSqlQuery(_entityMap, QueryType.Delete);

            using IDbConnection db = new SqlConnection(_connectionString);
            cnn.Execute(deleteSql, parms, trans, commandType: CommandType.Text);
        }

        public bool Delete(List<T> entityList)
        {
            var parms = ParametersMapper.RetornaParametros(entityList, _entityMap, QueryType.DeleteMultiple);
            var updateCommand = QueryMapper.ReturnSqlQuery(entityList.Count, _entityMap, QueryType.DeleteMultiple);

            using IDbConnection db = new SqlConnection(_connectionString);
            var queryReturn = db.Query(updateCommand, parms, commandType: CommandType.Text).FirstOrDefault();

            return GetSqlTransactionReturn(queryReturn);
        }

        private static QueryResult GetSqlTransactionReturn(dynamic queryReturn)
        {
            if (queryReturn is IDictionary<string, object> resultDictionary)
            {
                if (resultDictionary[""] is int affectedRows)
                {
                    return new QueryResult(affectedRows > 0, affectedRows);
                }
                else
                {
                    var errorNumber = (int)resultDictionary["ErrorNumber"];
                    var errorSeverity = (int)resultDictionary["ErrorSeverity"];
                    var errorState = (int)resultDictionary["ErrorState"];
                    string errorProcedure = (string)resultDictionary["ErrorProcedure"];
                    var errorLine = (int)resultDictionary["ErrorLine"];
                    string errorMessage = (string)resultDictionary["ErrorMessage"];

                    return new QueryResult(
                        false,
                        0,
                        new SqlInfoError(errorNumber, errorSeverity, errorState, errorProcedure, errorLine, errorMessage));
                }
            }
            return null;
        }
    }
}
