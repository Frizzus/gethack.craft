
interface BaseUser{

    // Add Comment to database and to a hack (meant to be the current post) to avoid making another query
    Comment PostComment(String content, Hack post);

    // Add hack to the database and to the server TODO : handling images
    Hack PostHack(String title, String tags, String description);

}