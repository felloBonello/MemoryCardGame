/*
 * Program:         CardsLibrary.dll
 * Module:          GameState.cs
 * Date:            April 2, 2017
 * Author:          Justin Bonello & Marcus Baldassarre
 * Description:     Holds all the information of the current state of the game.
 * Status:          Complete.
 */

using System.Collections.Generic;
using System.ServiceModel;

namespace CardsLibrary
{
    /// <summary>
    /// This is an interface for the callback from the client.
    /// </summary>
    public interface IClientCallback
    {
        /// <summary>
        /// Holds the update for the UI
        /// </summary>
        /// <param name="id">Takes in a player ID</param>
        [OperationContract(IsOneWay = true)]
        void UpdateUI(int id);
    }

    /// <summary>
    /// This is to ensure that the contracts are enabled properly.
    /// </summary>
    [ServiceContract(CallbackContract = typeof(IClientCallback))]
    public interface IGameState
    {
        /// <summary>
        /// Players Property
        /// </summary>
        /// <value>
        /// Holds the players.
        /// </value>
        List<Player> Players { [OperationContract]get; }


        /// <summary>
        /// CardDeck Property
        /// </summary>
        /// <value>
        /// Holds the deck.
        /// </value>
        Deck CardDeck { [OperationContract]get; }


        /// <summary>
        /// Message Property
        /// </summary>
        /// <value>
        /// Keeps the messages.
        /// </value>
        string Message { [OperationContract]get; }


        /// <summary>
        /// CardsFlippedThisTurn Property
        /// </summary>
        /// <value>
        /// Keeps tracks of cards flipped this turn.
        /// </value>
        int MoveCount { [OperationContract]get; }

        /// <summary>
        /// Registers a player for the game.
        /// </summary>
        /// <param name="playerName">Takes in the player name.</param>
        /// <returns>Returns the current index of the player.</returns>
        /// <seealso cref="NotifyClients()"/>
        [OperationContract]
        int RegisterPlayer(string playerName);

        /// <summary>
        /// Flips a card.
        /// </summary>
        /// <param name="key">Takes in a key to check what card it is.</param>
        /// <seealso cref="IsGameOver()"/>
        /// <seealso cref="NotifyClients()"/>
        [OperationContract(IsOneWay = true)]
        void FlipCard(string key);

        /// <summary>
        /// When the game is over this ensures that the players are properly unloaded
        /// </summary>
        /// <param name="playerID">Takes in the player ID</param>
        /// <seealso cref="NotifyClients()"/>
        /// <seealso cref="QueueNextTurn()"/>
        [OperationContract(IsOneWay = true)]
        void LeaveGame(int playerID);

        /// <summary>
        /// This is to queue up the next player to prepare them for their turn.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void QueueNextTurn();

        /// <summary>
        /// Notifys the clients as to what point the program is at.
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void NotifyClients();

        /// <summary>
        /// Check if the game is over.
        /// </summary>
        /// <returns>If the game is over, the boolean returns true else false.</returns>
        [OperationContract]
        bool IsGameOver();
    }

