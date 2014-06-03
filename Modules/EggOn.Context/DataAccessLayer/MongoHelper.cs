using MongoDB.Driver;
namespace Context.DataAccessLayer
{
    /// <summary>
    /// Classe auxiliar que serve para a criação de uma ligação com a base de dados. 
    /// Todos os serviços encarregues de fazer a gestão da sua coleção na base de dados têm um 
    /// objecto deste tipo.
    /// </summary>
    /// <typeparam name="T">
    /// Tipo de Entidade a representa
    /// </typeparam>
    public class MongoHelper<T> where T : class
    {
        public MongoCollection<T> Collection { get; private set; }

        public MongoHelper()
        {
            var con = new MongoConnectionStringBuilder("EggOn");
            var server = MongoServer.Create(con);
            var db = server.GetDatabase(con.DatabaseName);
            Collection = db.GetCollection<T>(typeof(T).Name.ToLower());
        }
    }
}