using MySql.Data.MySqlClient;

/// <summary>
/// A Class representing a getcraft post
/// </summary>
public struct Hack : DBObject{

    /// <summary>
    /// Constructor method based on value passed as parameter (insert the new hack to the database)
    /// </summary>
    /// <param name="user">The user who own the post</param>
    /// <param name="title">The title of the Hack</param>
    /// <param name="type">The category of the post (build, redstone)</param>
    /// <param name="tags">Some tag to help with searches</param>
    public Hack(int user, string title, string type, MySqlCommand request, string tags = "", string description = ""){
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

        if (!this.ConstructForDB(request))
        {
            throw new Exception("The insertion of the object in the Database has failed");
        }
        // Get the id and the SQL server current date the DB give with AUTO-INCREMENT and then update the object with it
            
        // Getting the database id and date based on the most recent date from this.relatedUser
        request.CommandText = "SELECT id_hack, last_updated FROM Hack WHERE last_updated = (SELECT MAX(last_updated) FROM Hack WHERE id_user = @relatedUser)";
        request.Parameters.AddWithValue("@relatedUser", this.relatedUserId);
        request.Prepare();
        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);
    }


    public Hack(int id, MySqlCommand request){
        request.CommandText = "SELECT * FROM Hack WHERE id_hack = @id";
        request.Parameters.AddWithValue("@id", id);
        request.Prepare();
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
        request.Prepare();
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

    public bool ConstructForDB(MySqlCommand request){
        try
        {
            request.CommandText = "INSERT INTO Hack(title, img_url, description, nb_likes, reported, reason_reported, hack_type, id_user, last_updated, tags) VALUES(@title, @img_url, @description, @nb_likes, @reported, @reason_reported, @hack_type, @id_user, @last_updated, @tags)";
            request.Parameters.AddWithValue("@title", this.title);
            request.Parameters.AddWithValue("@img_url", this.imgLink);
            request.Parameters.AddWithValue("@description", this.description);
            request.Parameters.AddWithValue("@nb_likes", this.nbLikes);
            request.Parameters.AddWithValue("@reported", this.reported);
            request.Parameters.AddWithValue("@reason_reported", this.reasonReported);
            request.Parameters.AddWithValue("@hack_type", this.HackType);
            request.Parameters.AddWithValue("@id_user", this.relatedUserId);
            request.Parameters.AddWithValue("@last_updated", this._lastUpdated);
            request.Parameters.AddWithValue("@tags", this.Tags);
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
            request.CommandText = "UPDATE Hack SET nb_likes = @nb_likes, reported = @reported, reason_reported @reason_reported, last_updated = @last_updated";
            request.Parameters.AddWithValue("@nb_likes", this.nbLikes);
            request.Parameters.AddWithValue("@reported", this.reported);
            request.Parameters.AddWithValue("@reason_reported", this.reasonReported);
            request.Parameters.AddWithValue("@last_updated", this._lastUpdated);
            request.Parameters.AddWithValue("@tags", this.Tags);
            request.Prepare();

            request.ExecuteNonQuery();

            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
    public bool DeleteFromDB(MySqlCommand request){
        try
        {
            request.CommandText = "DELETE FROM Hack WHERE id_hack = @id";
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


}