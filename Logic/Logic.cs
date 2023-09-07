using System;
using System.Collections.Generic;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xaml;

namespace Logic
{
    //This is the logic of the game
    public class logic
    {

        public Canvas canvas;//canvas - the gameboard
        private List<Thief> thieves;//the list of the bots
        private Menu menu;
        private bool menuUp;//helps cheking if the menu is presented;
        private Dora dora;//the player itself
        public int maxWidth = 1500, maxHeight = 800;//the board's sizes
        private static readonly DispatcherTimer t = new();//the timer for the bots movement

        //creating the logic
        public logic()
        {
            //creating canvas
            this.canvas = new Canvas();
            canvas.Background = new ImageBrush { ImageSource = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@"source\Background.jpg"), UriKind.Absolute)) };
            canvas.Height = maxHeight;
            canvas.Width = maxWidth;
            //creating bots list
            this.thieves = new List<Thief>();
            //creating menu
            menu = new Menu(canvas, thieves, InitiateRandomBoard, InitiateSavedBoard, Run, dora);
            //defining the timer
            t.Interval = TimeSpan.FromMilliseconds(20);
            t.Tick += TimeToGo;
        }

        //every 20 milliseconds the timer will call this function
        //this function moves the bots and checks of the player won/lost
        public void TimeToGo(object sender, EventArgs e)
        {
            if (thieves != null)
            {
                MoveThieves();
                CheckCollision();
            }
            LostGame();
        }

        //starting the timer after a break when the menu is up/the game is over
        public static void Run()
        {
            t.Start();
        }

        //stopping the timer when there is a break/the game is over
        public static void Stop()
        {
            t.Stop();
        }

        //initiating the last saved board.
        //getting from the menu the player and the list of bots and clearing the board.
        //present the bot list and the player
        public void InitiateSavedBoard()
        {
            dora = menu.GetSavedDora();
            thieves = menu.GetThiefList();
            if (dora == null || thieves == null) return;
            canvas.Children.Clear();
            //as long as the game is not over show the objects on the canvas
            if (thieves.Count > 1)
            {
                foreach(Thief thief in thieves)
                {
                    thief.GetRect(canvas);
                }
                dora.GetDora(canvas);
                Run();
            }
        }

        /*
         Initiating the new game.
        random unoverlapping positions are generated and sent to
        the bot's builder and creating the bots.
         */
        public void InitiateRandomBoard()
        {
            //clearing variables
            Tuple<int, int> t;
            canvas.Children.Clear();
            thieves.Clear();
            //generate first bot so the overlap check will occur
            t = GenerateLocation();
            thieves.Add(new Thief(t.Item1, t.Item2));
            //generating another 9 bots and checking overlapping
            //when the coordinates are alright add a new bot
            for(int i = 0; i < 9; i++)
            {
                t = GetLocation();
                thieves.Add(new Thief(t.Item1, t.Item2));
            }
            //presenting the bots
            foreach (Thief thief in thieves)
            {
                thief.GetRect(canvas);
            }
            //creating new player
            t = GetLocation();
            dora = new Dora(t.Item1, t.Item2);
            this.dora.GetDora(canvas);
            //updating the duplicates in the menu class incase of saving.
            menu.UpdateList(thieves, dora);
            Run();
        }

        /*
         generating location and checking if it is a valid one,
        if not regenerate and check again.
        return valid coordinates.
         */
        public Tuple<int, int> GetLocation()
        {
            Tuple<int, int> t;
            bool ans;
            do
            {
                //generate random location
                t = GenerateLocation();
                //checking the location validation
                ans = CheckThiefsLocation(t.Item1, t.Item2);
            } while (!ans);
            return t;
        }
        
        /*
         checking if a generated location is valid.
        if the locations are overlapping return false, else true.
         */
        public bool CheckThiefsLocation(int x_pos, int y_pos)
        {
            bool ans;
            //checking all the bots that were created are not being overlapped by the new bot
            foreach (Thief thief in thieves)
            {
                //the check function
                ans = Thief.CheckValidation(thief, x_pos, y_pos);
                if(!ans)
                {
                    return ans; 
                }
            }
            return true;
        }

        //Generating random coordinates within the borders of the board and returning it as a tuple
        public Tuple<int, int> GenerateLocation()
        {
            Random random = new();
            int random_x = random.Next(0, maxWidth - Thief.thiefs_width);
            int random_y = random.Next(0, maxHeight - Thief.thiefs_height);
            return Tuple.Create(random_x, random_y);
        }

       
        /*
         Whenever a key is being pressed the function will recieve the key
         */
        public void KeyEntered(KeyEventArgs e)
        {
            //if the key is Escape call menu function
            if (e.Key == Key.Escape) CallMenu();
            //if the game is over do not recieve the other keys
            if (dora == null) return;
            //the keys are compatible to the movement of the player
            switch (e.Key)
            {
                //up - move up/down - move down/left - move left/right - move right
                case Key.Up: dora.MoveUp(); break;
                case Key.Down: dora.MoveDown(); break;
                case Key.Left: dora.MoveLeft(); break;
                case Key.Right: dora.MoveRight(); break;
            }
            //update the menu incase of saving the board
            menu.UpdateList(thieves, dora);
        }

        //the menu function - if the menu is up collapse it and if its down present it
        public void CallMenu()
        {
            //stop the game
            Stop();
            if (!menuUp)
            {   
                //if the menu is not presented, present and change the menuUp to true
                menu.Present();
                menuUp = true;
            }
            else
            {
                //if the menu is presented remove it and change the menuUp to false
                menu.Collapse();
                menuUp = false;
                if(thieves != null)
                {
                    //if the game is not over continue it
                    Run();
                }
            }
        }

        //move the thieves and check if while the movement the player collided with a bot
        public void MoveThieves()
        {
            //
            foreach (Thief thief in thieves)
            {
                if (thief.getx_pos() > dora.getx_pos()) thief.MoveLeft();
                
                else thief.MoveRight();

                if (thief.gety_pos() > dora.gety_pos()) thief.MoveUp();

                else thief.MoveDown();
              
            }
            //check the bots collisions
            CheckCollision();  
        }


        /*
         Checking if 2 bots are overlapping and if yes removing one
         */
        public void CheckCollision()
        {
            List<Thief> deleteList = new();
            //this counter is in charge of not making the same check twice
            int i = 0;
            //creating 2 loop. 1 loop has the checked bot and the 2 loop will check if the other bots are overlapping him
            foreach (Thief outhief in thieves)
            {
                //helps the i counter
                int j = 0;
                //the inner loop
                foreach (Thief inthief in thieves)
                {

                    j++;
                    //checking if the counters are pointing that the same check is not occured twiced
                    if (j > i && i != thieves.Count - 1)
                    {
                        continue;
                    }
                    //if 2 bots are colliding by their frames and they are not the same bot
                    if ((outhief.getx_pos() < inthief.getx_pos() + inthief.getWidth) &&
                        (inthief.getx_pos() < outhief.getx_pos() + outhief.getWidth) &&
                        (outhief.gety_pos() < inthief.gety_pos() + inthief.getHeight) &&
                        (inthief.gety_pos() < outhief.gety_pos() + outhief.getHeight)
                        && inthief != outhief)
                    {
                        //if the bot is collding delete it later
                        deleteList.Add(inthief);
                        break;
                    }
                }
                i++;
            }
            //delete the bots from the list and the canvas
            foreach (Thief delete in deleteList)
            {
                delete.RemoveRect(canvas);
                thieves.Remove(delete);
            }
            //if the count of the bots is 1 the player won
            if (thieves.Count == 1)
            {
                thieves.Clear();
                dora = null;
                Stop();
                WinGame();
            }
            //update the menu incase of saving
            menu.UpdateList(thieves, dora);
        }

        //win message and remove all items incase of saving a finished game
        public void WinGame()
        {
            dora = null;
            thieves.Clear();
            menu.UpdateList(thieves, dora);
            MessageBox.Show("WINNER WINNER CHICKEN DINNER!", "YOU WON!");
            menuUp = false;
            CallMenu();
 
        }

        //this function is in charge on checking if the player collided with a bot and finish the game
        public void LostGame()
        {
            //checking each bot with the player
            foreach (Thief inthief in thieves)
            {
                //if the player is overlapping a bot
                if ((dora.getx_pos() < inthief.getx_pos() + inthief.getWidth) &&
                    (inthief.getx_pos() < dora.getx_pos() + dora.getWidth) &&
                    (dora.gety_pos() < inthief.gety_pos() + inthief.getHeight) &&
                    (inthief.gety_pos() < dora.gety_pos() + dora.getHeight))
                {
                    //clear all variables and update incase of saving, calling menu and displaying a message
                    thieves.Clear();
                    dora = null;
                    menu.UpdateList(thieves, dora);
                    Stop();
                    MessageBox.Show("You Lost!", "LOSER");
                    menuUp = false;
                    CallMenu();
                    return;
                }
            }
        }

    }

}
