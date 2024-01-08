using System.Collections.Concurrent;

using MTCG.Models;
namespace MTCG.Logic;

public class BattleManager
{
    private BlockingCollection<User> _matchmakingQueue = new BlockingCollection<User>();
    private BlockingCollection<string> _resultQueue = new BlockingCollection<string>();
    private static int _requiredStringCount = 2;
    private static int _suppliedStringCount = 0;
    private static SemaphoreSlim _semaphore = new SemaphoreSlim(0);
    private static CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();


    public void StartMatchmaking() {
        Task.Run(() => MatchMakingLoop(), _cancellationTokenSource.Token);
    }

    public void MatchMakingLoop()
    {
        while (true)
        {
            _semaphore.Wait(_cancellationTokenSource.Token);
            
            List<User> users = new List<User>
            {
                _matchmakingQueue.Take(),
                _matchmakingQueue.Take()
            };

            Battle battle = new Battle(users[0], users[1]);
            battle.StartBattle();

            string log = battle.GetLog();

            // Add the result to the queue
            _resultQueue.Add(log);
            _resultQueue.Add(log);
        }
    }



    public void EnterMatchmaking(User user)
    {
        // Your logic to supply data to the loop goes here
        _matchmakingQueue.Add(user);
        
        // Increment the supplied string count
        Interlocked.Increment(ref _suppliedStringCount);

        // Release the semaphore to signal that data is available
        _semaphore.Release();

        // If supplied enough strings, reset the count and data
        if (_suppliedStringCount >= _requiredStringCount)
        {
            _suppliedStringCount = 0;
            _matchmakingQueue = new BlockingCollection<User>();  // Reset the data queue
        }

        
    }
    
    
    

    public string WaitForResult() {

        return _resultQueue.Take();  // Use the variable 'log' instead of calling 'resultQueue.Take()' again
    }
}