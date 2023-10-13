using System.Net;

namespace MTCG;

public class User {
    private string username { get; set; }
    private string password { get; set; }
    private List<Card> stack;
    private List<Card> deck { get; }
    private int coins;
    private int elo { get; }
    private DateTime sessionTime { get; }
    


    public User() {
        sessionTime = DateTime.Now;
        stack = new List<Card>();
        deck = new List<Card>();
        coins = 20;
    }

    public bool sessionExpired(DateTime timeNow) {
        TimeSpan timeDifference = DateTime.Now - sessionTime;
        double totalMinutes = timeDifference.TotalMinutes;
        return totalMinutes < 90;
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