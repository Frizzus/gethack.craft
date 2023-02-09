

public struct Hack{
    
    public string? title;
    private string[] _tags;
    private string? _img;
    public string description;
    public int nbLikes;
    public bool reported;
    public string? reasonReported;
    public BaseUser relatedUser;
    public string _hackType;


    public Hack(BaseUser user, string title = "default", string type = "build"){
        if (title == "default")
        {
            throw new Exception("title have to be specified");
        }

        this.title = title;
        this._tags = new string[0];
        this.description = "";
        this.nbLikes = 0;
        this.reported = false;
        this.reasonReported = null;
        this.relatedUser = user;
        this._hackType = type;
    }


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


}