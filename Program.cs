using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Npgsql;
using Npgsql.Replication.PgOutput.Messages;
using NpgsqlTypes;
//Konstanten in CAPS

namespace MonsterTradingCardGame
{
    class Program
    {
        const int PACKPRICE = 5;
        const int WIN = 3;
        const int LOSS = -5;
        const int MAXROUNDS = 100;


        static void Main(string[] args)
        {
            int PlayerID = RegisterAndLogin();
            User Player1 = DataBase.GetUserById(PlayerID);
            Player1.setUserStack(DataBase.getUserDeck(DataBase.OwnerShipRelations(PlayerID)));
            User enemyKI = new User(0, "GEGNER", "Passwort", 0, 0);

            while (true)
            {

                Console.WriteLine("You've these Options:\n" +
                    "[1] Acquire some cards\n" +
                    "[2] Define a deck of monsters/spells\n" +
                    "[3] Battle\n" +
                    "[4] Score-Board\n" +
                    "[5] Profile-Page\n" +
                    "[6] Trade-Center\n" +
                    "[7] Quit");

                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        if (Player1.getCoins() > 0)
                        {
                            int[] boughtPack = randomPackage();
                            DataBase.InsertNewCards(boughtPack, PlayerID);
                            DataBase.UpdateCoinsById(PlayerID, PACKPRICE);
                            Player1.setBattleDeck(DataBase.OwnerShipRelations(PlayerID));

                            foreach (Card card in Player1.getUserStack())
                            {
                                if (card.getID() != 0)
                                {
                                    card.printInfo();
                                }

                            }
                            Console.ReadLine();
                        }
                        else
                        {
                            Console.WriteLine("You don´t have enough coins");
                        }


                        break;

                    case "2":

                        Player1.addCardsToBattleDeck();
                        Player1.printBattleDeck();
                        Console.ReadLine();

                        break;

                    case "3":

                        BattleLogic(Player1, enemyKI);

                        break;

                    case "4":

                        Console.WriteLine("SCORE-BOARD");

                        User[] Scoreboard = new User[50];
                        Scoreboard = DataBase.GetAllUserObjectsForScoreBoard();
                        int place = 1;
                        foreach (User user in Scoreboard)
                        {
                            if (user != null)
                            {
                                Console.WriteLine("RANK: " + place + " ID: " + user.getID() + " USERNAME: "
                                + user.getUsername() + " COINS: "
                                + user.getCoins() + " ELO: "
                                + user.getELO() + " WINS: "
                                + DataBase.GetWinsById(user.getID()) + " LOSSES: "
                                + DataBase.GetLossesById(user.getID()));
                            }
                            place++;

                        }
                        break;

                    case "5":

                        Player1.printUserInfo();
                        break;
                    
                    case "6":

                        Console.WriteLine("TRADE-CENTER\nMöchtest du eine Karte erwerben[e] oder zum Tausch[t] freigeben?");
                        string tradeInput = Console.ReadLine();
                        switch (tradeInput)
                        {
                            case "e":

                                TradeOffer[] allOffers = DataBase.GetAllTradeOffers();
                                foreach(TradeOffer offer in allOffers)
                                {
                                    if(offer != null)
                                    {
                                        offer.printOffer();
                                    }
                                    
                                }

                                Console.WriteLine("Wessen Angebot nimmst du an?[ID]");

                                string anbieter0 = Console.ReadLine();
                                int anbieter = Int32.Parse(anbieter0);
                                TradeOffer currTrade = null;

                                foreach(TradeOffer offer in allOffers)
                                {
                                    
                                    
                                    if(offer.GetUserID() == anbieter)
                                    {
                                        currTrade = offer;
                                        break;
                                    }
                                }

                                Console.WriteLine("Welche Karte willst du hergeben?");

                                Player1.printStackOfCards();

                                Console.WriteLine("Gib die ID der Karte ein");

                                string userInput0 = Console.ReadLine();
                                int userTradeOffer = Int32.Parse(userInput0);

                                Card tmpCard = DataBase.GetCardById(userTradeOffer);

                                if (tmpCard.getDamage() >= currTrade.GetWantedDMG() && tmpCard.getCardType() == currTrade.GetWantedCardType())
                                {
                                    DataBase.RemoveOwnerShipOfCardById(Player1.getID(), tmpCard.getID());
                                    DataBase.AddOwnership(Player1.getID(), currTrade.GetCardID());
                                    DataBase.AddOwnership(currTrade.GetUserID(), tmpCard.getID());

                                }
                                else
                                {
                                    Console.WriteLine("Diese Karte entspricht nicht den Anforderungen");
                                }


                                
                                break;

                            case "t":

                                Console.WriteLine("Möchtest du eine [MONSTER] oder [SPELL]-Karte?");
                                string wantedCardType = Console.ReadLine();
                                Console.WriteLine("Wieviel Damage soll die Karte mindestens machen?");
                                string wantedDMG0 = Console.ReadLine();
                                int wantedDMG = Int32.Parse(wantedDMG0);

                                Console.WriteLine("Welche Karte möchtest du dafür hergeben?");
                                string tradeCard0 = Console.ReadLine();
                                int tradeCard = Int32.Parse(tradeCard0);

                                DataBase.InsertTradeOffer(Player1.getID(), wantedCardType, wantedDMG, tradeCard);
                                Player1.RemoveFromStack(tradeCard);
                                DataBase.RemoveOwnerShipOfCardById(Player1.getID(), tradeCard);

                                break;
                        }


                        break; ;

                    case "7":

                        return;

                    default:
                        Console.WriteLine("Keine übereinstimmende Funktion gefunden. Bitte versuche es erneut!");
                        break;

                }
            }
        }

