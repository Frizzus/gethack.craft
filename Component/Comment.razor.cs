using MySql.Data.MySqlClient;

public class Comment : DBObject{

    public int id;
    public String content;
    public int nbLikes = 0;
    public int relatedUserId;
    public int relatedHackId;
    private DateTime _lastUpdated;

    /// <summary>
    /// Constructor to create a new comment, it will automatically update the database
    /// </summary>
    /// <param name="user">The user who pubished this comment</param>
    /// <param name="hack">The post where the comment has been published</param>
    /// <param name="content">The content of the comment</param>
    public Comment(int user, int hack, string content, MySqlCommand request){

        this.relatedUserId = user;
        this.relatedHackId = hack;
        this.content = content;

        if (!this.ConstructForDB(request))
        {
            throw new Exception("The insertion of the object in the Database has failed");
        }
        // Get the id and the SQL server current date the DB give with AUTO-INCREMENT and then update the object with it
            
        // Getting the database id and date based on the most recent date from this.relatedUser
        request.CommandText = "SELECT id_comment, last_updated FROM Comment WHERE last_updated = (SELECT MAX(last_updated) FROM Comment WHERE id_user = @relatedUser AND id_hack = @relatedHack)";
        request.Parameters.AddWithValue("@relatedUser", this.relatedUserId);
        request.Parameters.AddWithValue("@relatedHack", this.relatedHackId);

        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);
    }

    public Comment(int id, MySqlCommand request){
        request.CommandText = "SELECT * FROM Comment WHERE id_comment = @id";
        request.Parameters.AddWithValue("@id", id);
        MySqlDataReader reader = request.ExecuteReader();

        this.id = id;
        this.content = reader.GetString("content");
        this.nbLikes = reader.GetInt32("nb_likes");
        this.relatedUserId = reader.GetInt32("id_user");
        this.relatedHackId = reader.GetInt32("id_hack");
        this._lastUpdated = reader.GetDateTime("last_updated");
    }


    public DateTime LastUpdated {get;}

    public bool DeleteFromDB(MySqlCommand request){
        try
        {
            request.CommandText = "DELETE FROM Comment WHERE id_comment = @id";
            request.Parameters.AddWithValue("@id", this.id);
            request.Prepare();

            request.ExecuteNonQuery();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public bool UpdateToDB(MySqlCommand request){
        try
        {
            request.CommandText = "UPDATE Comment SET content = @content, nb_likes = @nbLikes";
            request.Parameters.AddWithValue("@content", this.content);
            request.Parameters.AddWithValue("@nbLikes", this.nbLikes);
            request.Prepare();
            
            request.ExecuteNonQuery();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public bool ConstructForDB(MySqlCommand request){
        try
        {

            request.CommandText = "INSERT INTO Comment(content, nb_likes, id_hack, id_user) VALUES(@content, @nbLikes, @relatedHack, @relatedUser)";
            request.Parameters.AddWithValue("@content", this.content);
            request.Parameters.AddWithValue("@nbLikes", this.nbLikes);
            request.Parameters.AddWithValue("@relatedHack", this.relatedHackId);
            request.Parameters.AddWithValue("@relatedUser", this.relatedUserId);
            request.Prepare();

            request.ExecuteNonQuery();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}