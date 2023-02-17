using MySql.Data.MySqlClient;
public interface BaseUser{

    /// <summary>
    /// Create a comment, upload it to the BD and return it
    /// </summary>
    /// <param name="post">The hack where the comment has been posted</param>
    /// <param name="description">The content of the comment</param>
    /// <returns>The comment created</returns>
    Comment postComment(int postId, MySqlCommand request, string description = "");
    
    /// <summary>
    /// Create a hack, upload it to the BD and returns it 
    /// </summary>
    /// <param name="title">The title of the Hack</param>
    /// <param name="tags">tag to help searching through hacks</param>
    /// <param name="description">the description of the hack</param>
    /// <returns>A Hack</returns>
    Hack PostHack(String title, String tags, String description, string category, MySqlCommand request);

    /// <summary>
    /// Set the hack to the favorite and update the DB
    /// </summary>
    /// <param name="post">the hack we wanted to keep</param>
    void setFavorite(Hack post, MySqlCommand request);

    /// <summary>
    /// Delete the hack from the user's favorite
    /// </summary>
    /// <param name="post">the hack to delete</param>
    public void unsetFavorite(Hack post, MySqlCommand request);

    /// <summary>
    /// Delete one of user's hack from the whole site
    /// </summary>
    /// <param name="post">the hack to delete</param>
    public void deleteOwn(Hack post, MySqlCommand request);

    /// <summary>
    /// Delete one of user's Comment from the whole site
    /// </summary>
    /// <param name="comment">the comment to delete</param>
    public void deleteOwn(Comment comment, MySqlCommand request);

}