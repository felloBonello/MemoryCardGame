/*
 * Program:         CardsLibrary.dll
 * Module:          Player.cs
 * Date:            April 2, 2017
 * Author:          Justin Bonello & Marcus Baldassarre
 * Description:     Contians attributes about the player objects.
 * Status:          Complete.
 */

using System.Runtime.Serialization;
using System.ServiceModel;

namespace CardsLibrary
{
    /// <summary>
    /// Contians attributes about the player objects.
    /// </summary>
    [DataContract]
    public class Player
    {
        /// <summary>
        /// HasTurn Property
        /// </summary>
        /// <value>
        /// See's if the player has a turn.
        /// </value>
        [DataMember]
        public bool? HasTurn { get; internal set; }

        /// <summary>
        /// Score Property
        /// </summary>
        /// <value>
        /// Holds the score.
        /// </value>
        [DataMember]
        public int Score { get; set; }

        /// <summary>
        /// Name Property
        /// </summary>
        /// <value>
        /// Holds the players name.
        /// </value>
        [DataMember]
        public string Name { get; private set; }

        /// <summary>
        /// PlayerIndex Property
        /// </summary>
        /// <value>
        /// Holds the players index.
        /// </value>
        [DataMember]
        public int PlayerIndex { get; private set; }

        /// <summary>
        /// Callback Property
        /// </summary>
        /// <value>
        /// Contians the callback from the client.
        /// </value>
        internal IClientCallback Callback { get; private set; }

        private static int indexCount = 1;

        /// <summary>
        /// Sets the attributes of the player.
        /// </summary>
        /// <param name="name">Takes in the players name.</param>
        public Player(string name)
        {
            Name = name;
            Score = 0;
            PlayerIndex = indexCount++;
            Callback = OperationContext.Current.GetCallbackChannel<IClientCallback>();
        }

        /// <summary>
        /// Resets the hasTurn variable
        /// </summary>
        internal void Reset()
        {
            HasTurn = false;
        }
    }
}