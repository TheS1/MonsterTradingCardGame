using System.Text.Json.Serialization;
using MTCG.Server;

namespace MTCG.Models;

public class User {
    public int id { get; set; }
    [JsonPropertyName("Username")]
    public string username { get; set; }
    [JsonPropertyName("Password")]
    public string password { get; set; }
    private List<Card> stack;
    private List<Card> deck { get; set; }
    public int coins { get; set; }
    public int elo { get; set; }
    private DateTime timeActive { get; set; }



    
    public void setNewUserData() {
        timeActive = DateTime.Now;
        stack = new List<Card>();
        deck = new List<Card>();
        password = "";
        coins = 30;
        elo = 1000;
    }

    public void setUserData(int coins, int elo) {
        timeActive = DateTime.Now;
        password = "";
        this.coins = coins;
        this.elo = elo;

    }

    public string getUsername()
    {
        return username;
    }

    public string getPassword()
    {
        return password;
    }
    public bool sessionExpired() {
        TimeSpan timeDifference = DateTime.Now - timeActive;
        double totalMinutes = timeDifference.TotalMinutes;
        return totalMinutes > 90;
    }

    public void buyPack(StreamWriter writer) {
        if (coins >= 5) {
            stack.AddRange(CardPool.drawPack());
            coins -= 5;
            //to do: after implementing cards sendresponse of cards drawn
            string responseString = "Pack bought!";
            Server.Server.SendResponse(writer, responseString);
        }
        else {
            string responseString = "Not enough coins.";
            Server.Server.SendResponse(writer, responseString);
        }
    }

    public void selectDeck() {
        //to do
    }
}