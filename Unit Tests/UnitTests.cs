using MTCG.Logic;
using MTCG.Models;

namespace Unit_Tests
{
    [TestFixture]
    public class BattleManagerTest
    {
        [Test]
        public void TestBattleP1Wins()
        {
            User player1 = new User() { Username = "p1", Elo = 1000};
            List<Card> p1Deck = new List<Card>
            {
                new() { Id = "1", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
                new() { Id = "2", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
                new() { Id = "3", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
                new() { Id = "4", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
            };
            player1.UpdateDeck(p1Deck);

            User player2 = new User() { Username = "p2", Elo = 1000};
            List<Card> p2Deck = new List<Card>
            {
                new() { Id = "5", Name = "Rizzard", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "6", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "7", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "8", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
            };
            player2.UpdateDeck(p2Deck);
            
            var battle = new BattleTest(player1, player2);
            battle.StartBattle();
            var result = battle.GetResult();

            Assert.That(result, Is.EqualTo("p1 wins"));
        }

        [Test]
        public void TestBattleP2Wins()
        {
            User player1 = new User() { Username = "p1", Elo = 1000};
            List<Card> p1Deck = new List<Card>
            {
                new() { Id = "1", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "2", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "3", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "4", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
            };
            player1.UpdateDeck(p1Deck);

            User player2 = new User() { Username = "p2", Elo = 1000};
            List<Card> p2Deck = new List<Card>
            {
                new() { Id = "5", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
                new() { Id = "6", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
                new() { Id = "7", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
                new() { Id = "8", Name = "WaterGoblin", Damage = 15, Type = "Monster", Element = ElementType.Water },
            };
            player2.UpdateDeck(p2Deck);

            var battle = new BattleTest(player1, player2);
            battle.StartBattle();
            var result = battle.GetResult();

            Assert.That(result, Is.EqualTo("p2 wins"));
        }

        [Test]
        public void TestBattleDraw()
        {
            User player1 = new User() { Username = "p1", Elo = 1000};
            List<Card> p1Deck = new List<Card>
            {
                new() { Id = "1", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "2", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "3", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "4", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
            };
            player1.UpdateDeck(p1Deck);

            User player2 = new User() { Username = "p2", Elo = 1000};
            List<Card> p2Deck = new List<Card>
            {
                new() { Id = "5", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "6", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "7", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
                new() { Id = "8", Name = "WaterGoblin", Damage = 5, Type = "Monster", Element = ElementType.Water },
            };
            player2.UpdateDeck(p2Deck);

            var battle = new BattleTest(player1, player2);
            battle.StartBattle();
            var result = battle.GetResult();

            Assert.That(result, Is.EqualTo("draw"));
        }

        [TestCase("WaterGoblin", 10, ElementType.Water, "FireGoblin", 15, ElementType.Fire, "FireGoblin won")]
        [TestCase("FireTroll", 15, ElementType.Fire, "WaterGoblin", 10, ElementType.Water, "FireTroll won")]
        [TestCase("Wizzard", 10, ElementType.Regular, "WaterGoblin", 10, ElementType.Water, "draw")]
        public void TestMonsterDmgCalc(string name1, double damage1, ElementType element1, string name2, double damage2, ElementType element2, string correctResult)
        {
            var c1 = new Card() { Id = "1", Name = name1, Damage = damage1, Type = "Monster", Element = element1 };
            var c2 = new Card() { Id = "2", Name = name2, Damage = damage2, Type = "Monster", Element = element2 };

            var damageCard1 = c1.CalculateDamage(c2);
            var damageCard2 = c2.CalculateDamage(c1);
            var result = "draw";
            if (damageCard1 > damageCard2)
            {
                result = c1.Name + " won";
            } else if (damageCard2 > damageCard1)
            {
                result = c2.Name + " won";
            }
            Assert.That(result, Is.EqualTo(correctResult));
        }
        
        [TestCase("FireSpell", 10, ElementType.Fire, "WaterSpell", 20, ElementType.Water, "WaterSpell won")]
        [TestCase("FireSpell", 20, ElementType.Fire, "WaterSpell",  5, ElementType.Water, "draw")]
        [TestCase("FireSpell", 90, ElementType.Fire, "WaterSpell",  5, ElementType.Water, "FireSpell won")]
        public void TestSpellFights(string name1, double damage1, ElementType element1, string name2, double damage2, ElementType element2, string correctResult)
        {
            var c1 = new Card() { Id = "1", Name = name1, Damage = damage1, Type = "Spell", Element = element1 };
            var c2 = new Card() { Id = "2", Name = name2, Damage = damage2, Type = "Spell", Element = element2 };

            var damageCard1 = c1.CalculateDamage(c2);
            var damageCard2 = c2.CalculateDamage(c1);
            var result = "draw";
            if (damageCard1 > damageCard2)
            {
                result = c1.Name + " won";
            } else if (damageCard2 > damageCard1)
            {
                result = c2.Name + " won";
            }
            Assert.That(correctResult, Is.EqualTo(result));
        }
        
        [TestCase("FireSpell", 10, "Spell", ElementType.Fire, "WaterGoblin",  10, "Monster", ElementType.Water, "WaterGoblin won")]
        [TestCase("WaterSpell", 10, "Spell", ElementType.Water, "WaterGoblin",  10, "Monster",ElementType.Water, "draw")]
        [TestCase("RegularSpell", 10, "Spell", ElementType.Regular, "WaterGoblin",  10, "Monster",ElementType.Water, "RegularSpell won")]
        [TestCase("RegularSpell", 10, "Spell",ElementType.Regular, "Knight",  15, "Monster", ElementType.Regular, "Knight won")]
        public void TestMixedFights(string name1, double damage1, string type1, ElementType element1, string name2, double damage2, string type2, ElementType element2, string correctResult)
        {
            var c1 = new Card() { Id = "1", Name = name1, Damage = damage1, Type = type1, Element = element1 };
            var c2 = new Card() { Id = "2", Name = name2, Damage = damage2, Type = type2, Element = element2 };

            var damageCard1 = c1.CalculateDamage(c2);
            var damageCard2 = c2.CalculateDamage(c1);
            var result = "draw";
            if (damageCard1 > damageCard2)
            {
                result = c1.Name + " won";
            } else if (damageCard2 > damageCard1)
            {
                result = c2.Name + " won";
            }
            Assert.That(correctResult, Is.EqualTo(result));
        }

        [TestCase(ElementType.Fire, 10)]
        [TestCase(ElementType.Water, 20)]
        [TestCase(ElementType.Regular, 5)]
        public void TestFireElementSpellDamage(ElementType element, double expectedDamage)
        {
            var c1 = new Card() {Id = "1", Type = "Spell", Damage = 10, Element = element};
            var c2 = new Card() {Id = "2", Type = "Monster", Damage = 0, Element = ElementType.Fire};

            Assert.That(expectedDamage, Is.EqualTo(c1.CalculateDamage(c2)));
        }
        
        [TestCase(ElementType.Water, 10)]
        [TestCase(ElementType.Regular, 20)]
        [TestCase(ElementType.Fire, 5)]
        public void TestWaterElementSpellDamage(ElementType element, double expectedDamage)
        {
            var c1 = new Card() {Id = "1", Type = "Spell", Damage = 10, Element = element};
            var c2 = new Card() {Id = "2", Type = "Monster", Damage = 0, Element = ElementType.Water};

            Assert.That(expectedDamage, Is.EqualTo(c1.CalculateDamage(c2)));
        }
        
        [TestCase(ElementType.Regular, 10)]
        [TestCase(ElementType.Fire, 20)]
        [TestCase(ElementType.Water, 5)]
        public void TestRegularElementSpellDamage(ElementType element, double expectedDamage)
        {
            var c1 = new Card() {Id = "1", Type = "Spell", Damage = 10, Element = element};
            var c2 = new Card() {Id = "2", Type = "Monster", Damage = 0, Element = ElementType.Regular};

            Assert.That(expectedDamage, Is.EqualTo(c1.CalculateDamage(c2)));
        }
        
        [TestCase(ElementType.Regular, ElementType.Regular)]
        [TestCase(ElementType.Regular, ElementType.Fire)]
        [TestCase(ElementType.Regular, ElementType.Water)]
        [TestCase(ElementType.Fire, ElementType.Regular)]
        [TestCase(ElementType.Fire, ElementType.Fire)]
        [TestCase(ElementType.Fire, ElementType.Water)]
        [TestCase(ElementType.Water, ElementType.Regular)]
        [TestCase(ElementType.Water, ElementType.Fire)]
        [TestCase(ElementType.Water, ElementType.Water)]
        public void TestOnlyCardElementDamage(ElementType element, ElementType element2)
        {
            var c1 = new Card() {Id = "1", Type = "Monster", Damage = 10, Element = element};
            var c2 = new Card() {Id = "2", Type = "Monster", Damage = 0, Element = element2};

            
            Assert.That(c1.CalculateDamage(c2), Is.EqualTo(10));
        }
        
        [TestCase(ElementType.Regular, ElementType.Regular, 10)]
        [TestCase(ElementType.Regular, ElementType.Fire  ,  5)]
        [TestCase(ElementType.Regular, ElementType.Water , 20)]
        [TestCase(ElementType.Fire  , ElementType.Regular, 20)]
        [TestCase(ElementType.Fire  , ElementType.Fire  , 10)]
        [TestCase(ElementType.Fire  , ElementType.Water ,  5)]
        [TestCase(ElementType.Water , ElementType.Regular, 5)]
        [TestCase(ElementType.Water , ElementType.Fire  , 20)]
        [TestCase(ElementType.Water , ElementType.Water , 10)]
        public void TestOnlySpellDamage(ElementType element, ElementType element2, double expectedDamage)
        {
            var c1 = new Card() {Id = "1", Type = "Spell", Damage = 10, Element = element};
            var c2 = new Card() {Id = "2", Type = "Monster", Damage = 0, Element = element2};

            Assert.That(expectedDamage, Is.EqualTo(c1.CalculateDamage(c2)));
        }
        
        [Test]
        public void TestPlayerDeckGetRandomCard()
        {
            User player = new User();
            var card = new Card() {Id = "1", Type = "Spell", Damage = 10, Element = ElementType.Water};
           
            player.AddCard(card);

            Assert.That(player.ChooseRandomCard(), Is.EqualTo(card));
        }
        
        
    }
}