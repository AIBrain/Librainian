namespace Librainian.Magic.Abodit {
    using MongoDB.Bson;

    /// <summary>
    ///     All MongoDynamic objects support this interface because every object needs an _id in MongoDB
    /// </summary>
    public interface IId {
        ObjectId ID { get; set; }
    }
}