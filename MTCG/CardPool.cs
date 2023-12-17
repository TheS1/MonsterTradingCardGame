using MTCG.Models;
namespace MTCG;

public static class CardPool
{
    private static List<Card> pool = new List<Card>();

    public static void resetPool(List<Card> newPool) {
        pool = newPool;
    }
    
    public static void addCardsToPool(List<Card> newCards) {
        for (int i = 0; i < newCards.Count; i++) {
            pool.Add(newCards[i]);
        }
    }

    public static List<Card> drawPack() {
        List<Card> pack = new List<Card>();
        Random random = new Random();
        for (int i = 0; i < 5; i++) {
            pack.Add(pool[random.Next(0, pool.Count - 1)]);
        }

        return pack;
    }
}