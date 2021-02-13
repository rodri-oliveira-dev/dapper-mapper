using DapperMapper.Attributes;
using System;

namespace DapperMapper.Console
{
    [DapperTable("Contrato")]
    public class Contrato
    {
        [DapperColumn(PrimaryKey = true)]
        public Guid Id { get; set; }

        public string NomeContrato { get; set; }
    }
}
