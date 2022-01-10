using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    public class Card
    {
        const int EFFECTIVITY = 2;
        int id;
        string name;
        string cardType;
        string elementType;
        int damage;

            public Card(int id, string name, int damage, string cardType, string elementType){
            this.id = id;
            this.name = name;
            this.damage = damage;
            this.cardType = cardType;
            this.elementType = elementType;
                }

        public void printInfo()
        {
            Console.WriteLine("ID: " + id + 
                "\nName: " + name + 
                "\nDamage: " + damage + 
                "\nCard Type: " + cardType + 
                "\nElement Type: " + elementType + 
                "\n--------------------");
        }

        public int getID()
        {
            return this.id;
        }

        public string getCardType()
        {
            return this.cardType;
        }

        public string getElementType()
        {
            return this.elementType;
        }

        public int getDamage()
        {
            return this.damage;
        }

        public string getName()
        {
            return name;
        }

        public int CalcDamage(Card enemyCard)
        {
            //Karte ist WATER

            if (enemyCard.getElementType() == "FIRE" && this.getElementType() == "WATER")
            {
                return getDamage() * EFFECTIVITY;
            }
            else if (enemyCard.getElementType() == "NORMAL" && this.getElementType() == "WATER")
            {
                return getDamage() / EFFECTIVITY;
            }
            else

            //Karte ist FIRE

            if(enemyCard.getElementType() == "WATER" && this.getElementType() == "FIRE")
            {
                return getDamage() / EFFECTIVITY;

            }else if(enemyCard.getElementType() == "NORMAL" && this.getElementType() == "FIRE")
            {
                return getDamage() * EFFECTIVITY;

            }else

            //Karte ist NORMAL

            if(enemyCard.getElementType() == "WATER" && this.getElementType() == "NORMAL")
            {
                return getDamage() * EFFECTIVITY;

            }else if(enemyCard.getElementType() == "FIRE" && this.getElementType() == "NORMAL")
            {
                return getDamage() / EFFECTIVITY;
            }
            else
            {
                return getDamage();
            }
        }

        public static Card calculateWinningCard(Card playerCard, Card enemyCard)
        {
            Card winningCard = null;


            //Specialties 
            if (playerCard.getName() == "Goblin" && enemyCard.getName() == "Dragon")
            {
                return enemyCard;
            }

            if (playerCard.getName() == "Dragon" && enemyCard.getName() == "Goblin")
            {
                return playerCard;
            }

            if (playerCard.getName() == "Ork" && enemyCard.getName() == "Wizzard")
            {
                return enemyCard;
            }

            if (playerCard.getName() == "Wizzard" && enemyCard.getName() == "Ork")
            {
                return playerCard;
            }

            if (playerCard.getName() == "Knight" && enemyCard.getCardType() == "SPELL" && enemyCard.getElementType() == "WATER")
            {
                return enemyCard;
            }

            if (enemyCard.getName() == "Knight" && playerCard.getCardType() == "SPELL" && playerCard.getElementType() == "WATER")
            {
                return playerCard;
            }

            if (playerCard.getName() == "Kraken" && enemyCard.getCardType() == "SPELL")
            {
                return playerCard;
            }

            if (playerCard.getCardType() == "SPELL" && enemyCard.getName() == "Kraken")
            {
                return enemyCard;
            }

            if (playerCard.getName() == "Dragon" && enemyCard.getName() == "FireElve")
            {
                return enemyCard;
            }

            if (playerCard.getName() == "FireElve" && enemyCard.getName() == "Dragon")
            {
                return playerCard;
            }


            //PURE MONSTER FIGHT
            if (playerCard.getCardType() == "MONSTER" && enemyCard.getCardType() == "MONSTER")
            {
                if (playerCard.getDamage() > enemyCard.getDamage())
                {
                    winningCard = playerCard;
                }

                if (enemyCard.getDamage() > playerCard.getDamage())
                {
                    winningCard = enemyCard;
                }

                if (enemyCard.getDamage() == playerCard.getDamage())
                {
                    return null;
                }

            }

            //SPELL INLCUDED FIGHT
            if (playerCard.getCardType() == "SPELL" || enemyCard.getCardType() == "SPELL")
            {
                if (playerCard.CalcDamage(enemyCard) > enemyCard.CalcDamage(playerCard))
                {
                    winningCard = playerCard;
                }

                if (playerCard.CalcDamage(enemyCard) < enemyCard.CalcDamage(playerCard))
                {
                    winningCard = enemyCard;
                }

                if (playerCard.CalcDamage(enemyCard) == enemyCard.CalcDamage(playerCard))
                {
                    return enemyCard;
                }
            }


            return winningCard;
        }
    }
}
