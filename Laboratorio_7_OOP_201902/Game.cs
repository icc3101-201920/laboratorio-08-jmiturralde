﻿using Laboratorio_7_OOP_201902.Cards;
using Laboratorio_7_OOP_201902.Enums;
using Laboratorio_7_OOP_201902.Static;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;

namespace Laboratorio_7_OOP_201902
{
    public class Game
    {
        //Constantes
        private const int DEFAULT_CHANGE_CARDS_NUMBER = 3;

        //Atributos
        private Player[] players;
        private Player activePlayer;
        private List<Deck> decks;
        private List<SpecialCard> captains;
        private Board boardGame;
        internal int turn;

        //Constructor
        public Game()
        {
            Random random = new Random();
            decks = new List<Deck>();
            captains = new List<SpecialCard>();
            AddDecks();
            AddCaptains();
            players = new Player[2] { new Player(), new Player() };
            ActivePlayer = Players[random.Next(2)];
            boardGame = new Board();
            //Add board to players
            players[0].Board = boardGame;
            players[1].Board = boardGame;
            turn = 0;
        }
        //Propiedades
        public Player[] Players
        {
            get
            {
                return this.players;
            }
        }
        public Player ActivePlayer
        {
            get
            {
                return this.activePlayer;
            }
            set
            {
                activePlayer = value;
            }
        }
        public List<Deck> Decks
        {
            get
            {
                return this.decks;
            }
        }
        public List<SpecialCard> Captains
        {
            get
            {
                return this.captains;
            }
        }
        public Board BoardGame
        {
            get
            {
                return this.boardGame;
            }
        }


