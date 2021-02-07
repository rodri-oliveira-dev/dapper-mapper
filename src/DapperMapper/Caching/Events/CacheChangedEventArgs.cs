using DapperMapper.Caching.Enuns;
using System;

namespace DapperMapper.Caching.Events
{
    public class CacheChangedEventArgs : EventArgs
    {
        public CacheChangedEventArgs(string chave, ActionCache acao)
        {
            Chave = chave;
            Status = acao;
        }

        public string Chave { get; }

        public ActionCache Status { get; }
    }
}
