using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonsterTradingCardGame
{
    class TradeOffer
    {
        int userID;
        string wantedCardType;
        int wantedDMG;
        int cardIdToTrade;

        public TradeOffer(int userID, string wantedCardType, int wantedDMG, int cardIdToTrade)
        {
            this.userID = userID;
            this.wantedCardType = wantedCardType;
            this.wantedDMG = wantedDMG;
            this.cardIdToTrade = cardIdToTrade;
        }

        public void printOffer()
        {
            Card tradingCard = DataBase.GetCardById(cardIdToTrade);

            Console.WriteLine("Spieler " + DataBase.GetUsernameByID(userID) + " mit ID: " + userID + " tauscht seinen/ihren " + tradingCard.getName() + " mit " + tradingCard.getDamage() + " DMG " +
                "für eine " + wantedCardType + "-Karte mit " + wantedDMG + " mindestens DMG");
        }

        public string GetWantedCardType()
        {
            return this.wantedCardType;
        }

        public int GetUserID()
        {
            return this.userID;
        }

        public int GetWantedDMG()
        {
            return this.wantedDMG;
        }

        public int GetCardID()
        {
            return this.cardIdToTrade;
        }
    }
}
