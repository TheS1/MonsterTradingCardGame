namespace MTCG;

public static class Server
{
    private static List<User> users;

    public static User register(string username, string password) {
        // to do - save password on server
        return new User(username, password);
    }
    
    // returning an user instance
    public static User login(string username, string password) {
        // to do - check password
        // return users out of list of users
        return new User(username, password);
    }
    
    
}