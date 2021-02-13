using Dapper;
using DapperMapper.Attributes;
using DapperMapper.Caching;
using DapperMapper.Enums;
using DapperMapper.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DapperMapper.Repositories.Supports
{
    internal static class QueryMapper
    {
        private static readonly CachingMannager Cache = new CachingMannager(new TimeSpan(30, 0, 0, 0));

        internal static List<MappedEntityProperty<T>> RetornaDadosMap<T>()
        {
            var map = Cache.Recuperar<Dictionary<string, List<MappedEntityProperty<T>>>>("mapCmd").Value
                      ?? new Dictionary<string, List<MappedEntityProperty<T>>>();
            var keyMap = typeof(T).FullName;

            if (string.IsNullOrEmpty(keyMap))
            {
                return new List<MappedEntityProperty<T>>();
            }


            if (map.ContainsKey(keyMap))
            {
                return map[keyMap];
            }

            if (!typeof(T).GetCustomAttributes(typeof(DapperTable), true).Any())
            {
                throw new ArgumentOutOfRangeException($"Entidade não possui atributo {nameof(DapperTable)}");
            }

            var mapObj = typeof(T)
                .GetProperties()
                .Select(p => new MappedEntityProperty<T>(
                    p,
                    p.GetCustomAttributes(true).Where(att => att is DapperColumn).Cast<DapperColumn>().FirstOrDefault())
                ).ToList();

            map.Add(keyMap, mapObj);
            Cache.Adicionar("mapCmd", map);

            return mapObj;
        }

        internal static DynamicParameters RetornaParametroPrimaryKeyUnica<T>(dynamic paramValue, List<MappedEntityProperty<T>> entityMap)
        {
            var parametros = new DynamicParameters();

            if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
            }

            foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.PrimaryKey))
            {
                parametros.Add(coluna.DapperColumn.ColumnName, paramValue);
            }

            return parametros;
        }

        internal static DynamicParameters RetornaParametros<T>(T entity, List<MappedEntityProperty<T>> entityMap, QueryType tipoConsulta)
        {
            var parametros = new DynamicParameters();

            switch (tipoConsulta)
            {
                case QueryType.Insert:
                case QueryType.InsertWithCount:
                case QueryType.InsertWithReturn:
                    foreach (var coluna in entityMap.Where(coluna => (coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert)))
                    {
                        parametros.Add(coluna.DapperColumn.ParameterName, coluna.Getter(entity));
                    }
                    break;

                case QueryType.Update:
                case QueryType.UpdateWithCount:
                case QueryType.UpdateWithReturn:
                    foreach (var coluna in entityMap.Where(coluna => (coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnUpdate)))
                    {
                        parametros.Add(coluna.DapperColumn.ParameterName, coluna.Getter(entity));
                    }
                    break;

                case QueryType.SelectById:
                case QueryType.Delete:
                case QueryType.DeleteWithCount:
                    foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.PrimaryKey))
                    {
                        parametros.Add(coluna.DapperColumn.ParameterName, coluna.Getter(entity));
                    }
                    break;
                default:
                    return new DynamicParameters();
            }

            return parametros;
        }


        #region .: Metodos :.

        internal static string RetornaConsultaSql<T>(List<MappedEntityProperty<T>> entityMap, QueryType queryType)
        {
            switch (queryType)
            {
                case QueryType.SelectById:

                    if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
                    }

                    var whereById = new List<string>();

                    foreach (var coluna in entityMap.Where(c => c.DapperColumn.PrimaryKey))
                    {
                        whereById.Add($"{coluna.DapperColumn.ColumnName} = @{coluna.DapperColumn.ParameterName}");
                    }

                    return $"SELECT {string.Join(", ", entityMap.Select(c => c.ToString()).ToArray())} FROM {GetTableName(typeof(T))} WHERE {string.Join(" AND ", whereById.ToArray())};";

                case QueryType.SelectAll:
                    return $"SELECT {string.Join(", ", entityMap.Select(c => c.ToString()).ToArray())} FROM {GetTableName(typeof(T))};";

                case QueryType.Insert:
                case QueryType.InsertWithCount:
                    var camposInsert = entityMap
                        .Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert)
                        .Select(c => c.DapperColumn.ColumnName)
                        .ToList();

                    var parametrosInsert = entityMap
                        .Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert)
                        .Select(c => c.DapperColumn.ParameterName)
                        .ToList();

                    return $"INSERT INTO {GetTableName(typeof(T))} ({string.Join(", ", camposInsert.ToArray())}) VALUES (@{string.Join(" ,@", parametrosInsert.ToArray())});{(queryType == QueryType.InsertWithCount ? "SELECT @@ROWCOUNT;" : "")}";

                case QueryType.Update:
                case QueryType.UpdateWithCount:

                    if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
                    }

                    var camposUpdate = new List<string>();
                    var whereUpdate = new List<string>();

                    foreach (var coluna in entityMap.OrderByDescending(c => c.DapperColumn.PrimaryKey))
                    {
                        switch (coluna.DapperColumn.PrimaryKey)
                        {
                            case false when (coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnUpdate):
                                camposUpdate.Add($"{coluna.DapperColumn.ColumnName} = @{coluna.DapperColumn.ParameterName}");
                                break;

                            case true:
                                whereUpdate.Add($"{coluna.DapperColumn.ColumnName} = @{coluna.DapperColumn.ParameterName}");
                                break;
                        }
                    }

                    return $"UPDATE {GetTableName(typeof(T))} SET {string.Join(",", camposUpdate.ToArray())} WHERE {string.Join(" AND ", whereUpdate.ToArray())};{(queryType == QueryType.UpdateWithCount ? "SELECT @@ROWCOUNT;" : "")}";

                case QueryType.Delete:
                case QueryType.DeleteWithCount:

                    if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
                    }

                    var whereDel = new List<string>();

                    foreach (var coluna in entityMap.Where(c => c.DapperColumn.PrimaryKey))
                    {
                        whereDel.Add($"{coluna.DapperColumn.ColumnName} = @{coluna.DapperColumn.ParameterName}");
                    }

                    return $"DELETE FROM {GetTableName(typeof(T))} WHERE {string.Join(" AND ", whereDel.ToArray())};{(queryType == QueryType.DeleteWithCount ? "SELECT @@ROWCOUNT;" : "")}";

                default:
                    throw new ArgumentOutOfRangeException(nameof(queryType), "Tipo consulta não implementado.");
            }
        }

        internal static string GetTableName(Type entityType)
        {
            var attribute = (DapperTable)entityType.GetCustomAttributes(typeof(DapperTable), true).FirstOrDefault();

            if (attribute == null)
            {
                throw new ArgumentOutOfRangeException(nameof(entityType), $"Entidade não possui atributo {nameof(DapperTable)}");
            }

            return attribute.TableName;
        }

        #endregion
    }
}
