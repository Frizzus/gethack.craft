using MySql.Data.MySqlClient;
public class User : BaseUser{

    public User(string password, string email, string username = ""){
        
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

    public User(string username, string password, MySqlCommand request){
        if (password == "")
        {
            throw new Exception("you have to insert a password");
        }

        this.username = username;
        this._password = password;

        request.CommandText = "SELECT * FROM User WHERE username = @username AND pwd = @password";
        request.Parameters.AddWithValue("@username", username);
        request.Parameters.AddWithValue("@password", password);
        MySqlDataReader reader = request.ExecuteReader();
        


        this.id = reader.GetInt32("user_id");
        this._email = reader.GetString("email");
        this.profilePicture = reader.GetString("profile_picture");
        this.description = reader.GetString("description");
        this.banned = reader.GetBoolean("ban");
        this.banTime = reader.GetDateTime("ban_time");
        this._lastUpdated = reader.GetDateTime("last_updated");


        if (username == "")
        {
            this.username = this._email;
        }

        // Load Loved hack
        request.Parameters.Clear();
        request.CommandText = "SELECT id_hack FROM User u INNER JOIN loved_hack l WHERE u.id_user = l.id_user AND l.id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        reader = request.ExecuteReader();

        this.hackLoved = new List<Hack>();

        while (reader.Read())
        {
            this.hackLoved.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load own hack

        request.Parameters.Clear();
        request.CommandText = "SELECT id_hack FROM Hack WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        reader = request.ExecuteReader();

        this.hackPosted = new List<Hack>();

        while (reader.Read())
        {
            this.hackPosted.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load Comments

        request.Parameters.Clear();
        request.CommandText = "SELECT id_comment FROM Comment WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        reader = request.ExecuteReader();

        this.personnalComment = new List<Comment>();

        while (reader.Read())
        {
            this.personnalComment.Add(new Comment(reader.GetInt32("id_hack"), request));
        }
    }

    public User(int id, MySqlCommand request){
        

        request.CommandText = "SELECT * FROM User WHERE user_id = @id";
        request.Parameters.AddWithValue("@id", id);
        MySqlDataReader reader = request.ExecuteReader();
        

        this.username = reader.GetString("username");
        this._password = reader.GetString("pwd");
        this.id = reader.GetInt32("user_id");
        this._email = reader.GetString("email");
        this.profilePicture = reader.GetString("profile_picture");
        this.description = reader.GetString("description");
        this.banned = reader.GetBoolean("ban");
        this.banTime = reader.GetDateTime("ban_time");
        this._lastUpdated = reader.GetDateTime("last_updated");


    if (this._password == "")
        {
            throw new Exception("you have to insert a password");
        }
    if (username == "")
        {
            this.username = this._email;
        }

        // Load Loved hack
        request.Parameters.Clear();
        request.CommandText = "SELECT id_hack FROM User u INNER JOIN loved_hack l WHERE u.id_user = l.id_user AND l.id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        reader = request.ExecuteReader();

        this.hackLoved = new List<Hack>();

        while (reader.Read())
        {
            this.hackLoved.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load own hack

        request.Parameters.Clear();
        request.CommandText = "SELECT id_hack FROM Hack WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        reader = request.ExecuteReader();

        this.hackPosted = new List<Hack>();

        while (reader.Read())
        {
            this.hackPosted.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load Comments

        request.Parameters.Clear();
        request.CommandText = "SELECT id_comment FROM Comment WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        reader = request.ExecuteReader();

        this.personnalComment = new List<Comment>();

        while (reader.Read())
        {
            this.personnalComment.Add(new Comment(reader.GetInt32("id_hack"), request));
        }


    }


    public int id;
    public string username;
    public string _password = "";
    private string _email = "";
    private string? profilePicture;
    public string? description;
    public bool? banned;
    private DateTime? banTime;
    public List<Hack> hackLoved;
    public List<Hack> hackPosted;
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
        // Create the record in the DB
        // Link it to the user
        // Link it to the hack
        Comment comment = new Comment(this, post,description);
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

}