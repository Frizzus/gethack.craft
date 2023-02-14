using MySql.Data.MySqlClient;

public class Comment : DBObject{

    // TODO : makes 2 constructor one for creating from Zero and inserting it into the DB and one to fethc from the DB
    public Comment(BaseUser relatedU, Hack relatedH, String content = "Placeholder"){
        this.relatedUser = relatedU;
        this.relatedHack = relatedH;
        this.content = content;
        this.nbLikes = 0;
    }

    public int id;
    public String content;
    public int nbLikes;
    public BaseUser relatedUser;
    public Hack relatedHack;
    private DateTime _recentUpdate;


    public DateTime RecentUpdate {get;}

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

            // Get the id the DB give with AUTO-INCREMENT and then update the object with it

            // TODO : add date where the comment and other classes has been created

            connection.Close();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}