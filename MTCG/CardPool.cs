using MTCG.Models;
namespace MTCG;

public static class CardPool
{
    private static List<Card> _pool = new List<Card>();

    public static void ResetPool(List<Card> newPool) {
        _pool = newPool;
    }
    
    public static void AddCardsToPool(List<Card> newCards) {
        for (int i = 0; i < newCards.Count; i++) {
            _pool.Add(newCards[i]);
        }
    }

    public static List<Card> DrawPack() {
        List<Card> pack = new List<Card>();
        Random random = new Random();
        for (int i = 0; i < 5; i++) {
            pack.Add(_pool[random.Next(0, _pool.Count - 1)]);
        }

        return pack;
    }
}