    /// <summary>
    /// Holds all the information of the current state of the game.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class GameState : IGameState
    {
        /// <summary>
        /// Players Property
        /// </summary>
        /// <value>
        /// Holds the players.
        /// </value>
        public List<Player> Players { get; private set; }

        /// <summary>
        /// CardDeck Property
        /// </summary>
        /// <value>
        /// Holds the deck.
        /// </value>
        public Deck CardDeck { get; set; }

        /// <summary>
        /// Message Property
        /// </summary>
        /// <value>
        /// Keeps the messages.
        /// </value>
        public string Message { get; private set; }

        /// <summary>
        /// MoveCount Property
        /// </summary>
        /// <value>
        /// Moves the count of the game play.
        /// </value>
        public int MoveCount { get; set; }

        /// <summary>
        /// CardsFlippedThisTurn Property
        /// </summary>
        /// <value>
        /// Keeps tracks of cards flipped this turn.
        /// </value>
        private List<string> CardsFlippedThisTurn { get; set; }

        /// <summary>
        /// CurrentPlayerIndex Property
        /// </summary>
        /// <value>
        /// The current players index.
        /// </value>
        private int CurrentPlayerIndex { get; set; }

        /// <summary>
        /// MatchCount Property
        /// </summary>
        /// <value>
        /// How many matches have been played.
        /// </value>
        private int MatchCount { get; set; }

        /// <summary>
        /// PlayerLeft Property
        /// </summary>
        /// <value>
        /// For if a player leaves.
        /// </value>
        private bool PlayerLeft { get; set; }

        private const int MAX_PLAYERS = 4;

        /// <summary>
        /// This is the default constructor
        /// </summary>
        public GameState()
        {
            Players = new List<Player>();
            CardsFlippedThisTurn = new List<string>();
            CardDeck = new Deck();
            MatchCount = 0;
            CurrentPlayerIndex = -1;
            PlayerLeft = false;
        }

        /// <summary>
        /// This method determines the winner between the players.
        /// </summary>
        /// <seealso cref="NotifyClients()"/>
        public void GetWinner()
        {
            int highScore = 0;
            List<Player> winners = new List<Player>();

            foreach (var p in Players)
                if (p.Score > highScore)
                    highScore = p.Score;

            string postText = " is the winner with " + highScore + " matches!";
            if (winners.Count > 1)
                postText = " are tied with " + highScore + " matches";

            string preText = "";
            foreach (var p in Players)
                if (p.Score == highScore)
                    preText += "Player " + p.PlayerIndex + ", ";

            preText = preText.Substring(0, preText.Length - 2);
            Message = preText + postText;

            NotifyClients();

        }

        /// <summary>
        /// Check if the game is over.
        /// </summary>
        /// <returns>If the game is over, the boolean returns true else false.</returns>
        public bool IsGameOver()
        {
            if (MatchCount == 27)
                return true;
            return false;
        }

        /// <summary>
        /// Registers a player for the game.
        /// </summary>
        /// <param name="playerName">Takes in the player name.</param>
        /// <returns>Returns the current index of the player.</returns>
        /// <seealso cref="NotifyClients()"/>
        public int RegisterPlayer(string playerName)
        {
            if (Players.Count >= MAX_PLAYERS)
                return -1;

            Player p = new Player(playerName);
            p.HasTurn = false;
            p.Score = 0;
            Players.Add(p);

            if (Players.Count == 1)
                QueueNextTurn();

            NotifyClients();
            return Players.IndexOf(p);
        }

        /// <summary>
        /// When the game is over this ensures that the players are properly unloaded
        /// </summary>
        /// <param name="playerID">Takes in the player ID</param>
        /// <seealso cref="NotifyClients()"/>
        /// <seealso cref="QueueNextTurn()"/>
        public void LeaveGame(int playerID)
        {
            Player p = Players[playerID];
            PlayerLeft = true;

            if (p.HasTurn == true && Players.Count > 1)
            {
                --CurrentPlayerIndex;
                Players.Remove(p);
                QueueNextTurn();
            }
            else
                Players.Remove(p);

            NotifyClients();
        }

        /// <summary>
        /// Flips a card.
        /// </summary>
        /// <param name="key">Takes in a key to check what card it is.</param>
        /// <seealso cref="IsGameOver()"/>
        /// <seealso cref="GetWinner()"/>
        /// <seealso cref="NotifyClients()"/>
        public void FlipCard(string key)
        {
            CardDeck.cards[key].isFlipped = !CardDeck.cards[key].isFlipped;
            CardsFlippedThisTurn.Add(key);
            ++MoveCount;

            if (CardsFlippedThisTurn.Count == 2 &&
                !(CardDeck.cards[CardsFlippedThisTurn[0]].Rank ==
                CardDeck.cards[CardsFlippedThisTurn[1]].Rank))
                Message = "Player " + Players[CurrentPlayerIndex].PlayerIndex + ": Click continue to end turn..";

            if (CardsFlippedThisTurn.Count == 2 &&
                CardDeck.cards[CardsFlippedThisTurn[0]].Rank ==
                CardDeck.cards[CardsFlippedThisTurn[1]].Rank)
            {
                Message = "Player " + Players[CurrentPlayerIndex].PlayerIndex + ": Matched " + CardDeck.cards[CardsFlippedThisTurn[0]].Rank.ToString() + "s, go again";
                ++MatchCount;
            }

            if(IsGameOver())
                GetWinner();

            NotifyClients();
        }

        /// <summary>
        /// Notifys the clients as to what point the program is at.
        /// </summary>
        public void NotifyClients()
        {
            for (int i = 0; i < Players.Count; ++i)
                Players[i].Callback.UpdateUI(i);
        }

        /// <summary>
        /// Flips the cards back if they are not a match, if they are removes them.
        /// </summary>
        /// <returns>Returns a boolean to check whether or not the two flipped cards are a match</returns>
        private bool ResetCardsFlippedThisTurn()
        {
            bool isMatch = false;
            if(CardsFlippedThisTurn.Count == 2 &&
               CardDeck.cards[CardsFlippedThisTurn[0]].Rank ==
               CardDeck.cards[CardsFlippedThisTurn[1]].Rank )
            {
                if(!PlayerLeft)
                    ++Players[CurrentPlayerIndex].Score;
                isMatch = true;                
                foreach (var key in CardsFlippedThisTurn)
                    CardDeck.cards[key].isMatched = true;
            }             
            else
            {
                isMatch = false;
                foreach (var key in CardsFlippedThisTurn)
                    CardDeck.cards[key].isFlipped = false;
            }
          
            CardsFlippedThisTurn = new List<string>();
            return isMatch;
        }

        /// <summary>
        /// This is to queue up the next player to prepare them for their turn.
        /// </summary>
        public void QueueNextTurn()
        {
            bool isMatch = ResetCardsFlippedThisTurn();
            if (CurrentPlayerIndex != -1)
                Players[CurrentPlayerIndex].HasTurn = false;
            if ((!isMatch || PlayerLeft) && ++CurrentPlayerIndex >= Players.Count)
                CurrentPlayerIndex = 0;
            if (!isMatch || PlayerLeft)
                Message = "Player " + Players[CurrentPlayerIndex].PlayerIndex + ": Select two cards";

            Players[CurrentPlayerIndex].HasTurn = true;                       
            MoveCount = 0;
            PlayerLeft = false;
        }

    }
}