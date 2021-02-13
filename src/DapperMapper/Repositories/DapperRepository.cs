using Dapper;
using DapperMapper.Attributes;
using DapperMapper.Enums;
using DapperMapper.Mapper;
using DapperMapper.Repositories.Supports;
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

            _entityMap = QueryMapper.RetornaDadosMap<T>();

            if (!_entityMap.Any(p => p.DapperColumn.PrimaryKey))
            {
                throw new InvalidOperationException($"Entidade do tipo {typeof(T).FullName} não possui uma chave primaria.");
            }

            _connectionString = connectionString;
        }

        public T GetById(dynamic id)
        {
            DynamicParameters parms = QueryMapper.RetornaParametroPrimaryKeyUnica<T>(id, _entityMap);
            var selectAllCommand = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.SelectById);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<T>(selectAllCommand, parms, commandType: CommandType.Text).FirstOrDefault();
        }

        public T GetById(T entity)
        {
            var parms = QueryMapper.RetornaParametros(entity, _entityMap, QueryType.SelectById);
            var selectByIdCommand = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.SelectById);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.Query<T>(selectByIdCommand, parms, commandType: CommandType.Text).FirstOrDefault();
        }

        public List<T> GetAll()
        {
            var selectAllCommand = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.SelectAll);

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
            var parms = QueryMapper.RetornaParametros(entity, _entityMap, QueryType.Insert);
            var insertCommand = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.InsertWithCount);

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

            var parms = QueryMapper.RetornaParametros<T>(entity, _entityMap, QueryType.Insert);
            var insertCommand = QueryMapper.RetornaConsultaSql<T>(_entityMap, QueryType.Insert);

            cnn.Execute(insertCommand, parms, trans, commandType: CommandType.Text);
        }

        public bool Insert(List<T> entityList)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var trans = db.BeginTransaction();

                try
                {
                    foreach (var entity in entityList)
                    {
                        Insert(entity, db, trans);
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }

        public bool Update(T entity)
        {
            var parms = QueryMapper.RetornaParametros(entity, _entityMap, QueryType.Update);
            var updateCommand = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.UpdateWithCount);

            using IDbConnection db = new SqlConnection(_connectionString);
            return db.ExecuteScalar<int>(updateCommand, parms, commandType: CommandType.Text) > 0;
        }

        public virtual void Update(T entity, IDbConnection cnn, IDbTransaction trans)
        {
            if (cnn == null)
            {
                throw new ArgumentNullException(nameof(cnn), "Conexão não pode ser nula");
            }

            if (trans == null)
            {
                throw new ArgumentNullException(nameof(trans), "Transação não pode ser nula");
            }

            var parms = QueryMapper.RetornaParametros(entity, _entityMap, QueryType.Update);
            var insertCommand = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.Update);

            cnn.Execute(insertCommand, parms, trans, commandType: CommandType.Text);
        }

        public bool Update(List<T> entityList)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var trans = db.BeginTransaction();

                try
                {
                    foreach (var entity in entityList)
                    {
                        Insert(entity, db, trans);
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }

        public bool Delete(T entity)
        {
            var parms = QueryMapper.RetornaParametros(entity, _entityMap, QueryType.Delete);
            var deleteSql = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.DeleteWithCount);

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

            var parms = QueryMapper.RetornaParametros(entity, _entityMap, QueryType.Delete);
            var deleteSql = QueryMapper.RetornaConsultaSql(_entityMap, QueryType.Delete);

            using IDbConnection db = new SqlConnection(_connectionString);
            cnn.Execute(deleteSql, parms, trans, commandType: CommandType.Text);
        }

        public virtual bool Delete(List<T> entityList)
        {
            using (IDbConnection db = new SqlConnection(_connectionString))
            {
                var trans = db.BeginTransaction();

                try
                {
                    foreach (var entity in entityList)
                    {
                        Delete(entity, db, trans);
                    }

                    trans.Commit();
                    return true;
                }
                catch
                {
                    trans.Rollback();
                    return false;
                }
            }
        }
    }
}
