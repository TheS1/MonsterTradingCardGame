using MTCG.Models;
namespace MTCG.Logic;

public class BattleManager
{
    private List<User> matchmakingList = new List<User>();
    
    public string enterMatchmaking(User user) {
        if (!matchmakingList.Contains(user))
        {
            matchmakingList.Add(user);
            Console.WriteLine($"{user.username} entered matchmaking.");
            User opponent = searchForMatch(user);
            Battle battle = new Battle(user, opponent);
            battle.StartBattle();
            return battle.getLog();
        }

        return "Already in Matchmaking";
    }

    private User searchForMatch(User user)
    {
        while (true) {

            // Search for a match among other users in the list
            for (int i = 0; i < matchmakingList.Count; i++) {
                User potentialMatch = matchmakingList[i];
                if (IsMatch(user, potentialMatch)) {
                    Console.WriteLine($"Match found! {user.username} vs {potentialMatch.username}");

                    matchmakingList.Remove(user);
                    matchmakingList.Remove(potentialMatch);
                    
                    return potentialMatch;
                }
            }

            // No match found for the current user, continue searching
            Console.WriteLine($"{user.username} is still in the queue. Waiting for a match...");

            // Simulate time passing or other actions...
            Thread.Sleep(3000); 
        }
    }
    private bool IsMatch(User user, User potentialMatch) {
        if (user.id == potentialMatch.id) {
            return false;
        }
        return true;
    }
}