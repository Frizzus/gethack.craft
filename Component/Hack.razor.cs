using MySql.Data.MySqlClient;

/// <summary>
/// A Class representing a getcraft post
/// </summary>
public struct Hack{

    /// <summary>
    /// Constructor method based on value passed as parameter (insert the new hack to the database)
    /// </summary>
    /// <param name="user">The user who own the post</param>
    /// <param name="title">The title of the Hack</param>
    /// <param name="type">The category of the post (build, redstone)</param>
    /// <param name="tags">Some tag to help with searches</param>
    public Hack(int user, string title, string type, string tags = "", string description = ""){
        if (this.title == "default" || this._hackType == "default")
        {
            throw new Exception("title and type have to be specified");
        }

        this.title = title;
        this.Tags = type + tags;
        this.description = "";
        this.nbLikes = 0;
        this.reported = false;
        this.reasonReported = null;
        this.relatedUserId = user;
        this._hackType = type;

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
        request.CommandText = "SELECT id_hack, last_updated FROM Hack WHERE last_updated = (SELECT MAX(last_updated) FROM Hack WHERE id_user = @relatedUser)";
        request.Parameters.AddWithValue("@relatedUser", this.relatedUserId);

        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);

        connection.Close();
    }


    public Hack(int id, MySqlCommand request){
        request.CommandText = "SELECT * FROM Hack WHERE id_hack = @id";
        request.Parameters.AddWithValue("@id", id);
        MySqlDataReader reader = request.ExecuteReader();

        this.id = id;
        this.title = reader.GetString("title");
        this.Tags = reader.GetString("tags");
        this.imgLink = reader.GetString("img_url");
        this.description = reader.GetString("description");
        this.nbLikes = reader.GetInt32("nb_likes");
        this.reported = reader.GetBoolean("reported");
        this.reasonReported = reader.GetString("reason_reported");
        this._hackType = reader.GetString("hack_type");
        this._lastUpdated = reader.GetDateTime("last_updated");
        this.relatedUserId = reader.GetInt32("id_user");
    }

    
    public int id;
    public string? title = "default";
    private string[] _tags = new string[0];
    private string? imgLink = "placeholder.svg";
    public string description;
    public int nbLikes;
    public bool reported;
    public string? reasonReported;
    private string _hackType;
    private DateTime _lastUpdated;
    private int relatedUserId;


    /// <summary>
    /// Property used to get the relatedUser without the need to get the whole database by recursion
    /// </summary>
    /// <value></value>
    public BaseUser RelatedUser(MySqlCommand request) {
        // check if the user is an admin or not
        request.CommandText = "SELECT is_admin FROM User WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", this.id);
        MySqlDataReader reader = request.ExecuteReader(); 

        if (reader.GetBoolean("is_admin"))
        {
            return new UserAdmin(this.relatedUserId, request);
        }
        else
        {
            return new User(this.relatedUserId, request);
        }
    }

    public DateTime LastUpdated {get;}
    public string Tags {
        get {
            string temp = "";
            foreach (var tag in this._tags)
            {
                temp = string.Concat(temp+" ", tag);
            }
            return temp;
        }
        set {
            this._tags = value.Split(' ');
        }
    }

    public string HackType{
        get => this._hackType;
        set{
            if (!(Equals(value, "build") || Equals(value, "redstone")))
            {
                throw new Exception("\"" + value + "\" is not a valid value (\"build\" OR \"redstone\")");
            }
            this._hackType = value;
        }
    }

    public bool ConstructForDB(){return false;}
    public bool UpdateToDB(){return false;}
    public bool DeleteFromDB(){return false;}


}