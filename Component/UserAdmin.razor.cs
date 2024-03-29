using MySql.Data.MySqlClient;

class UserAdmin : BaseUser,DBObject
{

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

    public UserAdmin(string password, string email, MySqlCommand request, string username = ""){
        
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

        if (!this.ConstructForDB(request))
        {
            throw new Exception("The insertion of the object in the Database has failed");
        }
        // Get the id and the SQL server current date the DB give with AUTO-INCREMENT and then update the object with it
            
        // Getting the database id and date based on the most recent date from this.username
        request.CommandText = "SELECT id_user, last_updated FROM Comment WHERE last_updated = (SELECT MAX(last_updated) FROM User WHERE username = @username AND pwd = @password)";
        request.Parameters.AddWithValue("@username", this.username);
        request.Parameters.AddWithValue("@password", this._password);
        request.Prepare();
        MySqlDataReader data = request.ExecuteReader();
        
        this.id = data.GetInt32(0);
        this._lastUpdated = data.GetDateTime(1);

    }

    public UserAdmin(string username, string password, MySqlCommand request){
        if (password == "")
        {
            throw new Exception("you have to insert a password");
        }

        this.username = username;
        this._password = password;

        request.CommandText = "SELECT * FROM User WHERE username = @username AND pwd = @password";
        request.Parameters.AddWithValue("@username", username);
        request.Parameters.AddWithValue("@password", password);
        request.Prepare();
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
        request.Prepare();
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
        request.Prepare();
        reader = request.ExecuteReader();

        this.hackPosted = new List<Hack>();

        while (reader.Read())
        {
            this.hackPosted.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load toInspectHack

        request.Parameters.Clear();
        request.CommandText = "SELECT id_hack FROM User u INNER JOIN to_inspect_hack i WHERE u.id_user = i.id_user AND i.id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        request.Prepare();
        reader = request.ExecuteReader();

        this.hackToInspect = new List<Hack>();

        while (reader.Read())
        {
            this.hackToInspect.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load Comments

        request.Parameters.Clear();
        request.CommandText = "SELECT id_comment FROM Comment WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        request.Prepare();
        reader = request.ExecuteReader();

        this.personnalComment = new List<Comment>();

        while (reader.Read())
        {
            this.personnalComment.Add(new Comment(reader.GetInt32("id_hack"), request));
        }
    }

    public UserAdmin(int id, MySqlCommand request){
        

        request.CommandText = "SELECT * FROM User WHERE user_id = @id";
        request.Parameters.AddWithValue("@id", id);
        request.Prepare();
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
        request.Prepare();
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
        request.Prepare();
        reader = request.ExecuteReader();

        this.hackPosted = new List<Hack>();

        while (reader.Read())
        {
            this.hackPosted.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load toInspectHack

        request.Parameters.Clear();
        request.CommandText = "SELECT id_hack FROM User u INNER JOIN to_inspect_hack i WHERE u.id_user = i.id_user AND i.id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        request.Prepare();
        reader = request.ExecuteReader();

        this.hackToInspect = new List<Hack>();

        while (reader.Read())
        {
            this.hackToInspect.Add(new Hack(reader.GetInt32("id_hack"), request));
        }

        // Load Comments

        request.Parameters.Clear();
        request.CommandText = "SELECT id_comment FROM Comment WHERE id_user = @id";
        request.Parameters.AddWithValue("@id", id);
        request.Prepare();
        reader = request.ExecuteReader();

        this.personnalComment = new List<Comment>();

        while (reader.Read())
        {
            this.personnalComment.Add(new Comment(reader.GetInt32("id_hack"), request));
        }


    }



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


    public Comment postComment(int postId, MySqlCommand request, string description = ""){
        // Post the comment to the DB
        Comment comment = new Comment(this.id, postId, description, request);
        this.personnalComment.Add(comment);
        this.UpdateToDB(request);
        return comment;
    }

    public Hack PostHack(String title, String tags, String description, string category, MySqlCommand request){
        // Upload the Hack to the BD
        Hack h = new Hack(this.id, title: title, type: category, tags: tags, description: description, request: request);
        this.hackPosted.Add(h);
        this.UpdateToDB(request);
        return h;
    }

    public void setFavorite(Hack post, MySqlCommand request){
        this.hackLoved.Add(post);
        // set the hack to loved in the DB
        request.Parameters.Clear();
        request.CommandText = "INSERT INTO loved_hack(id_hack, id_user) VALUES(@id_hack, @id_user)";
        request.Parameters.AddWithValue("@id_hack", post.id);
        request.Parameters.AddWithValue("@id_user", this.id);
        request.Prepare();
        request.ExecuteNonQuery();
    }

    public void unsetFavorite(Hack post, MySqlCommand request){
        int indexToSupr = this.hackLoved.FindIndex(0, x => (x.id == post.id));
        this.hackLoved.RemoveAt(indexToSupr);
        // delete the record in the DB
        request.Parameters.Clear();
        request.CommandText = "DELETE FROM loved_hack WHERE id_hack = @id_hack AND id_user = @id_user";
        request.Parameters.AddWithValue("@id_hack", post.id);
        request.Parameters.AddWithValue("@id_user", this.id);
        request.Prepare();
        request.ExecuteNonQuery();
    }

    public void deleteOwn(Hack post, MySqlCommand request){
        int indexToSupr = this.hackPosted.FindIndex(0, x => (x.id == post.id));
        this.hackPosted.RemoveAt(indexToSupr);
        // delete the record in the DB
        request.Parameters.Clear();
        request.CommandText = "DELETE FROM Hack WHERE id_hack = @id_hack AND id_user = @id_user";
        request.Parameters.AddWithValue("@id_hack", post.id);
        request.Parameters.AddWithValue("@id_user", this.id);
        request.Prepare();
        request.ExecuteNonQuery();
    }

    public void deleteOwn(Comment comment, MySqlCommand request){
        int indexToSupr = this.personnalComment.FindIndex(0, x => (x.id == comment.id));
        this.personnalComment.RemoveAt(indexToSupr);
        // delete the record in the DB
        request.Parameters.Clear();
        request.CommandText = "DELETE FROM Comment WHERE id_comment = @id_comment AND id_user = @id_user";
        request.Parameters.AddWithValue("@id_comment", comment.id);
        request.Parameters.AddWithValue("@id_user", this.id);
        request.Prepare();
        request.ExecuteNonQuery();
    }
    

    /// <summary>
    /// An admin method to delete any comment safely
    /// </summary>
    /// <param name="comment">The comment targeted to be deleted</param>
    public void adminDelete(Comment comment, MySqlCommand request){
        request.Parameters.Clear();
        request.CommandText = "DELETE FROM Comment WHERE id_comment = @id_comment";
        request.Parameters.AddWithValue("@id_comment", comment.id);
        request.Prepare();
        request.ExecuteNonQuery();
    }

    /// <summary>
    /// An admin method to delete any hack safely 
    /// </summary>
    /// <param name="post">The post targeted to be deleted</param>
    public void adminDelete(Hack post, MySqlCommand request){
        request.Parameters.Clear();
        request.CommandText = "DELETE FROM Hack WHERE id_hack = @id_hack";
        request.Parameters.AddWithValue("@id_hack", post.id);
        request.Prepare();
        request.ExecuteNonQuery();
    }

    /// <summary>
    /// An admin method to ban a user for a certain amount of time
    /// </summary>
    /// <param name="user">The future banned user</param>
    /// <param name="untilDate">The Date until the user will be banned</param>
    public void adminBan(User user, DateTime untilDate, MySqlCommand request){
        user.banned = true;
        user.banTime = untilDate;
        //Update in DB
        user.UpdateToDB(request);
    }

    public bool ConstructForDB(MySqlCommand request){
        try
        {
            request.CommandText = "INSERT INTO User(username, pwd, email, profile_picture, description, ban, ban_time, is_admin, last_updated) VALUES(@username, @pwd, @email, @profile_picture, @description, @ban, @ban_time, @is_admin, @last_updated)";
            request.Parameters.AddWithValue("@username", this.username);
            request.Parameters.AddWithValue("@pwd", this._password);
            request.Parameters.AddWithValue("@email", this._email);
            request.Parameters.AddWithValue("@profile_picture", this.profilePicture);
            request.Parameters.AddWithValue("@description", this.description);
            request.Parameters.AddWithValue("@ban", this.banned);
            request.Parameters.AddWithValue("@ban_time", this.banTime);
            request.Parameters.AddWithValue("@last_updated", this._lastUpdated);
            request.Parameters.AddWithValue("@is_admin", true);
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
            request.CommandText = "UPDATE User SET username = @username, pwd = @pwd, email = @email, profile_picture = @profile_picture, description = @description, ban = @ban, ban_time = @ban_time, last_updated = @last_updated";
            request.Parameters.AddWithValue("@username", this.username);
            request.Parameters.AddWithValue("@pwd", this._password);
            request.Parameters.AddWithValue("@email", this._email);
            request.Parameters.AddWithValue("@profile_picture", this.profilePicture);
            request.Parameters.AddWithValue("@description", this.description);
            request.Parameters.AddWithValue("@ban", this.banned);
            request.Parameters.AddWithValue("@ban_time", this.banTime);
            request.Parameters.AddWithValue("@last_updated", this._lastUpdated);
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
            request.CommandText = "DELETE FROM User WHERE id_user = @id";
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