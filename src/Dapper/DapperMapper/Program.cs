using BenchmarkDotNet.Running;

namespace DapperMapper.Console
{
    class Program
    {

        private const string StringConnection = "Data Source=localhost,1433;Initial Catalog=ExemplosDapper;User Id=sa;Password=Hc#2020@3011;MultipleActiveResultSets=True;";

        static void Main(string[] args)
        {

            var repoNovo = new ProdutosRepository(StringConnection);

            //var produto = new Produtos
            //{
            //    Descricao = "Produto 1",
            //    Valor = 12.7,
            //    Ativo = true,
            //    CodigoIdentificacao = "1234567890"

            //};


            //repoNovo.Insert(produto);

            //produto.Descricao = "Produto-1";
            //produto.DataAtualizacao = DateTime.Now;

            //repoNovo.Update(produto);

            //repoNovo.Delete(produto);

            //var produtos = new List<Produtos>
            //{
            //    new Produtos
            //    {
            //        Descricao = "Produto 1",
            //        Valor = 12.7,
            //        Ativo = true,
            //        CodigoIdentificacao = "1234567890"
            //    },
            //    new Produtos
            //    {
            //        Descricao = "Produto 2",
            //        Valor = 12.7,
            //        Ativo = true,
            //        CodigoIdentificacao = "1234567890"
            //    },
            //    new Produtos
            //    {
            //        Descricao = "Produto 3",
            //        Valor = 12.7,
            //        Ativo = true,
            //        CodigoIdentificacao = "1234567890"
            //    },
            //    new Produtos
            //    {
            //        Descricao = "Produto 4",
            //        Valor = 12.7,
            //        Ativo = true,
            //        CodigoIdentificacao = "1234567890"
            //    },
            //};

            //repoNovo.Insert(produtos);

            //produtos[0].Descricao = "Produto-1";
            //produtos[0].DataAtualizacao = DateTime.Now;
            //produtos[1].Descricao = "Produto-2";
            //produtos[1].DataAtualizacao = DateTime.Now;
            //produtos[2].Descricao = "Produto-3";
            //produtos[2].DataAtualizacao = DateTime.Now;
            //produtos[3].Descricao = "Produto-4";
            //produtos[3].DataAtualizacao = DateTime.Now;

            //var totalInserido = repoNovo.QuantidadeRegistros();

            //repoNovo.Update(produtos);

            //repoNovo.Delete(produtos);

            var summary = BenchmarkRunner.Run<MemoryBenchmarkerDemo>();

        }
    }
}
