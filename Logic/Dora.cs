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
     This class is the players class. it has different measures and speed than the computer's bots.
    It is inherating the ScreenObject class' fields.
     */
    class Dora : ScreenObject
    {
        public static int thiefs_width = 30;
        public static int thiefs_height = 40;
        protected static int speed = 10;
        public Dora(int x_pos, int y_pos) : base(x_pos, y_pos, thiefs_width, thiefs_height, speed) { }
    }
}
