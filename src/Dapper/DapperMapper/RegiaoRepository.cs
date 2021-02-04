using System;
using System.Collections.Generic;
using System.Text;

namespace DapperMapper
{
    public class RegiaoRepository : DapperRepositoryBase<Regioes>
    {
        public RegiaoRepository(string connectionString) : base(connectionString)
        {
        }
    }
}
