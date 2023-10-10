using System.Net;

namespace MTCG;

public class User
{
    private string username { get; }
    private string password;
    private List<Card> stack;
    private List<Card> deck { get; }
    private int coins;
    private int elo { get; }
    

    public User(string username) {
        this.username = username;
        stack = new List<Card>();
        deck = new List<Card>();
        coins = 20;
    }

    public void buyPack(StreamWriter writer) {
        if (coins >= 5) {
            stack.AddRange(CardPool.drawPack());
            coins -= 5;
            //to do: after implementing cards sendresponse of cards drawn
            string responseString = "Pack bought!";
            Server.SendResponse(writer, responseString);
        }
        else {
            string responseString = "Not enough coins.";
            Server.SendResponse(writer, responseString);
        }
    }

    public void selectDeck() {
        //to do
    }
}