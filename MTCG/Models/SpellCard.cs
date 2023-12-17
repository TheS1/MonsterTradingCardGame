namespace MTCG.Models;

public class SpellCard : Card
{
    public SpellCard(string name, int damage, string elementType)
    {
        this.name = name;
        this.damage = damage;
        this.elementType = elementType;
    }

    public string name { get; }
    public int damage { get; }
    public string elementType { get; }
}