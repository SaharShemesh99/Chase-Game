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

    /*
     This class represents thief who is suppose to run from the cop.
    There are 10 thieves and 1 player.
    The thieves have size and speed for the game and are required for the game's operation
     */
    internal class Thief : ScreenObject
    {
        public static int thiefs_width = 40;
        public static int thiefs_height = 50;
        protected static int speed = 1;
        public Thief(int x_pos, int y_pos) : base(x_pos, y_pos, thiefs_width, thiefs_height, speed) { }

        // Getting coordinates and checks if the current coordinates are touching
        //If yes return false else true.
        public static bool CheckValidation(Thief obj, int x_pos, int y_pos)
        {
            if ((obj.x_pos - thiefs_width <= x_pos && x_pos <= obj.x_pos + thiefs_width) ||
                (obj.y_pos - thiefs_height <= y_pos && y_pos <= obj.y_pos + thiefs_height))
            {
                    return false;
            }
            return true;
        }

    }
}
