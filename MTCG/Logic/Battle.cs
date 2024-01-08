using MTCG.Models;
using MTCG.DAL;
public class Battle
{
    private User _player1;
    private User _player2;
    private float _result;
    private DateTime _battleDate;
    private int _maxRounds;
    private string _log;
    public string Winstring;
    private int _eloChange;
    public Battle(User player1, User player2, int maxRounds = 100)
    {
        _player1 = player1;
        _player2 = player2;
        Console.WriteLine(_player1.Username);
        Console.WriteLine(_player2.Username);
        Console.WriteLine(_player1.Id);
        Console.WriteLine(_player2.Id);
        _maxRounds = maxRounds;
    }

    public void StartBattle()
    {
        int round = 1;
        _battleDate = DateTime.Now;
        while (round <= _maxRounds) {
            if (!_player1.HasCards()) {
                _result = 0;
                Winstring = "p2 wins";
                _log += _player2.Username + " wins!\n";
                _eloChange = CalcElo(_player1.Elo, _player2.Elo, _result);
                DbManager.HandleBattleResult(_player1, _player2, _result, _battleDate, round, _eloChange);
                return; 
            } 
            if (!_player2.HasCards()) {
                _result = 1;
                Winstring = "p1 wins";
                _log += _player1.Username + " wins!\n";
                _eloChange = CalcElo(_player1.Elo, _player2.Elo, _result);
                DbManager.HandleBattleResult(_player1, _player2, _result, _battleDate, round, _eloChange);
                return; 
            } 

            Card cardPlayer1 = _player1.ChooseRandomCard();
            Card cardPlayer2 = _player2.ChooseRandomCard();

            double damageUser1 = cardPlayer1.CalculateDamage(cardPlayer2);
            double damageUser2 = cardPlayer2.CalculateDamage(cardPlayer1);

            Console.WriteLine($"{_player1.Username}'s {cardPlayer1.Name} attacks with {damageUser1} damage.");
            Console.WriteLine($"{_player2.Username}'s {cardPlayer2.Name} attacks with {damageUser2} damage.");
            _log += "Round " + round + ": " + _player1.Username + ": " + cardPlayer1.Name + " " + cardPlayer1.Damage +
                   " vs " +  _player2.Username + ": " + cardPlayer2.Name + " " + cardPlayer2.Damage + " => " + damageUser1 + " vs " + damageUser2;
            
            if (damageUser1 > damageUser2)
            {
                _log += " => " +_player1.Username + "'s "+ cardPlayer1.Name + " wins\n";
                Console.WriteLine($"{_player1.Username} wins the round!");
                _player1.AddCard(cardPlayer2);
                _player2.RemoveCard(cardPlayer2);
            }
            else if (damageUser2 > damageUser1)
            {
                _log += " => " + _player2.Username + "'s "+ cardPlayer2.Name + " wins\n";
                Console.WriteLine($"{_player2.Username} wins the round!");
                _player2.AddCard(cardPlayer1);
                _player1.RemoveCard(cardPlayer1);
            }
            else
            {
                _log += " Draw (no action)\n";
                Console.WriteLine("It's a draw! No cards are moved.");
            }

            Console.WriteLine($"{_player1.Username}'s Deck: {string.Join(", ", _player1.GetDeck())}");
            Console.WriteLine($"{_player2.Username}'s Deck: {string.Join(", ", _player2.GetDeck())}");
            Console.WriteLine();
    
            round++;
        }
    
        _log += "Draw after 100 rounds";
        Winstring = "draw";
        _result = 0.5f;
        _eloChange = CalcElo(_player1.Elo, _player2.Elo, _result);
        DbManager.HandleBattleResult(_player1, _player2, _result, _battleDate, round, _eloChange);
    }


    private int CalcElo(int eloP1, int eloP2, float result) {
        double e = 1 / (1 + Math.Pow(10, (eloP2 - eloP1) / 400));
        int eloChange = (int)Math.Round(20 * (result - e));
        return eloChange;
    }
    public string GetLog()
    {
        return _log;
    }
}