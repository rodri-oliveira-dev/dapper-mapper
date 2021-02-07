using DapperMapper.Caching.Events;
using System;
using System.Collections.Generic;

namespace DapperMapper.Caching.Interfaces
{
    public interface ICachingProvider : IDisposable
    {
        void Adicionar<T>(string chave, T valor);
        void Remover(string chave);
        bool Existe(string chave);
        KeyValuePair<bool, T> Recuperar<T>(string chave, bool removerAposRecuperar = false);
        void Limpar();

        event CacheChangedEventHandler CacheChanged;
    }
}
