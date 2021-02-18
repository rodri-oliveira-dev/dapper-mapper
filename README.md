# Dapper.Mapper
Uma maneira simples para mapear suas entidades para as tabelas e colunas de sua tabela do banco de dados sem afetar seu projeto que utiliza Dapper.

<hr>

## Introdução

A extensão permite configurar de maneira fácil o mapeamento entre as entidades e as colunas do banco de dados. Isso permite que mantém sua entidade sincronizada. Se você tiver alguma dúvida, sugestão ou bug, por favor, não hesite em [me contatar](mailto:rodrigodotnet@gmail.com) ou crie uma issue.

<hr>

### Maneira de usar

```csharp
    [DapperTable("TabelaTeste")]
    public class TabelaTeste
    {
        [DapperColumn(PrimaryKey = true)]
        public Guid Id { get; set; }

        public string Campo1 { get; set; }

    }
```

Para mapear uma coluna que tenha um nome diferente da propriede use  `[DapperColumn(ColumnName = "Nome_Franquia")]` onde : `ColumnName` é o nome da coluna. Também é possivel definir quando o campo deve fazer parte do precesso de CRUD através da propriedade Sync `[DapperColumn(Sync = AutoSync.OnlyOnInsert)]`.

<hr>

## Exemplo de mappeing entidade => tabela

### A Entidade

```csharp
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
```

Nessa classe temos alguns modelos de usso do mapping:

* A propriedade `Id` como chave primaria;
* A propriedade `CodigoIdentificacao` mapeia faz vinculo com a coluna  `CodigoBarra` nesse caso temos uma propriedade com nome diferente da respectiva coluna do banco.
* A propriedade `DataCadastro` tem a definição `Sync = AutoSync.OnlyOnInsert` que define que essa propriedade só deve ser usada no Insert.
* A propriedade `DataAtualizacao` tem a definição `Sync = AutoSync.OnlyOnUpdate` que define que essa propriedade só deve ser usada no Update.

### O repository

```csharp
    public class ProdutosRepository : DapperRepository<Produtos>
    {
        public ProdutosRepository(string connectionString) : base(connectionString)
        {
        }
    }
```

Com isso podemos começar a efetuar nossa chamdas:

```csharp
            var repoNovo = new ProdutosRepository(StringConnection);

            var produto = new Produtos
            {
                Descricao = "Produto 1",
                Valor = 12.7,
                Ativo = true,
                CodigoIdentificacao = "1234567890"

            };

            // Efetua um INSERT da entidade.
            repoNovo.Insert(produto);

            produto.Descricao = "Produto-1";
            produto.DataAtualizacao = DateTime.Now;

            // Efetua um UPDATE da entidade.
            repoNovo.Update(produto);

            // Efetua um DELETE da entidade.
            repoNovo.Delete(produto);

            // Define uma lista de entidades
            var produtos = new List<Produtos>
            {
                new Produtos
                {
                    Descricao = "Produto 1",
                    Valor = 12.7,
                    Ativo = true,
                    CodigoIdentificacao = "1234567890"
                },
                new Produtos
                {
                    Descricao = "Produto 2",
                    Valor = 12.7,
                    Ativo = true,
                    CodigoIdentificacao = "1234567890"
                },
                new Produtos
                {
                    Descricao = "Produto 3",
                    Valor = 12.7,
                    Ativo = true,
                    CodigoIdentificacao = "1234567890"
                },
                new Produtos
                {
                    Descricao = "Produto 4",
                    Valor = 12.7,
                    Ativo = true,
                    CodigoIdentificacao = "1234567890"
                },
            };

            // Efetua um INSERT de uma lista de entidades.
            repoNovo.Insert(produtos);

            produtos[0].Descricao = "Produto-1";
            produtos[0].DataAtualizacao = DateTime.Now;
            produtos[1].Descricao = "Produto-2";
            produtos[1].DataAtualizacao = DateTime.Now;
            produtos[2].Descricao = "Produto-3";
            produtos[2].DataAtualizacao = DateTime.Now;
            produtos[3].Descricao = "Produto-4";
            produtos[3].DataAtualizacao = DateTime.Now;

            // Quantidade de registros
            var totalInserido = repoNovo.QuantidadeRegistros();

            // Efetua um UPDATE de uma lista de entidades.
            repoNovo.Update(produtos);

            // Efetua um DELETE de uma lista de entidades.
            repoNovo.Delete(produtos);
```