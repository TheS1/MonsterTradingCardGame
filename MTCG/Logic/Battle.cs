using MTCG.Models;
public class Battle
{
    private User player1;
    private User player2;
    private int winner;
    private DateTime battleDate;
    private int maxRounds;
    private string log;
    public Battle(User player1, User player2, int maxRounds = 100)
    {
        this.player1 = player1;
        this.player2 = player2;
        this.maxRounds = maxRounds;
    }

    public void StartBattle()
    {
        int round = 1;

        while (round <= maxRounds) {
            if (!player1.HasCards()) {
                winner = 1;
                log += player1.username + " wins!\n";
                break; // Exit the loop if one of the players has no cards
            } 
            if (!player2.HasCards()) {
                winner = 2;
                log += player2.username + " wins!\n";
                break; // Exit the loop if one of the players has no cards
            } 

            Card cardPlayer1 = player1.ChooseRandomCard();
            Card cardPlayer2 = player2.ChooseRandomCard();

            double damageUser1 = cardPlayer1.CalculateDamage(cardPlayer2);
            double damageUser2 = cardPlayer2.CalculateDamage(cardPlayer1);

            Console.WriteLine($"{player1.username}'s {cardPlayer1.Name} attacks with {damageUser1} damage.");
            Console.WriteLine($"{player2.username}'s {cardPlayer2.Name} attacks with {damageUser2} damage.");
            log += "Round " + round + ": " + player1.username + ": " + cardPlayer1.Name + " " + cardPlayer1.Damage +
                   " vs " + cardPlayer2.Damage + " " + cardPlayer2.Name + " => " + damageUser1 + " vs " + damageUser2;
            
            if (damageUser1 > damageUser2)
            {
                log += " => " + cardPlayer1.Name + " wins\n";
                Console.WriteLine($"{player1.username} wins the round!");
                player1.AddCard(cardPlayer2);
                player2.RemoveCard(cardPlayer2);
            }
            else if (damageUser2 > damageUser1)
            {
                log += " => " + cardPlayer2.Name + " wins\n";
                Console.WriteLine($"{player2.username} wins the round!");
                player2.AddCard(cardPlayer1);
                player1.RemoveCard(cardPlayer1);
            }
            else
            {
                log += " Draw (no action)\n";
                Console.WriteLine("It's a draw! No cards are moved.");
            }

            Console.WriteLine($"{player1.username}'s Deck: {string.Join(", ", player1.GetDeck())}");
            Console.WriteLine($"{player2.username}'s Deck: {string.Join(", ", player2.GetDeck())}");
            Console.WriteLine();
    
            round++;
        }
    
        log += "Draw after 100 rounds";
        winner = 0;


    }

    public string getLog()
    {
        return log;
    }
}