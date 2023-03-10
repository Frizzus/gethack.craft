using MySql.Data.MySqlClient;
interface DBObject{
    /// <summary>
    /// Delete the object's related record from the database,  the C# object is not deleted !
    /// </summary>
    /// <returns>True if the record has been deleted, else false</returns>
    public bool DeleteFromDB(MySqlCommand request);
    /// <summary>
    /// Update the object's related record from the database
    /// </summary>
    /// <returns>True if the record has been updated, else false</returns>
    public bool UpdateToDB(MySqlCommand request);
    /// <summary>
    /// Insert a record corresponding to an object
    /// </summary>
    /// <returns>True if the record has been created, else false</returns>
    public bool ConstructForDB(MySqlCommand request);
}