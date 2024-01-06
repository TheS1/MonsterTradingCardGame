using Npgsql;
using MTCG.Models;
namespace MTCG.DAL;


public static class DbManager
{

    private static NpgsqlConnection conn;
    private static UserDbRepo userRepository;
    private static CardDbRepo cardRepository;
    public static void connect() {
        string connString = "Host=207.180.211.57;Port=5000;Username=postgres ;Password=admin;Database=MTCG";
        // Create a new connection
        conn = new NpgsqlConnection(connString);
        conn.Open();
        userRepository = new UserDbRepo(conn);
        cardRepository = new CardDbRepo(conn);
    }

    public static bool loginUser(User? user) {
        if (userRepository.LoginUser(user.getUsername(), user.getPassword())) {
            userRepository.SetUserData(user);
            user.isAdmin = userRepository.isAdmin(user.getUsername());
            return true;
        }
        return false;
    }
    
    public static bool registerUser(User user) {
        bool usernameExists = userRepository.UsernameExists(user.getUsername());

        if (!usernameExists) // if username not taken add user
        {
            userRepository.Add(user);
            userRepository.SetUserData(user);
            return true;
        }
        return false;
    }

    public static int checkSet(string set) {
        return cardRepository.CardSetExists(set);
    }

    public static bool checkUserEnoughCoins(int userCoins, int setID) {
        return userCoins >= cardRepository.GetCardSetPrice(setID);
    }

    public static List<string> buyPack(int userID, int setID) {
        List<string> CardsDrawn = cardRepository.drawPack(setID);
        foreach (var card in CardsDrawn) {
            userRepository.AddCardsToUser(userID, card);
        }
        userRepository.SubtractCoins(userID, cardRepository.GetCardSetPrice(setID));

        return CardsDrawn;
    }

    public static string getAllUserCards(int userID)
    {
        string responseString = "";
        List<string> cards = userRepository.getCardsOfUser(userID);
        if (cards.Any())
        {
            foreach (var cardID in cards)
            {
                responseString += cardRepository.getCardInfoByID(cardID) + "\n";
            }

        }
        else
        {
            responseString = "No Cards acquired";
        }
        

        return responseString;
    }


    public static string getUserDeck(int userID) {
        string responseString = "";
        Console.WriteLine(userID);
        List<string> deck = userRepository.getDeckOfUser(userID);
        if (deck.Any()) {
            foreach (var cardID in deck) {
                responseString += cardRepository.getCardInfoByID(cardID) + "\n";
            }

        }
        else {
            return "Deck not configured";
        }

        return responseString;
    }

    public static string getUserProfile(int userID)
    {
        return "\nProfile: " + userRepository.getUserProfile(userID);
    }

    public static bool updateDeck(User user, List<string> deck) {
        foreach (var cardID in deck) {
            if (!userRepository.UserOwnsCard(user.id, cardID)) {
                return false;
            }
        }
        List<Card> cardDeck = userRepository.updateDeck(user.id, deck);
        user.UpdateDeck(cardDeck);
        return true;
    }
    
    public static string getAllCards(List<string> cards)
    {
        string responseString = "";

        foreach (var cardID in cards)
        {
            responseString += cardRepository.getCardInfoByID(cardID) + "\n";
        }

        return responseString;
    }
    public static bool addCards(string id, string cardName, double cardDamage, string cardType, string elementType, string set)
    {
        int cardSetId;
        
        if (set == "") {
            cardSetId = 1;
        } else {
            cardSetId = cardRepository.CardSetExists(set);
        };

        if (cardSetId == -99) {
            return false;
        }
        return cardRepository.AddCard(id, cardName, cardDamage, cardType, elementType, cardSetId);
    }

    public static bool updateProfile(int userID, string username, string bio, string image) {
        if (!userRepository.UsernameExistsBesidesSelf(userID, username)) {
            userRepository.updateProfile(userID, username, bio, image);
            return true;
        }

        return false;
    }

    public static string getScoreboard() {
        string responseString = "Scoredboard\n--------------------------------------------------------------";
        List<string> users = userRepository.getScoreboard();

        for (int i = 0; i < users.Count(); i++)
        {
            responseString += "\n " + (i + 1) + ": " + users[i];
        }
        responseString += "\n--------------------------------------------------------------";
        return responseString;
    }
}