        private static int RegisterAndLogin()
        {
            int PlayerID = 0;
            Console.WriteLine("Welcome to MonsterTradingCardGame by Stefan Pesl\nWe want you to \n[L] Login or [R] Register");
            string input = Console.ReadLine();

            switch (input)
            {
                case "L":

                    Console.WriteLine("Username:");
                    string inputUsername = Console.ReadLine();
                    Console.WriteLine("Password:");
                    string inputPassword = GetHiddenConsoleInput();

                    while(!DataBase.CorrectPassword(inputUsername, inputPassword))
                    {
                        Console.WriteLine("Falsches Passwort! Bitte versuche es erneut:");
                        inputPassword = GetHiddenConsoleInput();
                    }

                    if(DataBase.CorrectPassword(inputUsername, inputPassword))
                    {
                        Console.WriteLine("Eingeloggt");
                        PlayerID = DataBase.GetIdByUsernameAndPassword(inputUsername, inputPassword);
                    }
                    break;

                case "R":

                    string username = "";
                    string password;

                    Console.WriteLine("Wähle einen Nutzernamen aus:");

                    username = Console.ReadLine();

                    if (DataBase.UsernameAlreadyTaken(username))
                    {
                        while (DataBase.UsernameAlreadyTaken(username))
                        {
                            Console.WriteLine("Dieser Nutzername ist leider schon vergeben, bitte versuche es erneut");
                            username = Console.ReadLine();
                        }
                    }
                    
                    Console.WriteLine("Wählen Sie ein Passwort:");

                    password = GetHiddenConsoleInput();

                    DataBase.InsertUsernameAndPassword(username, password);
                    PlayerID = DataBase.GetIdByUsernameAndPassword(username, password);

                    break;

                default:

                    break;

            }
            return PlayerID;
        }

