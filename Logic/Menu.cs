using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Text.Json;
using System.Windows.Media;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using JsonSerializer = System.Text.Json.JsonSerializer;
using System.IO;

namespace Logic
{
    /*
     This class is in charge of the menu's functions.
    it has a path for a saving file
    duplicates of the canvas, the list of bots and the player
    functions' pointers
    colors for the written inside.
     */
    internal class Menu
    {
        protected static string path = @"D:\Visual Studio\Projects\CopsAndThieves\source\SavedGame.txt";
        protected static Dora dora; //dora duplicate
        protected static List<Tuple<int, int>> coor = new List<Tuple<int, int>>(); //list of coordinates to write in to json file
        protected Action FuncNewGame; //InitiateRandomBoard from logic
        protected Action FuncLoadGame;//InitiateSavedBoard from logic
        protected Action run;//Run from logic
        protected static SolidColorBrush b = new SolidColorBrush(Colors.Transparent); //menu's background
        protected static SolidColorBrush f = new SolidColorBrush(Colors.Cyan); //menu's foreground
        protected static List<Thief> thiefList; //thieves duplicate
        protected Canvas canvas; //canvas duplicate

        //Creating a stackpanel for the menu
        public static StackPanel sp = new StackPanel
        {
            Background = b,
            Height = 600,
            Width = 200,
        };

        //Textblock with directions
        public TextBlock Text = new TextBlock
        {
            Foreground = f,
            FontSize = 30,
            Background = b,
            Height = 100,
            Width = 200,
            Text = "Choose Option:"
        };

        //The load button definition
        public Button Load = new Button
        {
            Foreground = f,
            FontSize = 50,
            Background = b,
            Height = 100,
            Width = 200,
            Content = "Load",
        };

        //The save button definition
        public Button Save = new Button
        {
            Foreground = f,
            FontSize = 50,
            Background = b,
            Height = 100,
            Width = 200,
            Content = "Save"
        };

        //The quit button definition
        public Button Quit = new Button
        {
            Foreground = f,
            FontSize = 50,
            Background = b,
            Height = 100,
            Width = 200,
            Content = "Quit",
        };

        //The new button definition
        public Button New = new Button
        {
            Foreground = f,
            FontSize = 30,
            Background = b,
            Height = 100,
            Width = 200,
            Content = "New Game"
        };

        //The back button definition
        public Button Back = new Button
        {
            Foreground = f,
            FontSize = 50,
            Background = b,
            Height = 100,
            Width = 200,
            Content = "Resume"
        };

        //creating the menu and recieving from the logic
        //Canvas, list of bots, player, InitiateRandomBoard, InitiateSavedBoard and Run function
        public Menu(Canvas canvas, List<Thief> thieves, Action NewFunc, Action LoadFunc, Action Run, Dora d)
        {
            dora = d;
            thiefList = thieves;
            this.canvas = canvas;
            this.run = Run;
            this.FuncLoadGame = LoadFunc;
            this.FuncNewGame = NewFunc;
            //adding the buttons to the stackpanel menu
            sp.Children.Add(Text);
            sp.Children.Add(New);
            sp.Children.Add(Save);
            sp.Children.Add(Load);
            sp.Children.Add(Back);
            sp.Children.Add(Quit);
            //adding the buttons events
            Quit.Click += QuitGame;
            Back.Click += BackToGame;
            Save.Click += SaveGame;
            New.Click += this.NewGame;
            Load.Click += this.LoadGame;
        }

        //Sending the bots list and the player to the logic
        //So the InitiateSavedBoard will create from it the last saved game
        public List<Thief> GetThiefList() => thiefList;
        public Dora GetSavedDora() => dora;

        // The load game function
        public void LoadGame(object sender, RoutedEventArgs e)
        {
            Tuple<int, int> t; //The players coordinates
            thiefList.Clear();//clearing the memory
            if (coor != null) coor.Clear();
            dora = null;
            string temp; //string to read from the file
            temp = File.ReadAllText(path); 
            coor = JsonConvert.DeserializeObject<List<Tuple<int,int>>>(temp); //reading and converting from json string to the list of tuples
            if (coor == null) //if there is no save do not continue building the board
            {
                MessageBox.Show("No Saved Data", "ERROR");
                return;
            }
            t = coor[0];
            dora = new Dora(t.Item1, t.Item2); //create the player and remove it from the bot list.
            coor.Remove(t);
            CreateThieves();
            this.FuncLoadGame(); //InitiateSavedBoard
        }

        //saving the game into json file
        public void SaveGame(object sender, RoutedEventArgs e)
        {
            //if there is no data to save do not continue
            if (dora == null) return;
            //creating the json string list to save into the file
            CreateCoorList();
            //convetring the list to json string
            string temp = JsonConvert.SerializeObject(coor, Formatting.Indented);
            //recreate the file
            File.Delete(path);
            File.WriteAllText(path, temp);
        }

        //update the list of the bots and the player
        public void UpdateList(List<Thief> l, Dora doraUpdate)
        {
            dora = doraUpdate;
            thiefList = l;
        }

        //creating new game
        public void NewGame (object sender, RoutedEventArgs e)
        {
            //calling InitiateRandomBoard
            this.FuncNewGame();
        }

        //shutting down the game
        public void QuitGame(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        //close the menu and checking if the game is not over continue
        public void BackToGame(object sender, RoutedEventArgs e)
        {
            Collapse();
            if (!(thiefList.Count < 2)) run();
        }

        //creating thiefList
        public void CreateThieves()
        {
            //converting the coordinate tuple into bots objects
            thiefList.Clear();
            foreach(Tuple<int,int> t in coor)
            {
                thiefList.Add(new Thief(t.Item1, t.Item2));
            }
        }

        public void CreateCoorList()
        {
            //creating the coordinates list with the played as first coordinate
            //so the function LoadGame will save it as a player and then remove from the list
            if (coor != null) coor.Clear();
            coor.Add(Tuple.Create(dora.getx_pos(), dora.gety_pos()));
            //creating the bots list
            foreach (Thief thief in thiefList)
            {
                coor.Add(Tuple.Create(thief.getx_pos(), thief.gety_pos()));
            }
        }

        //present the menu
        public void Present()
        {
            canvas.Children.Add(sp);
        }

        //remove the menu
        public void Collapse()
        {
            canvas.Children.Remove(sp);
        }

    }
}
