using Dapper;
using DapperMapper.Enums;
using DapperMapper.Mapper;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DapperMapper.CommandMapper
{
    internal static class ParametersMapper
    {
        internal static DynamicParameters RetornaParametroPrimaryKeyUnica<T>(dynamic paramValue, List<MappedEntityProperty<T>> entityMap)
        {
            var parametros = new DynamicParameters();

            if (entityMap.Count(coluna => coluna.DapperColumn.PrimaryKey) > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(entityMap), "Entidade não possui um atributo PrimaryKey definido.");
            }

            foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.PrimaryKey))
            {
                parametros.Add(coluna.DapperColumn.ParameterName, paramValue);
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
                    foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert))
                    {
                        parametros.Add(coluna.DapperColumn.ParameterName, coluna.Getter(entity));
                    }
                    break;

                case QueryType.Update:
                case QueryType.UpdateWithCount:
                case QueryType.UpdateWithReturn:
                    foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnUpdate))
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

        internal static DynamicParameters RetornaParametros<T>(List<T> entities, List<MappedEntityProperty<T>> entityMap, QueryType tipoConsulta)
        {
            var parametros = new DynamicParameters();

            switch (tipoConsulta)
            {
                case QueryType.InsertMultiple:
                    var paramInsertNumber = 1;

                    foreach (var entity in entities)
                    {
                        foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnInsert))
                        {
                            parametros.Add($"{coluna.DapperColumn.ParameterName}{paramInsertNumber}", coluna.Getter(entity));
                        }

                        paramInsertNumber++;
                    }
                    break;

                case QueryType.UpdateMultiple:
                    var paramUpdateNumber = 1;

                    foreach (var entity in entities)
                    {
                        foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.Sync == AutoSync.Ever || coluna.DapperColumn.Sync == AutoSync.OnlyOnUpdate))
                        {
                            parametros.Add($"{coluna.DapperColumn.ParameterName}{paramUpdateNumber}", coluna.Getter(entity));
                        }

                        paramUpdateNumber++;
                    }
                    break;

                case QueryType.DeleteMultiple:
                    var paramDeleteNumber = 1;

                    foreach (var entity in entities)
                    {
                        foreach (var coluna in entityMap.Where(coluna => coluna.DapperColumn.PrimaryKey))
                        {
                            parametros.Add($"{coluna.DapperColumn.ParameterName}{paramDeleteNumber}", coluna.Getter(entity));
                        }

                        paramDeleteNumber++;
                    }

                    break;
                default:
                    return new DynamicParameters();
            }

            return parametros;
        }
    }
}
