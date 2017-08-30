/*
 * Program:         CardsLibrary.dll
 * Module:          Deck.cs
 * Date:            April 2, 2017
 * Author:          Justin Bonello & Marcus Baldassarre
 * Description:     Defines the Deck that manages a collection of Card 
 *                  objects.
 * Status:          Complete.
 */

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CardsLibrary
{
    /// <summary>
    /// Defines the Deck that manages a collection of Card objects.
    /// </summary>
    [DataContract]
    public class Deck
    {
        /// <summary>
        /// A Dictionary of the cards with key value pairs.
        /// </summary>
        [DataMember]
        public Dictionary<string, Card> cards { get; set; }

        private List<string> keys = new List<string>();

        /// <summary>
        /// Deck default constructor
        /// </summary>
        /// <seealso cref="Shuffle()"/>
        public Deck()
        {
            Shuffle();
        }

        /// <summary>
        /// NumCards Property
        /// </summary>
        /// <returns>
        /// Returns the card count.
        /// </returns>
        public int NumCards
        {
            get
            {
                return cards.Count;
            }
        }

        /// <summary>
        /// Shuffles the Cards.
        /// </summary>
        /// <seealso cref="populateKeyList()"/>
        /// <seealso cref="shuffleKeys()"/>
        /// <seealso cref="repopulate()"/>
        public void Shuffle()
        {
            populateKeyList();
            shuffleKeys();
            repopulate();
        }

        /// <summary>
        /// Creates keys for the cards.
        /// </summary>
        private void populateKeyList()
        {
            keys.Clear();
            for (int i = 1; i <= 6; ++i)
                for (char j = 'A'; j <= 'I'; ++j)
                    keys.Add(j.ToString() + i.ToString());
        }

        /// <summary>
        /// Suffles the keys.
        /// </summary>
        private void shuffleKeys()
        {
            Random rng = new Random();
            string temp;
            for (int i = 0; i < 54; i++)
            {
                // Choose a random index
                int randIdx = rng.Next(54);
                if (randIdx != i)
                {
                    temp = keys[i];
                    keys[i] = keys[randIdx];
                    keys[randIdx] = temp;
                }
            }
        }
     
        /// <summary>
        /// Populates a dicionary of card objects with shuffled key value pairs.
        /// </summary>
        private void repopulate()
        {
            int currentIndex = 0;
            cards = new Dictionary<string, Card>();

            foreach (Card.SuitID s in Enum.GetValues(typeof(Card.SuitID)))
            {
                if(s != Card.SuitID.Red && s != Card.SuitID.Black)
                {
                    foreach (Card.RankID r in Enum.GetValues(typeof(Card.RankID)))
                    {
                        if (r != Card.RankID.Joker)
                        {                         
                            cards.Add(keys[currentIndex], new Card(s, r));
                            ++currentIndex;
                        }                          
                    }
                }
                    
            }

            cards.Add(keys[currentIndex], new Card(Card.SuitID.Red, Card.RankID.Joker));
            cards.Add(keys[++currentIndex], new Card(Card.SuitID.Black, Card.RankID.Joker));
        }
    }
}
