using MySql.Data.MySqlClient;

public class Comment : DBObject{

    /// <summary>
    /// Constructor to create a new comment, it will automatically update the database
    /// </summary>
    /// <param name="user">The user who pubished this comment</param>
    /// <param name="hack">The post where the comment has been published</param>
    /// <param name="content">The content of the comment</param>
    public Comment(BaseUser user, Hack hack, string content){

        this.relatedUser = user;
        this.relatedHack = hack;
        this.content = content;

        if (!this.ConstructForDB())
        {
            throw new Exception("The insertion of the object in the Database has failed");
        }
        // Get the id and the SQL server current date the DB give with AUTO-INCREMENT and then update the object with it

        IDictionary<string,string> dotEnv = FrizzusUtils.getEnvArray(@"\.env");
        MySqlConnection connection = new MySqlConnection($"server={dotEnv["DB_HOST"]};userid={dotEnv["DB_USER"]};password={dotEnv["DB_PASSWORD"]};database={dotEnv["DB_DATABASE"]};");
        connection.Open();

        MySqlCommand request = new MySqlCommand();
        request.Connection = connection;
            
        // Getting the database id and date based on the most recent date from this.relatedUser
        request.CommandText = "SELECT id_comment, last_updated FROM Comment WHERE last_updated = (SELECT MAX(last_updated) FROM Comment WHERE id_user = @relatedUser AND id_hack = @relatedHack)";
        request.Parameters.AddWithValue("@relatedUser", this.relatedUser);
        request.Parameters.AddWithValue("@relatedHack", this.relatedHack);

        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);

        connection.Close();
    }

    /// <summary>
    /// A detailed constructor for fetching data from the database
    /// </summary>
    /// <param name="id">Database id form the Comment Table</param>
    /// <param name="content">Content of the comment</param>
    /// <param name="nbLikes">Number of like of the comment</param>
    /// <param name="user">The user who post</param>
    /// <param name="hack">The post where the comment has been posted</param>
    /// <param name="lastUpdated">The last time the comment has been updated</param>
    public Comment(int id, string content, int nbLikes, BaseUser user, Hack hack, DateTime lastUpdated){
        this.id = id;
        this.content = content;
        this.nbLikes = nbLikes;
        this.relatedUser = user;
        this.relatedHack = hack;
        this._lastUpdated = lastUpdated;
    }

    public int id;
    public String content;
    public int nbLikes = 0;
    public BaseUser relatedUser;
    public Hack relatedHack;
    private DateTime _lastUpdated;


    public DateTime LastUpdated {get;}

    public bool DeleteFromDB(){
        IDictionary<string,string> dotEnv = FrizzusUtils.getEnvArray(@"\.env");
        try
        {
            MySqlConnection connection = new MySqlConnection($"server={dotEnv["DB_HOST"]};userid={dotEnv["DB_USER"]};password={dotEnv["DB_PASSWORD"]};database={dotEnv["DB_DATABASE"]};");
            connection.Open();

            MySqlCommand request = new MySqlCommand();
            request.Connection = connection;
            request.CommandText = "DELETE FROM Comment WHERE id_comment = @id";
            request.Parameters.AddWithValue("@id", this.id);
            request.Prepare();

            request.ExecuteNonQuery();

            connection.Close();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    public bool UpdateToDB(){
        IDictionary<string,string> dotEnv = FrizzusUtils.getEnvArray(@"\.env");
        try
        {
            MySqlConnection connection = new MySqlConnection($"server={dotEnv["DB_HOST"]};userid={dotEnv["DB_USER"]};password={dotEnv["DB_PASSWORD"]};database={dotEnv["DB_DATABASE"]};");
            connection.Open();

            MySqlCommand request = new MySqlCommand();
            request.Connection = connection;

            request.CommandText = "UPDATE Comment SET content = @content, nb_likes = @nbLikes";
            request.Parameters.AddWithValue("@content", this.content);
            request.Parameters.AddWithValue("@nbLikes", this.nbLikes);
            request.Prepare();
            
            request.ExecuteNonQuery();

            connection.Close();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public bool ConstructForDB(){
        IDictionary<string,string> dotEnv = FrizzusUtils.getEnvArray(@"\.env");
        try
        {
            MySqlConnection connection = new MySqlConnection($"server={dotEnv["DB_HOST"]};userid={dotEnv["DB_USER"]};password={dotEnv["DB_PASSWORD"]};database={dotEnv["DB_DATABASE"]};");
            connection.Open();

            MySqlCommand request = new MySqlCommand();
            request.Connection = connection;

            request.CommandText = "INSERT INTO Comment(content, nb_likes, id_hack, id_user) VALUES(@content, @nbLikes, @relatedHack, @relatedUser)";
            request.Parameters.AddWithValue("@content", this.content);
            request.Parameters.AddWithValue("@nbLikes", this.nbLikes);
            request.Parameters.AddWithValue("@relatedHack", this.relatedHack);
            request.Parameters.AddWithValue("@relatedUser", this.relatedUser);
            request.Prepare();

            request.ExecuteNonQuery();

            connection.Close();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}