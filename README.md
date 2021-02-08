# Dapper.Mapper
Uma maneira simples para mapear suas entidades para as colunas do banco de dados ao usar o Dapper.

<hr>

### Introdução

A extensão permite configurar de maneira fácil o mapeamento entre as entidades e as colunas do banco de dados. Isso mantém sua entidade sincronizada. Se você tiver alguma dúvida, sugestão ou bug, por favor, não hesite em [me contatar](mailto:rodrigodotnet@gmail.com) ou crie uma issue.

<hr>

### Maneira de usar
#### Manual mapping
You can map property names manually using the [`EntityMap<TEntity>`](https://github.com/henkmollema/Dapper-FluentMap/blob/master/src/Dapper.FluentMap/Mapping/EntityMap.cs) class. When creating a derived class, the constructor gives you access to the `Map` method, allowing you to specify to which database column name a certain property of `TEntity` should map to.
```csharp
    [DapperTable("TabelaTeste")]
    public class TabelaTeste
    {
        [DapperColumn(PrimaryKey = true)]
        public Guid Id { get; set; }

        public string Campo1 { get; set; }

    }
```

Para mapear uma coluna com um nome diferente da propriedde use  `[DapperColumn(ColumnName = "Campo12")]` onde : `ColumnName` é o nome da coluna. Também é possivel definir quando o campo deve fazer parte do precesso de CRUD `[DapperColumn(Sync = AutoSync.OnlyOnInsert)]`.
