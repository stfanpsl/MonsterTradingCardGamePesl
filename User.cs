using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace MonsterTradingCardGame
{
    public class User
    {
        int id;
        string username;
        string password;
        Card[] stack;
        int coins;
        int[] BattleDeck;
        int elo;
        List<Card> FightDeck;
        int wins;
        int losses;
        int tmpInd;

        public User(int id, string username, string password, int coins, int elo)
        {
            this.id = id;
            this.username = username;
            this.password = password;
            this.coins = coins;
            this.elo = elo;
            this.BattleDeck = initDeck();
            this.stack = initStack();
            FightDeck = new List<Card>();
            this.wins = DataBase.GetWinsById(id);
            this.losses = DataBase.GetLossesById(id);
        }

        public void RemoveFromStack(int cardID)
        {
            for(int i = 0; i < 20; i++)
            {
                if(this.stack[i].getID() == cardID)
                {
                    this.stack[i] = null;
                    tmpInd = i;
                }
            }
        }

        public int GetTmpIndex()
        {
            return tmpInd;
        }

        public List<Card> GetFighDeck()
        {
            return FightDeck;
        }

        public void printUserInfo()
        {
            Console.WriteLine("" +
                "------------------\n" +
                "Username: " + this.username + "" +
                "\nHashed Password: " + this.password + "" +
                "\nCoins: " + this.coins + "" +
                "\nELO: " + this.elo + "" +
                "\nWINS:" + this.wins + "" +
                "\nLOSSES:" + this.losses + "" +
                "\n------------------");
        }

        public bool InStackOfCards(int id)
        {
            bool inStack = false;
            foreach(Card card in stack)
            {
                if(card.getID() == id)
                {
                    inStack = true;
                }
            }
            return inStack;
        }

        public string getPassword()
        {
            return this.password;
        }

        public void addCardsToBattleDeck()
        {
            int cardID = 0;
            string inputValue = "";
            printStackOfCards();
            int ctr = 0;
            while(ctr < 4)
            {
                Console.WriteLine("Wählen Sie die ID der gewünschten Karten:");
                inputValue = Console.ReadLine();
                cardID = Int32.Parse(inputValue);
                if (InStackOfCards(cardID))
                {
                    BattleDeck[ctr] = cardID;
                    ctr++;
                }
                else
                {
                    Console.WriteLine("Du besitzt diese Karte nicht");
                }
                
            }
            

        }


        public void AddCardToFightingDeck(Card newCard)
        {
            FightDeck.Add(newCard);
        }

        public void RemoveCardFromFightingDeck(Card losingCard)
        {
            FightDeck.Remove(losingCard);
        }

        public void setFightDeck(int[] BattleDeck)
        {
            foreach(int ID in BattleDeck)
            {
                if(ID != 0)
                FightDeck.Add(DataBase.GetCardById(ID));
            }
        }

        public int getNextEmptySlotInBattleDeck(int[] BattleDeck)
        {
            bool isNotEmpty = true;
            int index = 0;
            while (isNotEmpty && index < 8)
            {
                if(BattleDeck[index] == 0)
                {
                    isNotEmpty = false;
                    return index;
                }
                index++;

            }
            return 0;
        }
        

        public int getID()
        {
            return this.id;
        }

        public string getUsername()
        {
            return this.username;
        }

        public int getELO()
        {
            return this.elo;
        }


        public int[] initDeck()
        {
            int[] Deck = new int[4];
            for(int i = 0; i < 4; i++)
            {
                Deck[i] = 0;
            }

            return Deck;
        }

        public Card PullRandomCardFromFightingDeck()
        {
            var random = new Random();
            int index = random.Next(FightDeck.Count);
            return FightDeck[index];
        }

        public Card[] initStack()
        {
            Card[] Deck = new Card[20];
            for (int i = 0; i < 20; i++)
            {
                Deck[i] = null;
            }

            return Deck;
        }

        public void printStackOfCards()
        {
            foreach(Card card in stack)
            {
                card.printInfo();
            }
        }

        public int getCoins()
        {
            return this.coins;
        }

        public void setUserStack(Card[] UserStack)
        {
            this.stack = UserStack;
        }

        public Card[] getUserStack()
        {
            return this.stack;
        }

        public void printBattleDeck()
        {
            Card currCard = null;

            foreach(int cardID in BattleDeck)
            {
                currCard = DataBase.GetCardById(cardID);
                if(currCard.getID() != 0)
                {
                    currCard.printInfo();
                }
                    
            }
        }

        public void PrintBattleDeckEntrys()
        {
            foreach(int i in BattleDeck)
            {
                Console.WriteLine(i);
            }
        }

        public int[] getBattleDeck()
        {
            return this.BattleDeck;
        }

        public void setBattleDeck(int[] Deck)
        {
            this.BattleDeck = Deck;
        }

        public bool outOfCards()
        {
            bool CardsInDeck = false;
            foreach(int cardID in BattleDeck)
            {
                if(cardID != 0)
                {
                    CardsInDeck = true;
                }
            }
            return CardsInDeck;
        }

    }
}
