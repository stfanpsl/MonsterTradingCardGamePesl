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

namespace MonsterTradingCardGame
{
    class DataBase
    {

        public static void RemoveOwnerShipOfCardById(int userID, int cardID)
        {
            using (NpgsqlConnection con = GetConnection())
            {

                string preparedQuery = "delete from usercards where userid=" + userID + " and cardid = " + cardID;

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                //con.Close();
            }
        }

        public static TradeOffer GetTradeOfferByUsername(int id)
        {
            TradeOffer trade = null;
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT * FROM trades where userid = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                trade = new TradeOffer(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(3));
            }
            //con.Close();

            return trade;
        }

        public static void InsertTradeOffer(int userID, string wantedCardType, int wantedDMG, int tradeCardID)
        {
            using (NpgsqlConnection con = GetConnection())
            {
                string preparedQuery = "insert into trades(userid, wantedcardtype, wanteddamage, cardtotrade) values (" + userID + ", '" + wantedCardType +"', " + wantedDMG +", " + tradeCardID + ")";

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                Console.WriteLine("Zum Tauschen freigegeben");
            }
        }

        public static User GetUserById(int id)
        {
            User user = new User(0,"0", "0", 0, 0);
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT id, username, password, coins, elo FROM users where id = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                user = new User(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(3), rdr.GetInt32(4));
            }
            //con.Close();

