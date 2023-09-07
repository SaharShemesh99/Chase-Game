using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Timers;
using System.Windows.Shapes;
using System.Xaml;
using Logic;
using System.Windows.Threading;
using System.Threading;


namespace CopsAndThieves
{
    //The window of the User Interface
    public partial class MainWindow : Window
    {
        //The logic of the game
        public static logic log = new logic();

        public MainWindow()
        {
            InitializeComponent();
            //fixing the window's measures
            windy.Measure(new Size(MaxWidth, MaxHeight));

            //connecting the logic to the UI and calling the menu
            windy.Content = log.canvas;
            log.CallMenu();
            //connecting the keyboard for the player's game
            windy.KeyDown += keyPressed;

        }

        //sending the choices of the player to the logic
        private void keyPressed(object sender, KeyEventArgs e)
        {
            log.KeyEntered(e);
        }
    }
}
