/*
 * Program:         CardsLibrary.dll
 * Module:          Card.cs
 * Date:            April 2, 2017
 * Author:          Justin Bonello & Marcus Baldassarre
 * Description:     This Class Represents a standard playing card.
 * Status:          Complete.
 */

using System;
using System.Runtime.Serialization;

namespace CardsLibrary
{
    /// <summary>
    /// This Class Represents a standard playing card.
    /// </summary>
    [DataContract]
    public class Card
    {
        /// <summary>
        /// An enum that holds the suits type as well as its colour.
        /// </summary>
        public enum SuitID
        {
            Clubs, Diamonds, Hearts, Spades, Red, Black
        };

        /// <summary>
        /// An enum that holds the suits numerical value.
        /// </summary>
        public enum RankID
        {
            Ace, Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Joker
        };

        /// <summary>
        /// Suit Property
        /// </summary>
        /// <value>
        /// Grabs the SuitID enum.
        /// </value>
        /// <seealso cref="SuitID"/>
        [DataMember]
        public SuitID Suit { get; private set; }

        /// <summary>
        /// Rank Property
        /// </summary>
        /// <value>
        /// Grabs the RankID enum.
        /// </value>
        /// <seealso cref="RankID"/>
        [DataMember]
        public RankID Rank { get; private set; }

        /// <summary>
        /// Name Property
        /// </summary>
        /// <returns>
        /// Returns the suit and rank of the card to create the name.
        /// </returns>
        [DataMember]
        public String Name
        {
            get
            {
                if (Rank == RankID.Joker)
                    return Suit.ToString() + " " + Rank.ToString();
                return Rank.ToString() + " of " + Suit.ToString();
            }
            set { }
        }

        /// <summary>
        /// ImageName Property
        /// </summary>
        /// <returns>
        /// Returns the suit and rank of the card to get the correct picture, based on the pictures file name.
        /// </returns>
        [DataMember]
        public String ImageName
        {
            get
            {
                if (Rank == RankID.Joker)
                    return Suit.ToString() + "_" + Rank.ToString() + ".png";
                return Rank.ToString() + "_of_" + Suit.ToString() + ".png";
            }
            set { }
        }


        /// <summary>
        /// isMatched Property
        /// </summary>
        /// <value>
        /// Boolean value to check if there is a match.
        /// </value>
        [DataMember]
        public bool isMatched { get; set; }

        /// <summary>
        /// isFlipped Property
        /// </summary>
        /// <value>
        /// Boolean value to check if the card is flipped.
        /// </value>
        [DataMember]
        public bool isFlipped { get; set; }

        /// <summary>
        /// Card two arg constructor.
        /// </summary>
        /// <param name="s">Takes in a suit enum</param>
        /// <param name="r">Takes in a rank enum</param>
        public Card(SuitID s, RankID r)
        {
            Suit = s;
            Rank = r;
        }
    }
}
