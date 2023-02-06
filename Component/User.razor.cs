class User{
    public string username;
    public string password;
    private string _email;
    private string _profilePicture;
    public string description;
    public bool banned;
    private DateTime _banTime;


    //TODO : changed to private for safety
    private User(){}


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

        }
    }
    //TODO : changed to private for safety
    private string BanTime{
        get;set;
    }
    //TODO : changed to private for safety
    private string ProfilePicture{
        get;set;
    }

}