﻿using System;
using System.IO;
using System.Xml;

namespace EvilCar
{

    class Program
    {
        public const string PROFILES_FILENAME = "Profiles.xml";

        // in der Main soll die ganze Menünavigation laufen. Ob mit tasten oder mit befehlen ist mir egal...
        static void Main(string[] args)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(new StreamReader(PROFILES_FILENAME).ReadToEnd());
            Database db = Database.buildFromXMl(doc);

            while (true)
            {
                Console.WriteLine("Welcome to the EvilCar console.");
                Console.WriteLine("You need to authenticate yourself to get access.");
                Console.WriteLine("Please enter your username...");
                string username = Console.ReadLine();
                if (!db.checkUsername(username))
                {
                    Console.WriteLine("There is no such user...");
                    continue;
                }
                else
                {
                    Console.WriteLine("Please insert your password...");
                }
                string password = Console.ReadLine();
                
                if (db.checkCredentials(username, password))
                {
                    User profile = db.getUser(username);
                    Console.WriteLine($"Hello {profile.name}! You are now logged in.");

                    //the main menue
                    while (true)
                    {
                        Console.WriteLine("Enter the command you want to execute. If you don't know them, enter help or ?");

                        //TODO Commands Zeug
                        string[] command_args = Console.ReadLine().Split(' ');
                        if(command_args.Length > 0)
                        {
                            switch (command_args[0])
                            {
                                case "help":
                                case "?":

                                    break;
                                default:
                                    Console.WriteLine("There is no such command!");
                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("There was an error with your credentials. Please try again.\n");
                }
            }
        }
    }
}
