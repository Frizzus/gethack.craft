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
    public Hack(BaseUser user, string title, string type, string tags = "", string description = ""){
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
        this.relatedUser = user;
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
        request.Parameters.AddWithValue("@relatedUser", this.relatedUser);

        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);

        connection.Close();
    }

    /// <summary>
    /// A detailed Constructor for fetching a record of the Hack table and turn him in object without re-inserting an new record
    /// </summary>
    /// <param name="id">Id of the record fetched</param>
    /// <param name="title">Title of the post</param>
    /// <param name="tags">Tags to help with searches</param>
    /// <param name="imgLink">Link to the image of the hack</param>
    /// <param name="description">Content of the post</param>
    /// <param name="nbLikes">Number of time the users have liked the post</param>
    /// <param name="reported">Is this hack has been reported by a user</param>
    /// <param name="reasonReported">If the post has been reported => reason why the hack need to be judge by an admin</param>
    /// <param name="relatedUser">The user who own the post</param>
    /// <param name="hackType">The category of the post (build, redstone)</param>
    /// <param name="lastUpdated">The last time this hack has been updated</param>
    public Hack(int id, string title, string tags, string imgLink, string description, int nbLikes, bool reported, string reasonReported, BaseUser relatedUser, string hackType, DateTime lastUpdated){
        this.id = id;
        this.title = title;
        this.Tags = tags;
        this.imgLink = imgLink;
        this.description = description;
        this.nbLikes = nbLikes;
        this.reported = reported;
        this.reasonReported = reasonReported;
        this.relatedUser = relatedUser;
        this._hackType = hackType;
        this._lastUpdated = lastUpdated;
    }

    
    public int id;
    public string? title = "default";
    private string[] _tags = new string[0];
    private string? imgLink = "placeholder.svg";
    public string description;
    public int nbLikes;
    public bool reported;
    public string? reasonReported;
    public BaseUser relatedUser;
    private string _hackType;
    private DateTime _lastUpdated;


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