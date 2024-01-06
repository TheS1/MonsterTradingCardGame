namespace MTCG.Models;

public class Card
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public double Damage { get; set; }
    public ElementType Element { get; set; }
    
    public double CalculateDamage(Card opponent)
    {
        if (opponent.Type == "Monster")
        {
            return Damage; // Monster cards' damage is not affected by element type
        }
        if (opponent.Type == "Spell")
        {
            // Spell cards' damage is affected by element type
            if (Element == opponent.Element)
            {
                return Damage; // No effect on damage
            }
            if ((Element == ElementType.Water && opponent.Element == ElementType.Fire) ||
                     (Element == ElementType.Fire && opponent.Element == ElementType.Regular) ||
                     (Element == ElementType.Regular && opponent.Element == ElementType.Water))
            {
                return Damage * 2; // Effective damage
            }

            return Damage / 2; // Not effective damage
            
        }

        return 0; // Default to 0 damage if unknown card type
    }

}