        private static string GetHiddenConsoleInput()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }

        public static int[] randomPackage()
        {
            Random ranNumber = new Random();
            int[] buyPackage = new int[5];
            for(int i = 0; i < 5; i++)
            {   
                buyPackage[i] = ranNumber.Next(1, 81);    
            }
            return buyPackage;
        }

        public static void BattleLogic(User Player, User enemyKI)
        {
            int rounds = 0;
            if (Player.getBattleDeck()[0] == 0)
            {
                Console.WriteLine("You haven´t defined a BattleDeck yet");
                return;
            }

            //Console.WriteLine("PLAYERDECK");
            //Player.printBattleDeck();

            int[] enemyBattleDeck = randomPackage();
            enemyBattleDeck[4] = 0;
            enemyKI.setFightDeck(enemyBattleDeck);
            Player.setFightDeck(Player.getBattleDeck());
            //Console.WriteLine("ENEMYDECK");
            //enemyKI.printBattleDeck();


            foreach (Card item in Player.GetFighDeck())
            {
                item.printInfo();
            }

            foreach (Card item in enemyKI.GetFighDeck())
            {
                item.printInfo();
            }


            while (rounds < MAXROUNDS)
            {
                if(Player.GetFighDeck().Count == 0 || enemyKI.GetFighDeck().Count == 0)
                {

                    if(Player.GetFighDeck().Count == 0)
                    {
                        Console.WriteLine("DEIN GEGNER HAT GEWONNEN");
                        DataBase.UpdateEloById(Player.getID(), LOSS);
                        DataBase.UpdateLossesById(Player.getID());
                    }
                    else
                    {
                        Console.WriteLine("DU HAST GEWONNEN");
                        DataBase.UpdateEloById(Player.getID(), WIN);
                        DataBase.UpdateWinsById(Player.getID());
                    }

                    return;
                }
                

                //Karten werden random gezogen von beiden Seiten

                Card playerCard = Player.PullRandomCardFromFightingDeck();
                
                Card enemyCard = enemyKI.PullRandomCardFromFightingDeck();

                if (playerCard.getCardType() == "MONSTER" && enemyCard.getCardType() == "MONSTER")
                {
                    Console.WriteLine("PlayerCard: " + playerCard.getName() + "(" + playerCard.getCardType() + ")" + "(" + playerCard.getElementType() + ")" + "[" + playerCard.getDamage() +
                   " DMG] tritt gegen " + enemyCard.getName() +
                   "(" + playerCard.getCardType() + ")" + " (" + playerCard.getElementType() + ")" + "[" + enemyCard.getDamage() + " DMG] an\n" +
                   "------------------------------");
                }
                else
                {
                    Console.WriteLine("PlayerCard: " + playerCard.getName() + "(" + playerCard.getCardType() + ")" + "(" + playerCard.getElementType() + ")" + "[" + playerCard.getDamage() +
                   " DMG] --> [" + playerCard.CalcDamage(enemyCard) + " DMG] tritt gegen " + enemyCard.getName() +
                   "(" + enemyCard.getCardType() + ")" + " (" + enemyCard.getElementType() + ")" + "[" + enemyCard.getDamage() + " DMG] --> [" + enemyCard.CalcDamage(playerCard) + "DMG] an\n" +
                   "------------------------------");
                }

                //Pure Monster Fights haben keine Damage-Rechnung
                //1 Spell = Damage-Rechnung
                //higher calculated damage wins

                Card winner = Card.calculateWinningCard(playerCard, enemyCard);
                if(winner == null)
                {
                    Console.WriteLine("Unentschieden");
                }
               

                if (winner == playerCard)
                {
                    Player.AddCardToFightingDeck(enemyCard);
                    enemyKI.RemoveCardFromFightingDeck(enemyCard);
                    Console.WriteLine(playerCard.getName() + " hat gewonnen\n" +
                        "------------------------------");
                }

                if (winner == enemyCard)
                {
                    enemyKI.AddCardToFightingDeck(playerCard);
                    Player.RemoveCardFromFightingDeck(playerCard);
                    Console.WriteLine(enemyCard.getName() + " hat gewonnen\n" +
                        "------------------------------");
                }

                
                //Verlierer Karte wandert ins Deck des Gewinners
                //Kein Verlierer = Kein Karten wechsel
                rounds++;
                Console.WriteLine(rounds);
                
            }

            if(rounds >= 100)
            {
                Console.WriteLine("Kein Spieler hat gewonnen, es werden keine ELO verteilt");
            }


        }

    }
}
