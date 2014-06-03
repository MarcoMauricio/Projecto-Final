﻿using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using EggOn.Context.Models;

namespace Context.Models
{
    /// <summary>
    /// Representação do documento base que irá ser representado com :
    /// Category -> Categoria atribuida ao documento respectivo
    /// Lista de Entitades -> Todas as entidades encontradas nesse documento
    /// Summary -> Sumário do texto contido no documento
    /// </summary>
    public class Document
    {
        [BsonId]
        public ObjectId Id { get; set; }
        public string TableName { get; set; }
        public string TableIndex { get; set; }
        public List<Entity> Entities { get; set; }
        public Summary Summary { get; set; }
        public Category Category { get; set; }
    }
}