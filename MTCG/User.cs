namespace MTCG;

public class User
{
    private string username { get; }
    private string password;
    private List<Card> stack;
    private List<Card> deck { get; }
    private int coins;
    private int elo { get; }
    
    public User(){}
    public User(string username, string password) {
        this.username = username;
        this.password = password;
        stack = new List<Card>();
        deck = new List<Card>();
        coins = 20;
    }

    public void buyPack() {
        if (coins >= 5) {
            stack.AddRange(CardPool.drawPack());
            coins -= 5;
        }
        else {
            Console.WriteLine("Not enough coins.");
        }
    }

    public void selectDeck() {
        //to do
    }
}