using MySql.Data.MySqlClient;

class UserAdmin : BaseUser,DBObject
{

    public UserAdmin(string password, string email, string username = ""){
        
        if (password == "")
        {
            throw new Exception("you have to insert a password");
        }
        if (username == "")
        {
            this.username = email;
        }
        
        this.username = username;
        this.Password = password;
        this.Email = email;
        this.hackLoved = new List<Hack>();
        this.hackPosted = new List<Hack>();
        this.personnalComment = new List<Comment>();
        this.hackToInspect = new List<Hack>();

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
            
        // Getting the database id and date based on the most recent date from this.username
        request.CommandText = "SELECT id_user, last_updated FROM Comment WHERE last_updated = (SELECT MAX(last_updated) FROM User WHERE username = @username AND pwd = @password)";
        request.Parameters.AddWithValue("@username", this.username);
        request.Parameters.AddWithValue("@password", this._password);

        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);

        connection.Close();
    }

    public UserAdmin(int id, string username, string password, string email, string profilePicture, string description, bool banned, DateTime banTime, DateTime lastUpdated){
        if (password == "")
        {
            throw new Exception("you have to insert a password");
        }
        if (username == "")
        {
            this.username = email;
        }

        this.id = id;
        this.username = username;
        this._password = password;
        this._email = email;
        this.profilePicture = profilePicture;
        this.description = description;
        this.banned = banned;
        this.banTime = banTime;
        this._lastUpdated = lastUpdated;

        // fetching hacks and comments

        IDictionary<string,string> dotEnv = FrizzusUtils.getEnvArray(@"\.env");
        MySqlConnection connection = new MySqlConnection($"server={dotEnv["DB_HOST"]};userid={dotEnv["DB_USER"]};password={dotEnv["DB_PASSWORD"]};database={dotEnv["DB_DATABASE"]};");
        connection.Open();

        MySqlCommand request = new MySqlCommand();
        request.Connection = connection;

        // Fetching loved hacks
        request.CommandText = "SELECT u.id_user, h.id_hack, h.title, h.img_url, h.description, h.nb_likes, h.reported, h.reason_reported, h.hack_type, h.last_updated, h.tags FROM user u INNER JOIN loved_hack l ON l.id_user = u.id_user INNER JOIN hack h ON l.id_hack = h.id_hack";
        MySqlDataReader reader = request.ExecuteReader();

        while (reader.Read())
        {
            // Add a VARCHAR tag attribute to Hack tab
            // NEED TO REWORK THE WAY OF GETTING OTHER OBJECT (CURRENT NON LISIBLE)
            // IDEA : MAKING A USER CONSTRUCT WITH ONLY ID AND WITH THE ID GET ALL THE ATTRIBUTES
            // REQUEST IDEA : REPLACE THE INNER JOIN WITH A id_user = (SELECT...) 
            this.hackLoved.Append<Hack>(new Hack(id: reader.GetInt32("id_hack"), 
            relatedUser: reader.GetInt32("id_user"),
            title: reader.GetString("title"),
            tags: reader.GetString("tags"),
            imgLink: reader.GetString("img_url"),
            description: reader.GetString("description"),
            nbLikes: reader.GetInt32("nb_likes"),
            reported: reader.GetBoolean("reported"),
            reasonReported: reader.GetString("reason_reported"),
            hackType: reader.GetString("hack_type"),
            lastUpdated: reader.GetDateTime("last_updated")));
        }
    }

    public int id;
    public string username;
    public string _password = "";
    private string _email = "";
    private string? profilePicture = "placeholder.svg";
    public string? description;
    public bool? banned;
    public DateTime? banTime;
    public List<Hack> hackLoved;
    public List<Hack> hackPosted;
    public List<Hack> hackToInspect;
    public List<Comment> personnalComment;
    private DateTime _lastUpdated;



    // Property

    public DateTime LastUpdated {get;}

    /// <summary>
    /// Property for a valid email :
    /// <list type="bullet">
    ///     <item>
    ///         <description> does it contain only one "@" </description>
    ///     </item>
    ///     <item>
    ///         <description> does it contain a "." after "@"</description>
    ///     </item>
    ///     <item>
    ///         <description> does the "." is separate from "@" ( '@xxx.io' is valid, '@.io' is not )</description>
    ///     </item>
    ///     <item>
    ///         <description> does the "." is separate from the end ( '@xxx.com' is valid, '@xxx.' is not )</description>
    ///     </item>
    /// </list>
    /// </summary>
    /// <value></value>
    public string Email {
        get => this._email;
        set{
            /*Split the Email
            if there is only 1 index => no "@" => invalid
            if there is 3 or more index => several "@" => invalid
            if there is 2 index => only one "@" => valid => next step
            */

            /*Take the local-part <==> First part, before the "@"
                foreach the string
                    check if the char code is in the ascii table
                    if the char is a "."
                        if firt => invalid
                        if last => invalid
                        if not last and next char is . => invalid
            */

            /*Take the domain <==> Last part, after the "@"
                split with "." to see how many point there is (you should have one "." and so 2 index)
                foreach the string
                    check if the char code is in the ascii table (latin letter and digit + "-")
                    if the char is a "."
                        if firt => invalid
                        if last => invalid
                        if not last and next char is . => invalid
                    if the char is a "-"
                        if first => invalid
                        if last => invalid
            */
            this._email = value;
        }
    }

    private string Password{
        get => this._password;
        set {
            foreach (var ch in new char[] {'$', '%', '*', '#', '&', '€', '_', '-'})
            {
                if (!value.Contains(ch)){
                    throw new Exception("Your password does not contain any special characters");
                }
            }

            foreach (var ch in new char[] {'1', '2', '3', '4', '5', '6', '7', '8', '9'})
            {
                if (!value.Contains(ch)){
                    throw new Exception("Your password does not contain any number");
                }
            }

            this._password = value;
        }
    }


    // Methods


    public Comment postComment(Hack post, string description = ""){
        // Post the comment to the DB
        Comment comment = new Comment(this, post, description);
        this.personnalComment.Add(comment);
        return comment;

    }

    public Hack PostHack(String title, String tags, String description, string category){
        // Upload the Hack to the BD

        Hack h = new Hack(this, title: title, type: category, tags: tags, description: description);

        this.hackPosted.Add(h);

        return h;
    }

    public void setFavorite(Hack post){
        this.hackLoved.Add(post);

        // set the hack to loved in the DB
    }

    public void unsetFavorite(Hack post){
        // delete the record in the DB

        int indexToSupr = this.hackLoved.FindIndex(0, x => (x.id == post.id));

        this.hackLoved.RemoveAt(indexToSupr);
    }

    public void deleteOwn(Hack post){
        // delete the record in the DB

        int indexToSupr = this.hackPosted.FindIndex(0, x => (x.id == post.id));

        this.hackPosted.RemoveAt(indexToSupr);
    }

    public void deleteOwn(Comment comment){
        // delete the record in the DB

        int indexToSupr = this.personnalComment.FindIndex(0, x => (x.id == comment.id));

        this.personnalComment.RemoveAt(indexToSupr);
    }

    /// <summary>
    /// An admin method to delete any comment safely
    /// </summary>
    /// <param name="comment">The comment targeted to be deleted</param>
    public void adminDelete(Comment comment){
        comment.relatedUser.deleteOwn(comment);
    }

    /// <summary>
    /// An admin method to delete any hack safely 
    /// </summary>
    /// <param name="post">The post targeted to be deleted</param>
    public void adminDelete(Hack post){
        post.relatedUser.deleteOwn(post);
    }

    /// <summary>
    /// An admin method to ban a user for a certain amount of time
    /// </summary>
    /// <param name="user">The future banned user</param>
    /// <param name="untilDate">The Date until the user will be banned</param>
    public void adminBan(User user, string untilDate){
        //Update in DB

        user.banned = true;
        user.BanTime = untilDate;
    }

    public bool ConstructForDB(){return false;}
    public bool UpdateToDB(){return false;}
    public bool DeleteFromDB(){return false;}
}