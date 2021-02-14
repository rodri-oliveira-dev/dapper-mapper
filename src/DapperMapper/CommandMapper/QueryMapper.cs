using DapperMapper.Attributes;
using DapperMapper.Enums;
using DapperMapper.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DapperMapper.CommandMapper
{
    internal static class QueryMapper
    {
        private const string TransactionTemplate = @"BEGIN TRANSACTION;  
                                     BEGIN TRY  
	                                    {query}
                                        SELECT @@ROWCOUNT;

                                     END TRY  
                                     BEGIN CATCH  
                                        SELECT   
                                            ERROR_NUMBER() AS ErrorNumber  
                                            ,ERROR_SEVERITY() AS ErrorSeverity  
                                            ,ERROR_STATE() AS ErrorState  
                                            ,ERROR_PROCEDURE() AS ErrorProcedure  
                                            ,ERROR_LINE() AS ErrorLine  
                                            ,ERROR_MESSAGE() AS ErrorMessage;  
  
                                     IF @@TRANCOUNT > 0  
                                        ROLLBACK TRANSACTION;  
                                     END CATCH;  
                                     IF @@TRANCOUNT > 0  
                                        COMMIT TRANSACTION;";




        internal static string ReturnSqlQuery<T>(List<MappedEntityProperty<T>> entityMap, QueryType queryType) where T : class, new()
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
                            case false when coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnUpdate:
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

        internal static string ReturnSqlQuery<T>(int totalEntities, List<MappedEntityProperty<T>> entityMap, QueryType queryType) where T : class, new()
        {
            switch (queryType)
            {
                case QueryType.InsertMultiple:

                    var camposInsert = entityMap
                        .Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert)
                        .Select(c => c.DapperColumn.ColumnName)
                        .ToList();

                    var queryInsert = new StringBuilder($"INSERT INTO {GetTableName(typeof(T))} ({string.Join(", ", camposInsert.ToArray())}) VALUES ");

                    for (int i = 1; i <= totalEntities; i++)
                    {
                        var parametrosInsert = entityMap
                            .Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert)
                            .Select(c => $"@{c.DapperColumn.ParameterName}{i}")
                            .ToList();

                        queryInsert.AppendLine($"({string.Join(" ,", parametrosInsert.ToArray())}){(i < totalEntities ? "," : "")}");
                    }

                    queryInsert.Append(";");

                    return TransactionTemplate.Replace("{query}", queryInsert.ToString());

                case QueryType.UpdateMultiple:

                    if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
                    }

                    var queryUpdate = new StringBuilder();

                    for (int i = 1; i <= totalEntities; i++)
                    {
                        var camposUpdate = new List<string>();
                        var whereUpdate = new List<string>();

                        foreach (var coluna in entityMap.OrderByDescending(c => c.DapperColumn.PrimaryKey))
                        {
                            switch (coluna.DapperColumn.PrimaryKey)
                            {
                                case false when coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnUpdate:
                                    camposUpdate.Add($"{coluna.DapperColumn.ColumnName} = @{$"{coluna.DapperColumn.ParameterName}{i}"}");
                                    break;

                                case true:
                                    whereUpdate.Add($"{coluna.DapperColumn.ColumnName} = @{$"{coluna.DapperColumn.ParameterName}{i}"}");
                                    break;
                            }
                        }

                        queryUpdate.AppendLine($"UPDATE {GetTableName(typeof(T))} SET {string.Join(",", camposUpdate.ToArray())} WHERE {string.Join(" AND ", whereUpdate.ToArray())};");
                    }

                    return TransactionTemplate.Replace("{query}", queryUpdate.ToString());

                case QueryType.DeleteMultiple:

                    if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) == 0)
                    {
                        throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
                    }

                    var queryDelete = new StringBuilder();


                    if (entityMap.Count(c => c.DapperColumn.PrimaryKey) > 1)
                    {
                        var whereDel = new List<string>();

                        for (int i = 1; i <= totalEntities; i++)
                        {
                            foreach (var coluna in entityMap.Where(c => c.DapperColumn.PrimaryKey))
                            {
                                whereDel.Add($"{coluna.DapperColumn.ColumnName} = @{$"{coluna.DapperColumn.ParameterName}{i}"}");
                            }
                        }
                        queryDelete.AppendLine($"DELETE FROM {GetTableName(typeof(T))} WHERE {string.Join(" AND ", whereDel.ToArray())};");
                    }
                    else
                    {
                        var whereDel = new List<string>();

                        for (int i = 1; i <= totalEntities; i++)
                        {
                            foreach (var coluna in entityMap.Where(c => c.DapperColumn.PrimaryKey))
                            {
                                whereDel.Add($"@{$"{coluna.DapperColumn.ParameterName}{i}"}");
                            }
                        }
                        queryDelete.AppendLine($"DELETE FROM {GetTableName(typeof(T))} WHERE {entityMap.First(c => c.DapperColumn.PrimaryKey).DapperColumn.ColumnName} IN ({string.Join(", ", whereDel.ToArray())});");
                    }

                    return TransactionTemplate.Replace("{query}", queryDelete.ToString());

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
    }
}
