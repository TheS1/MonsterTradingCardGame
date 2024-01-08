using MTCG.Models;
namespace MTCG.Logic;

public class BattleTest {
    private User _player1;
    private User _player2;
    private string _result;
    private int _maxRounds;
    public BattleTest(User player1, User player2, int maxRounds = 100) {
        _player1 = player1;
        _player2 = player2;
        _result = "";
        _maxRounds = maxRounds;
    }

    public void StartBattle()
    {
        int round = 1;
        while (round <= _maxRounds) {
            if (!_player1.HasCards()) {
                _result = "p2 wins";
                return;
            } 
            if (!_player2.HasCards()) {
                _result = "p1 wins";
                return;
            } 

            Card cardPlayer1 = _player1.ChooseRandomCard();
            Card cardPlayer2 = _player2.ChooseRandomCard();

            double damageUser1 = cardPlayer1.CalculateDamage(cardPlayer2);
            double damageUser2 = cardPlayer2.CalculateDamage(cardPlayer1);

            if (damageUser1 > damageUser2)
            {
                _player1.AddCard(cardPlayer2);
                _player2.RemoveCard(cardPlayer2);
            }
            else if (damageUser2 > damageUser1)
            {
                _player2.AddCard(cardPlayer1);
                _player1.RemoveCard(cardPlayer1);
            }
            
            round++;
        }

        _result = "draw";
    }

    
    public string GetResult()
    {
        return _result;
    }

}