using System.Text.Json.Serialization;
using MTCG.Server;

namespace MTCG.Models;

public class User {
    public int id { get; set; }
    [JsonPropertyName("Username")]
    public string username { get; set; }
    [JsonPropertyName("Password")]
    public string password { get; set; }
    private List<Card> deck { get; set; }
    public int coins { get; set; }
    public int elo { get; set; }
    public bool isAdmin { get; set; }
    public DateTime timeActive { get; set; }

    
    
    public void UpdateDeck(List<Card> deck) {
        this.deck = deck;
    }
    
    public Card ChooseRandomCard() {
        Random random = new Random();
        return deck[random.Next(deck.Count)];
    }
    
    public void AddCard(Card card)
    {
        deck.Add(card);
    }

    
    public void RemoveCard(Card card)
    {
        deck.Remove(card);
    }
    
    
    public IEnumerable<Card> GetDeck()
    {
        return deck;
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
    

    public void selectDeck() {
        //to do
    }

    public bool HasCards()
    {
        return deck.Any();
    }
}