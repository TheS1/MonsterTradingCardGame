using System.Text.Json.Serialization;

namespace MTCG.Models;

public class User {
    public User() {
        Username = "";
        Password = "";
        Deck = new List<Card>();
    }

    public int Id { get; set; }
    [JsonPropertyName("Username")]
    public string Username { get; set; }
    [JsonPropertyName("Password")]
    public string Password { get; set; }
    private List<Card> Deck { get; set; }
    public int Coins { get; set; }
    public int Elo { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime TimeActive { get; set; }

    
    
    public void UpdateDeck(List<Card> deck) {
        this.Deck = deck;
    }
    
    public Card ChooseRandomCard() {
        Random random = new Random();
        return Deck[random.Next(Deck.Count)];
    }
    
    public void AddCard(Card card)
    {
        Deck.Add(card);
    }

    
    public void RemoveCard(Card card)
    {
        Deck.Remove(card);
    }
    
    
    public IEnumerable<Card> GetDeck()
    {
        return Deck;
    }
    
    public string GetUsername()
    {
        return Username;
    }

    public string GetPassword()
    {
        return Password;
    }
    public bool SessionExpired() {
        TimeSpan timeDifference = DateTime.Now - TimeActive;
        double totalMinutes = timeDifference.TotalMinutes;
        return totalMinutes > 90;
    }
    
    

    public bool HasCards()
    {
        return Deck.Any();
    }
}