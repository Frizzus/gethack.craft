
public struct Comment{

    /// <summary>
    /// The Comment Struct accept only a content as the number of like when posting should always be 0
    /// </summary>
    /// <param name="content">The content inside the comment</param>
    public Comment(String content = "Placeholder"){
        this.content = content;
        this.nbLikes = 0;
    }

    public String content;
    public int nbLikes;
}