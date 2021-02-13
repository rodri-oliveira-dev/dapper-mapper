using DapperMapper.Attributes;
using System;

namespace DapperMapper.Console
{
    [DapperTable("TabelaTeste")]
    public class TabelaTeste
    {
        [DapperColumn(PrimaryKey = true)]
        public Guid Id { get; set; }

        [DapperColumn("Campo1")]
        public string Campo { get; set; }


        public string Campo2 { get; set; }


        public string Campo3 { get; set; }


        public string Campo4 { get; set; }


        public string Campo5 { get; set; }


        public string Campo6 { get; set; }


        public string Campo7 { get; set; }


        public string Campo8 { get; set; }


        public string Campo9 { get; set; }


        public string Campo10 { get; set; }
    }
}
