/*
 * Program:         MemoryCardGame.exe
 * Module:          MainWindow.xaml.cs
 * Date:            April 2, 2017
 * Author:          Justin Bonello & Marcus Baldassarre
 * Description:     The back end of the card game
 * Status:          Complete.
 */

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ServiceModel;
using CardsLibrary;
using System.Threading;

namespace MemoryCardGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class MainWindow : Window, IClientCallback
    {
        /// <summary>
        /// 
        /// </summary>
        /// <value>
        /// The popup message.
        /// </value>
        public string msg
        {
            get { return (string)GetValue(msgProp); }
            set { SetValue(msgProp, value); }
        }

        /// <summary>
        /// Message Property
        /// </summary>
        /// <value>
        /// Holds the value for the message
        /// </value>
        public static readonly DependencyProperty msgProp =
            DependencyProperty.Register("msg", typeof(string), typeof(MainWindow), new PropertyMetadata(""));

        private delegate void UIUpdateDelegate(int id);

        private IGameState state = null;
        private int playerId;
        /// <summary>
        /// The Main Window
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            try
            {
                DuplexChannelFactory<IGameState> gameStateFactory =
                    new DuplexChannelFactory<IGameState>(this, "GameState");
                state = gameStateFactory.CreateChannel();

                // Join the game
                playerId = state.RegisterPlayer("");

                // If the game is full, exit
                if (playerId < 0)
                {
                    MessageBox.Show(
                        "Game is full. Try joining again later.",
                        "Game Full",
                        MessageBoxButton.OK,
                        MessageBoxImage.Exclamation);
                    Close();
                }

                mainWindow.Title = "The Game of Memory -- Player " + (playerId + 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error Message", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CardClicked(object sender, MouseButtonEventArgs e)
        {
            try
            {
                Image cardImg = (Image)sender;
                if (state.Players[playerId].HasTurn == false)
                    MessageBox.Show("Please wait your turn.", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (state.CardDeck.cards[cardImg.Name].isFlipped)
                    MessageBox.Show("You can't select the same card twice.", "Warning Message", MessageBoxButton.OK, MessageBoxImage.Warning);
                else if (state.MoveCount < 2 && !state.CardDeck.cards[cardImg.Name].isMatched)
                    state.FlipCard(cardImg.Name);
            }
            catch{ }
        }

        /// <summary>
        /// This is used to update the UI to let the player know information about the Game.
        /// </summary>
        /// <param name="id">Takes in the players ID</param>
        public void UpdateUI(int id)
        {
            try
            {
                if (Dispatcher.Thread == Thread.CurrentThread)
                {
                    playerId = id;
                    msg = state.Message;

                    player1.Visibility = Visibility.Hidden;
                    player2.Visibility = Visibility.Hidden;
                    player3.Visibility = Visibility.Hidden;
                    player4.Visibility = Visibility.Hidden;

                    foreach (var p in state.Players)
                    {
                        Grid playerGrid = (Grid)container.FindName("player" + (p.PlayerIndex));
                        Label playerScore = (Label)container.FindName("p" + (p.PlayerIndex) + "Score");
                        playerScore.Content = "Score: " + p.Score;
                        if (p.HasTurn == true)
                            playerGrid.Background = Brushes.LightGreen;
                        else
                            playerGrid.Background = Brushes.WhiteSmoke;
                        playerGrid.Visibility = Visibility.Visible;
                    }

                    Dictionary<string, Card> deck = state.CardDeck.cards;
                    foreach (var c in deck)
                    {
                        Image cardImg = (Image)container.FindName(c.Key);
                        if (c.Value.isMatched)
                            cardImg.Visibility = Visibility.Hidden;
                        if (c.Value.isFlipped)
                            cardImg.Source = new BitmapImage(new Uri("../img/" + c.Value.ImageName, UriKind.Relative));
                        else
                            cardImg.Source = new BitmapImage(new Uri("../img/back.png", UriKind.Relative));
                    }

                    if (!state.IsGameOver() && state.Players[playerId].HasTurn == true && state.MoveCount == 2)
                        endTurn.IsEnabled = true;
                    else
                        endTurn.IsEnabled = false;

                    if (state.IsGameOver())
                        gameOver.Visibility = Visibility.Visible;
                    else
                        gameOver.Visibility = Visibility.Hidden;
                }
                else
                {
                    // Call asynchronously
                    Dispatcher.BeginInvoke(new UIUpdateDelegate(UpdateUI), id);
                }
            }
            catch { }
            
        }

        /// <summary>
        /// Called when the Game is closed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                if (playerId >= 0)
                    state.LeaveGame(playerId);
            }
            catch { }
        }

        /// <summary>
        /// Once their is a click move on and update everything.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void continue_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                state.QueueNextTurn();
                state.NotifyClients();
            }
            catch { }
        }
    }
}