            return user;

        }

        public static string HashString(string text, string salt = "")
        {
            if (String.IsNullOrEmpty(text))
            {
                return String.Empty;
            }

            // Uses SHA256 to create the hash
            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                // Convert the string to a byte array first, to be processed
                byte[] textBytes = System.Text.Encoding.UTF8.GetBytes(text + salt);
                byte[] hashBytes = sha.ComputeHash(textBytes);

                // Convert back to a string, removing the '-' that BitConverter adds
                string hash = BitConverter
                    .ToString(hashBytes)
                    .Replace("-", String.Empty);

                return hash;
            }
        }

        public static void AddOwnership(int userID, int cardID)
        {
            using (NpgsqlConnection con = GetConnection())
            {
                string preparedQuery = "insert into usercards(userid, cardid) values (" + userID + ", " + cardID + ")";

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                Console.WriteLine("Karte erhalten");
            }
        }

        public static void InsertUsernameAndPassword(string username, string password)
        {
            using (NpgsqlConnection con = GetConnection())
            {
                password = HashString(password);
                string preparedQuery = "insert into users(username, password, coins, elo, wins, losses) values ('" + username + "', '" + password + "', 20, 100, 0, 0)";

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                Console.WriteLine("Registry Successfull");
            }
        }

        public static int GetIdByUsernameAndPassword(string username, string password)
        {
            int id = 0;
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT id FROM users where username ='" +  username + "' and password ='" + HashString(password) + "'";
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                id = rdr.GetInt32(0);
            }

            //con.Close();

            return id;

        }

        public static int GetCoinsById(int id)
        {
            int coins = 0;
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT coins FROM users where id = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                coins = rdr.GetInt32(0);
            }
            //con.Close();
            return coins;

        }

        public static string[] GetAllUsers()
        {
            string[] UserList = new string[30];
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT username FROM users";
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            int x = 0;
            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                UserList[x] = rdr.GetString(0);
                //Console.WriteLine(UserList[x]);
                x++;
            }
            //con.Close();
            return UserList;
        }

        public static NpgsqlConnection GetConnection()
        {
            return new NpgsqlConnection(@"Server=localhost; Port=5433; User Id=postgres;Password=;Database=monstertradingcardgame");
        }

        public static void UpdateCoinsById(int id, int costs)
        {
            int new_coins = GetCoinsById(id);
            new_coins -= costs;

            using (NpgsqlConnection con = GetConnection())
            {
                
                string preparedQuery = "update users set coins =" + new_coins + " where id = " + id;

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                //con.Close();
            }
        }

        public static int GetEloById(int id)
        {
            int elo = 0;
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT elo FROM users where id = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                elo = rdr.GetInt32(0);
            }
            //con.Close();
            return elo;

        }

        public static void UpdateEloById(int id, int outcome)
        {
            int new_elo = GetEloById(id);
            new_elo += outcome;

            using (NpgsqlConnection con = GetConnection())
            {

                string preparedQuery = "update users set elo =" + new_elo + " where id = " + id;

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                //con.Close();
            }
        }

        public static int GetWinsById(int id)
        {
            int wins = 0;
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT wins FROM users where id = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                wins = rdr.GetInt32(0);
            }
            //con.Close();
            return wins;

        }

        public static void UpdateWinsById(int id)
        {
            int new_wins = GetWinsById(id);
            new_wins += 1;

            using (NpgsqlConnection con = GetConnection())
            {

                string preparedQuery = "update users set wins =" + new_wins + " where id = " + id;

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                //con.Close();
            }
        }

        public static int GetLossesById(int id)
        {
            int losses = 0;
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT losses FROM users where id = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                losses = rdr.GetInt32(0);
            }
            //con.Close();
            return losses;

        }

        public static void UpdateLossesById(int id)
        {
            int new_losses = GetLossesById(id);
            new_losses += 1;

            using (NpgsqlConnection con = GetConnection())
            {

                string preparedQuery = "update users set losses =" + new_losses + " where id = " + id;

                string query = preparedQuery;

                NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                con.Open();
                int n = cmd.ExecuteNonQuery();
                //con.Close();
            }
        }

        public static Card[] GetAllCards()
        {
            Card[] allCards = new Card[81];
            
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT * FROM cards";
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            int x = 0;
            while (rdr.Read())
            {
                allCards[x] = new Card(rdr.GetInt32(0),rdr.GetString(1), rdr.GetInt32(2), rdr.GetString(3), rdr.GetString(4));   
                x++;
            }
            //con.Close();
            return allCards;

        }

        public static Card GetCardById(int id)
        {
            Card card = new Card(0, "0", 0, "0", "0");
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT * FROM cards where id = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                card = new Card(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2), rdr.GetString(3), rdr.GetString(4));
            }
            //con.Close();

            return card;
        }

        public static int[] OwnerShipRelations(int id)
        {
            int[] cardsOwned = new int[20];

            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT cardid FROM usercards where userid = " + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            int x = 0;
            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                cardsOwned[x] = rdr.GetInt32(0);
                //Console.WriteLine(UserList[x]);
                x++;
            }

            //con.Close();
            return cardsOwned;
        }

        public static Card[] getUserDeck(int[] OwnedCards)
        {
            Card[] UserDeck = new Card[20];
            int x = 0;
            foreach (var cardid in OwnedCards)
            {
                UserDeck[x] = GetCardById(cardid);
                x++;
            }

            return UserDeck;
        }
        
        public static bool UsernameAlreadyTaken(string provingName)
        {
            string[] UserList = GetAllUsers();
            bool alreadyTaken = false;
            foreach(string User in UserList)
            {
                if(User == provingName)
                {
                    alreadyTaken = true;
                }
            }
            return alreadyTaken;
        }

        public static string GetPasswordByUsername(string username)
        {
            string password = "";
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT password FROM users where username = '" + username + "'";
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                password = rdr.GetString(0);
            }
            //con.Close();
            return password;
        }

        public static bool CorrectPassword(string username, string password)
        {
            return (HashString(password) == GetPasswordByUsername(username));
        }

        public static void InsertNewCards(int[] CardID, int PlayerID)
        {
            using (NpgsqlConnection con = GetConnection())
            {
                con.Open();
                foreach (int id in CardID)
                {
                    string preparedQuery = "insert into usercards(userid, cardid) values ('" + PlayerID + "', '" + id + "')";

                    string query = preparedQuery;

                    NpgsqlCommand cmd = new NpgsqlCommand(query, con);
                    
                    int n = cmd.ExecuteNonQuery();
                }

                
                Console.WriteLine("Registry Successfull");
            }
        }

        public static User[] GetAllUserObjectsForScoreBoard()
        {
            User[] allUsers = new User[50];

            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT * FROM users order by elo desc";
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            int x = 0;
            while (rdr.Read())
            {
                allUsers[x] = new User(rdr.GetInt32(0), rdr.GetString(1), rdr.GetString(2), rdr.GetInt32(3), rdr.GetInt32(4));
                x++;
            }
            //con.Close();
            return allUsers;

        }

        public static string GetUsernameByID(int id)
        {
            string cardType = "";
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT username from users where id =" + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                cardType = rdr.GetString(0);
            }
            //con.Close();
            return cardType;
        }

        public static string GetCardTypeByID(int id)
        {
            string cardType = "";
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT cardtype from cards where id =" + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                cardType = rdr.GetString(0);
            }
            //con.Close();
            return cardType;
        }

        public static TradeOffer[] GetAllTradeOffers()
        {
            TradeOffer[] allUsers = new TradeOffer[50];

            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT * FROM trades";
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();
            int x = 0;
            while (rdr.Read())
            {
                allUsers[x] = new TradeOffer(rdr.GetInt32(0), rdr.GetString(1), rdr.GetInt32(2), rdr.GetInt32(3));
                x++;
            }
            //con.Close();
            return allUsers;

        }

        public static string GetElementTypeByID(int id)
        {
            string elementType = "";
            NpgsqlConnection con = GetConnection();
            con.Open();

            string sql = "SELECT elementtype from cards where id =" + id;
            var cmd = new NpgsqlCommand(sql, con);

            NpgsqlDataReader rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                //Console.WriteLine("{0}", rdr.GetString(0));
                elementType = rdr.GetString(0);
            }
            //con.Close();
            return elementType;
        }
    }
}
