using DapperMapper.Attributes;
using DapperMapper.Enums;
using System;

namespace DapperMapper.Console
{
    [DapperTable("Produtos")]
    public class Produtos
    {
        public Produtos()
        {
            Id = Guid.NewGuid();
            DataCadastro = DateTime.Now;
        }

        [DapperColumn(PrimaryKey = true)]
        public Guid Id { get; set; }

        [DapperColumn("CodigoBarra")]
        public string CodigoIdentificacao { get; set; }

        public string Descricao { get; set; }

        public double Valor { get; set; }

        public bool Ativo { get; set; }

        [DapperColumn(Sync = AutoSync.OnlyOnInsert)]
        public DateTime DataCadastro { get; set; }

        [DapperColumn(Sync = AutoSync.OnlyOnUpdate)]
        public DateTime? DataAtualizacao { get; set; }
    }
}
