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
    }


    public int id;
    public string username;
    public string _password = "";
    private string _email = "";
    private string? _profilePicture;
    public string? description;
    public bool? banned;
    private DateTime? _banTime;
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
    public string? BanTime{
        get;set;
    }
    private string? ProfilePicture{
        get;set;
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