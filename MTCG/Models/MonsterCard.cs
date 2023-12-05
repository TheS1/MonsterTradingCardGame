namespace MTCG.Models;

public class MonsterCard : Card
{
    public MonsterCard(string name, int damage, string elementType)
    {
        this.name = name;
        this.damage = damage;
        this.elementType = elementType;
    }

    public string name { get; }
    public int damage { get; }
    public string elementType { get; }
}