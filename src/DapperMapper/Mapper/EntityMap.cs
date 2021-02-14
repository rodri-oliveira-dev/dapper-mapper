using DapperMapper.Attributes;
using DapperMapper.Caching;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DapperMapper.Mapper
{
    internal static class EntityMap
    {
        private static readonly CachingMannager Cache = new CachingMannager(new TimeSpan(30, 0, 0, 0));

        internal static List<MappedEntityProperty<T>> ReturnEntityMap<T>()
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
    }
}
