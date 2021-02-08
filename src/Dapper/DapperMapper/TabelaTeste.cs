using DapperMapper.Attributes;
using System;

namespace DapperMapper.Console
{
    [DapperTable("TabelaTeste")]
    public class TabelaTeste
    {

        public TabelaTeste()
        {
            Id = Guid.NewGuid();
        }
        [DapperColumn(PrimaryKey = true)]
        public Guid Id { get; set; }

        [DapperColumn]
        public string Campo1 { get; set; }

        [DapperColumn]
        public string Campo2 { get; set; }

        [DapperColumn]
        public string Campo3 { get; set; }

        [DapperColumn]
        public string Campo4 { get; set; }

        [DapperColumn]
        public string Campo5 { get; set; }

        [DapperColumn]
        public string Campo6 { get; set; }

        [DapperColumn]
        public string Campo7 { get; set; }

        [DapperColumn]
        public string Campo8 { get; set; }

        [DapperColumn]
        public string Campo9 { get; set; }

        [DapperColumn]
        public string Campo10 { get; set; }
    }
}
