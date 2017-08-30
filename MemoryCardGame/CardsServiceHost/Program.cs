/*
 * Program:         CardsServiceHost.exe
 * Module:          Program.cs
 * Date:            April 2, 2017
 * Author:          Justin Bonello & Marcus Baldassarre
 * Description:     The main program for running the service.
 * Status:          Complete.
 */

using System;
using System.ServiceModel;  // WCF types
using CardsLibrary;         // IShoe, Shoe types

namespace CardsServiceHost
{
    /// <summary>
    /// The main program for running the service.
    /// </summary>
    class Program
    {
        /// <summary>
        /// The Main
        /// </summary>
        /// <param name="args">Args from cmd.</param>
        static void Main(string[] args)
        {
            ServiceHost host = null;

            try
            {
                // Create a service host object
                host = new ServiceHost(typeof(GameState));

                // Start the service
                host.Open();
                Console.WriteLine("Service started. Press a key to quit.");
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // Wait for keystroke then shut down
                Console.ReadKey();
                if (host != null)
                    host.Close();
            }
        }
    }
}
