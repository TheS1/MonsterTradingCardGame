using System.Diagnostics;
using Npgsql;
using MTCG.Models;
namespace MTCG.DAL;


public static class DbManager
{

    private static NpgsqlConnection _conn = null!;
    private static UserDbRepo _userRepository = null!;
    private static CardDbRepo _cardRepository = null!;
    private static TradingDbRepo _tradeRepository = null!;
    public static void Connect() {
        string connString = "Host=207.180.211.57;Port=5000;Username=postgres ;Password=admin;Database=MTCG";
        // Create a new connection
        _conn = new NpgsqlConnection(connString);
        _conn.Open();
        _userRepository = new UserDbRepo(_conn);
        _cardRepository = new CardDbRepo(_conn);
        _tradeRepository = new TradingDbRepo(_conn);
    }

    public static bool LoginUser(User user) {
        if (_userRepository.LoginUser(user.GetUsername(), user.GetPassword())) {
            _userRepository.SetUserData(user);
            user.IsAdmin = _userRepository.IsAdmin(user.GetUsername());
            return true;
        }
        return false;
    }
    
    public static bool RegisterUser(User user) {
        bool usernameExists = _userRepository.UsernameExists(user.GetUsername());

        if (!usernameExists) // if username not taken add user
        {
            _userRepository.Add(user);
            _userRepository.SetUserData(user);
            return true;
        }
        return false;
    }

    public static int CheckSet(string set) {
        return _cardRepository.CardSetExists(set);
    }

    public static bool CheckUserEnoughCoins(int userCoins, int setId) {
        return userCoins >= _cardRepository.GetCardSetPrice(setId);
    }

    public static List<string> BuyPack(int userId, int setId) {
        List<string> cardsDrawn = _cardRepository.DrawPack(setId);
        if (cardsDrawn.Count == 5) {
            foreach (var card in cardsDrawn)
            {
                _userRepository.AddCardsToUser(userId, card);
            }

            _userRepository.SubtractCoins(userId, _cardRepository.GetCardSetPrice(setId));
        }

        return cardsDrawn;
    }

    public static string GetAllUserCards(int userId)
    {
        string responseString = "";
        List<string> cards = _userRepository.GetCardsOfUser(userId);
        if (cards.Any())
        {
            foreach (var cardId in cards)
            {
                responseString += _cardRepository.GetCardInfoById(cardId) + "\n";
            }

        }
        else
        {
            responseString = "No Cards acquired";
        }
        

        return responseString;
    }


    public static string GetUserDeck(int userId) {
        string responseString = "";
        Console.WriteLine(userId);
        List<string> deck = _userRepository.GetDeckOfUser(userId);
        if (deck.Any()) {
            foreach (var cardId in deck) {
                responseString += _cardRepository.GetCardInfoById(cardId) + "\n";
            }

        }
        else {
            return "Deck not configured";
        }

        return responseString;
    }

    public static string GetUserProfile(int userId)
    {
        return "\nProfile: " + _userRepository.GetUserProfile(userId);
    }

    public static bool UpdateDeck(User user, List<string> deck) {
        foreach (var cardId in deck) {
            if (!_userRepository.UserOwnsCard(user.Id, cardId)) {
                return false;
            }
        }
        List<Card> cardDeck = _userRepository.UpdateDeck(user.Id, deck);
        user.UpdateDeck(cardDeck);
        return true;
    }
    
    public static string GetAllCards(List<string> cards)
    {
        string responseString = "";

        foreach (var cardId in cards)
        {
            responseString += _cardRepository.GetCardInfoById(cardId) + "\n";
        }

        return responseString;
    }
    public static bool AddCards(string id, string cardName, double cardDamage, string cardType, string elementType, string set)
    {
        int cardSetId;
        
        cardSetId = set == "" ? 1 : _cardRepository.CardSetExists(set);

        if (cardSetId == -99) {
            return false;
        }
        return _cardRepository.AddCard(id, cardName, cardDamage, cardType, elementType, cardSetId);
    }

    public static bool UpdateProfile(int userId, string username, string bio, string image) {
        if (!_userRepository.UsernameExistsBesidesSelf(userId, username)) {
            _userRepository.UpdateProfile(userId, username, bio, image);
            return true;
        }

        return false;
    }

    public static void HandleBattleResult(User player1, User player2, float result, DateTime date, int turns, int eloChange) {
        const double tolerance = 1e-10;
        _userRepository.AddBattle(player1.Id, player2.Id, result, date, turns);
        _userRepository.UpdateElo(player1.Id, eloChange, Math.Abs(result - 1) < tolerance ? 1 : 0, Math.Abs(result - 1) < tolerance ? 0 : 1);
        _userRepository.UpdateElo(player2.Id, -eloChange, Math.Abs(result - 1) < tolerance ? 0 : 1, Math.Abs(result - 1) < tolerance ? 1 : 0);
        _userRepository.SetUserData(player1);
        _userRepository.SetUserData(player2);
    }

    public static string GetScoreboard() {
        string responseString = "Scoreboard\n--------------------------------------------------------------";
        List<string> users = _userRepository.GetScoreboard();

        for (int i = 0; i < users.Count(); i++)
        {
            responseString += "\n " + (i + 1) + ": " + users[i];
        }
        responseString += "\n--------------------------------------------------------------";
        return responseString;
    }

    public static string GetTradedeals()
    {
        string responseString = "Trade Deals\n--------------------------------------------------------------";
        List<string> tradedeals = _tradeRepository.GetTradedeals();

        for (int i = 0; i < tradedeals.Count(); i++)
        {
            responseString += "\n " + (i + 1) + ": " + tradedeals[i];
        }
        responseString += "\n--------------------------------------------------------------";
        return responseString;
    }

    public static bool PostTradeOffer(string cardId, int userId, string desiredType, double minimumDamage)
    {
        if (!_userRepository.UserOwnsCard(userId, cardId))
        {
            return false;
        }

        if (_tradeRepository.CardAlreadyPosted(cardId))
        {
            return false;
        }
        _tradeRepository.PostTrade(cardId, userId, desiredType, minimumDamage);
        return true;
    }

    public static string TryTrading(int userId, int tradeId, string cardId)
    {
        if (!_userRepository.UserOwnsCard(userId, cardId))
        {
            return "You dont own this card";
        }
        if (!_tradeRepository.OfferIdExists(tradeId))
        {
            return "Offer Id doesnt exist";
        }
        if (_tradeRepository.ThisUserPostedTradeOffer(userId, tradeId))
        {
            _tradeRepository.DeleteOffer(tradeId);
            return "Deleted your offer!";
        }

        if (!_tradeRepository.TradingRequirementsMet(tradeId, cardId))
        {
            return "Trading Requirements not met!";
        }

        _tradeRepository.completeTrade(userId, tradeId, cardId);
        return "Trade successfull!";
    }
}