        //Metodos
        public bool CheckIfEndGame()
        {
            if (players[0].LifePoints == 0 || players[1].LifePoints == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public int GetWinner()
        {
            if (players[0].LifePoints == 0 && players[1].LifePoints > 0)
            {
                return 1;
            }
            else if (players[1].LifePoints == 0 && players[0].LifePoints > 0)
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }
        public int GetRoundWinner()
        {
            if (Players[0].GetAttackPoints()[0] == Players[1].GetAttackPoints()[0])
            {
                return -1;
            }
            else
            {
                int winner = Players[0].GetAttackPoints()[0] > Players[1].GetAttackPoints()[0] ? 0 : 1;
                return winner;
            }
        }
        public void Play()
        {
            int userInput = 0;
            int firstOrSecondUser = ActivePlayer.Id == 0 ? 0 : 1;
            int winner = -1;
            bool bothPlayersPlayed = false;
            List<string> saved = new List<string>();
            saved.Add("Yes");
            saved.Add("No");
            

            while (turn < 4 && !CheckIfEndGame())
            {
                bool drawCard = false;
                //turno 0 o configuracion
                if (turn == 0)
                {
                    int uploadField = 0;

                    //Cargar partida despues de turno 0
                    Visualization.ShowListOptions(saved, "Do you wish to ulpoad an existing game?");
                    int answer = Visualization.GetUserInput(1);
                    string curFile = @"C:\Users\Yiyo\Desktop\laboratorio-08-jmiturralde\Laboratorio_7_OOP_201902\bin\Debug\netcoreapp2.1\Player1Captain.bin";
                    string curFile1 = @"C:\Users\Yiyo\Desktop\laboratorio-08-jmiturralde\Laboratorio_7_OOP_201902\bin\Debug\netcoreapp2.1\Player1Cards.bin";
                    string curFile2 = @"C:\Users\Yiyo\Desktop\laboratorio-08-jmiturralde\Laboratorio_7_OOP_201902\bin\Debug\netcoreapp2.1\Player2Captain.bin";
                    string curFile3 = @"C:\Users\Yiyo\Desktop\laboratorio-08-jmiturralde\Laboratorio_7_OOP_201902\bin\Debug\netcoreapp2.1\Player2Cards.bin";
                    if (answer == 0 && File.Exists(curFile) && File.Exists(curFile1) && File.Exists(curFile2) && File.Exists(curFile3) )
                    {
                        Console.Clear();
                        IFormatter formatter1 = new BinaryFormatter();
                        Stream streamm = new FileStream("Player1Cards.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                        Stream streamm1 = new FileStream("Player1Captain.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                        Stream streamm2 = new FileStream("Player2Cards.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                        Stream streamm3 = new FileStream("Player2Captain.bin", FileMode.Open, FileAccess.Read, FileShare.Read);
                        Hand cards1 = (Hand)formatter1.Deserialize(streamm);
                        SpecialCard captain1 = (SpecialCard)formatter1.Deserialize(streamm1);
                        Hand cards2 = (Hand)formatter1.Deserialize(streamm2);
                        SpecialCard captain2 = (SpecialCard)formatter1.Deserialize(streamm3);
                        streamm.Close();
                        streamm1.Close();
                        streamm2.Close();
                        streamm3.Close();


                        Players[0].Hand = cards1;
                        Players[0].Captain = captain1;
                        Players[1].Hand = cards2;
                        Players[1].Captain = captain2;

                        Deck deck1 = new Deck();
                        deck1.Cards = new List<Card>(Players[0].Hand.Cards);
                        Players[0].Deck = deck1;
                        Players[0].ChooseCaptainCard(new SpecialCard(Players[0].Captain.Name, Players[0].Captain.Type, Players[0].Captain.Effect));

                        Deck deck2 = new Deck();
                        deck2.Cards = new List<Card>(Players[1].Hand.Cards);
                        Players[1].Deck = deck2;
                        Players[1].ChooseCaptainCard(new SpecialCard(Players[1].Captain.Name, Players[1].Captain.Type, Players[1].Captain.Effect));



                        Console.WriteLine("INFORMATION OF PLAYER 1");
                        Visualization.ShowHand(Players[0].Hand);
                        Console.WriteLine("");
                        Console.WriteLine($"Player captain: {Players[0].Captain.Name}");
                        Console.WriteLine("");
                        Console.WriteLine("INFORMATION OF PLAYER 2");
                        Visualization.ShowHand(Players[1].Hand);
                        Console.WriteLine("");
                        Console.WriteLine($"Player captain: {Players[1].Captain.Name}");
                        Console.WriteLine("");
                        Console.WriteLine("Press any key to start playing");
                        Console.ReadKey();
                        uploadField = 1;
                    }
                    else
                    {
                        if (answer == 0)
                        {
                            Visualization.ConsoleError("There was no infomation of a saved game to be use");
                            Thread.Sleep(2500);
                            Console.Clear();
                            Visualization.ShowProgramMessage("Players get ready to play");
                            Thread.Sleep(2500);
                            Console.Clear();
                        }
                        else
                        {
                            Visualization.ShowProgramMessage("Players get ready to play");
                            Thread.Sleep(2500);
                            Console.Clear();
                        }
                    }

                    if (uploadField != 1)
                    {
                        for (int _ = 0; _ < Players.Length; _++)
                        {
                            ActivePlayer = Players[firstOrSecondUser];
                            Visualization.ClearConsole();
                            //Mostrar mensaje de inicio
                            Visualization.ShowProgramMessage($"Player {ActivePlayer.Id + 1} select Deck and Captain:");
                            //Preguntar por deck
                            Visualization.ShowDecks(this.Decks);
                            userInput = Visualization.GetUserInput(this.Decks.Count - 1);
                            Deck deck = new Deck();
                            deck.Cards = new List<Card>(Decks[userInput].Cards);
                            ActivePlayer.Deck = deck;
                            //Preguntar por capitan
                            Visualization.ShowCaptains(Captains);
                            userInput = Visualization.GetUserInput(this.Captains.Count - 1);
                            ActivePlayer.ChooseCaptainCard(new SpecialCard(Captains[userInput].Name, Captains[userInput].Type, Captains[userInput].Effect));
                            //Asignar mano
                            ActivePlayer.FirstHand();
                            //Mostrar mano
                            Visualization.ShowHand(ActivePlayer.Hand);
                            //Mostar opciones, cambiar carta o pasar
                            Visualization.ShowListOptions(new List<string>() { "Change Card", "Pass" }, "Change 3 cards or ready to play:");
                            userInput = Visualization.GetUserInput(1);
                            if (userInput == 0)
                            {
                                Visualization.ClearConsole();
                                Visualization.ShowProgramMessage($"Player {ActivePlayer.Id + 1} change cards:");
                                Visualization.ShowHand(ActivePlayer.Hand);
                                for (int i = 0; i < DEFAULT_CHANGE_CARDS_NUMBER; i++)
                                {
                                    Visualization.ShowProgramMessage($"Input the numbers of the cards to change (max {DEFAULT_CHANGE_CARDS_NUMBER}). To stop enter -1");
                                    userInput = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                                    if (userInput == -1) break;
                                    ActivePlayer.ChangeCard(userInput);
                                    Visualization.ShowHand(ActivePlayer.Hand);
                                }
                            }
                            firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                        }
                    }
                    turn += 1;

                    //Guardado de partida desues de turno 0
                    IFormatter formatter = new BinaryFormatter();
                    Stream stream = new FileStream("Player1Cards.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                    Stream stream1 = new FileStream("Player1Captain.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                    Stream stream2 = new FileStream("Player2Cards.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                    Stream stream3 = new FileStream("Player2Captain.bin", FileMode.Create, FileAccess.Write, FileShare.None);
                    formatter.Serialize(stream, Players[0].Hand);
                    formatter.Serialize(stream1, Players[0].Captain);
                    formatter.Serialize(stream2, Players[1].Hand);
                    formatter.Serialize(stream3, Players[1].Captain);
                    stream.Close();
                    stream1.Close();
                    stream2.Close();
                    stream3.Close();
                }

                //turnos siguientes
                else
                {
                    while (true)
                    {
                        ActivePlayer = Players[firstOrSecondUser];
                        //Obtener lifePoints
                        int[] lifePoints = new int[2] { Players[0].LifePoints, Players[1].LifePoints };
                        //Obtener total de ataque:
                        int[] attackPoints = new int[2] { Players[0].GetAttackPoints()[0], Players[1].GetAttackPoints()[0] };
                        //Mostrar tablero, mano y solicitar jugada
                        Visualization.ClearConsole();
                        Visualization.ShowBoard(boardGame, ActivePlayer.Id, lifePoints,attackPoints);
                        //Robar carta
                        if (!drawCard)
                        {
                            ActivePlayer.DrawCard();
                            drawCard = true;
                        }
                        Visualization.ShowHand(ActivePlayer.Hand);
                        Visualization.ShowListOptions(new List<string> { "Play Card", "Pass" }, $"Make your move player {ActivePlayer.Id+1}:");
                        userInput = Visualization.GetUserInput(1);
                        if (userInput == 0)
                        {
                            //Si la carta es un buff solicitar a la fila que va.
                            Visualization.ShowProgramMessage($"Input the number of the card to play. To cancel enter -1");
                            userInput = Visualization.GetUserInput(ActivePlayer.Hand.Cards.Count, true);
                            if (userInput != -1)
                            {
                                if (ActivePlayer.Hand.Cards[userInput].Type == EnumType.buff)
                                {
                                    Visualization.ShowListOptions(new List<string> { "Melee", "Range", "LongRange" }, $"Choose row to buff {ActivePlayer.Id}:");
                                    int cardId = userInput;
                                    userInput = Visualization.GetUserInput(2);
                                    if (userInput == 0)
                                    {
                                        ActivePlayer.PlayCard(cardId, EnumType.buffmelee);
                                    }
                                    else if (userInput == 1)
                                    {
                                        ActivePlayer.PlayCard(cardId, EnumType.buffrange);
                                    }
                                    else
                                    {
                                        ActivePlayer.PlayCard(cardId, EnumType.bufflongRange);
                                    }
                                }
                                else
                                {
                                    ActivePlayer.PlayCard(userInput);
                                }
                            }
                            //Revisar si le quedan cartas, si no le quedan obligar a pasar.
                            if (ActivePlayer.Hand.Cards.Count == 0)
                            {
                                firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                                break;
                            }
                        }
                        else
                        {
                            firstOrSecondUser = ActivePlayer.Id == 0 ? 1 : 0;
                            break;
                        }
                    }
                    //Cambiar al oponente si no ha jugado
                    if (!bothPlayersPlayed)
                    {
                        bothPlayersPlayed = true;
                        drawCard = false;
                    }
                    //Si ambos jugaron obtener el ganador de la ronda, reiniciar el tablero y pasar de turno.
                    else
                    {
                        winner = GetRoundWinner();
                        if (winner == 0)
                        {
                            Players[1].LifePoints -= 1;
                        }
                        else if (winner == 1)
                        {
                            Players[0].LifePoints -= 1;
                        }
                        else
                        {
                            Players[0].LifePoints -= 1;
                            Players[1].LifePoints -= 1;
                        }
                        bothPlayersPlayed = false;
                        turn += 1;
                        //Destruir Cartas
                        BoardGame.DestroyCards();
                    }
                }
            }
            //Definir cual es el ganador.
            winner = GetWinner();
            if (winner == 0)
            {
                Visualization.ShowProgramMessage($"Player 1 is the winner!");
            }
            else if (winner == 1)
            {
                Visualization.ShowProgramMessage($"Player 2 is the winner!");
            }
            else
            {
                Visualization.ShowProgramMessage($"Draw!");
            }

        }
        public void AddDecks()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + @"\Files\Decks.txt";
            StreamReader reader = new StreamReader(path);
            int deckCounter = 0;
            List<Card> cards = new List<Card>();


            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string [] cardDetails = line.Split(",");

                if (cardDetails[0] == "END")
                {
                    decks[deckCounter].Cards = new List<Card>(cards);
                    deckCounter += 1;
                }
                else
                {
                    if (cardDetails[0] != "START")
                    {
                        if (cardDetails[0] == nameof(CombatCard))
                        {
                            cards.Add(new CombatCard(cardDetails[1], (EnumType) Enum.Parse(typeof(EnumType),cardDetails[2]), cardDetails[3], Int32.Parse(cardDetails[4]), bool.Parse(cardDetails[5])));
                        }
                        else
                        {
                            cards.Add(new SpecialCard(cardDetails[1], (EnumType)Enum.Parse(typeof(EnumType), cardDetails[2]), cardDetails[3]));
                        }
                    }
                    else
                    {
                        decks.Add(new Deck());
                        cards = new List<Card>();
                    }
                }

            }
            
        }
        public void AddCaptains()
        {
            string path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent + @"\Files\Captains.txt";
            StreamReader reader = new StreamReader(path);
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] cardDetails = line.Split(",");
                captains.Add(new SpecialCard(cardDetails[1], (EnumType)Enum.Parse(typeof(EnumType), cardDetails[2]), cardDetails[3]));
            }
        }
    }
}
