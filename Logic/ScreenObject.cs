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
     This class is the foundation of the code.
    The fields are required to create a rectangle 
    The speed is needed to move the object
    around the canvas
     */
    internal class ScreenObject
    {
        //rectangles fields and borders definition
        public static int left_border = 0;
        public static int right_border = 1500;
        public static int lower_border = 800;
        public static int upper_border = 0;
        protected int x_pos { get; set; }
        protected int y_pos { get; set; }
        protected int width { get; set; }
        protected int height { get; set; }
        protected Rectangle rect;
        protected int speed { get; set; }


        //Get measures and coordinates. different decleration for different fields for my own convenience.
        public int getWidth => width;
        public int getHeight => height;

        public int getx_pos() => x_pos;
        public int gety_pos() => y_pos;


        //Creating rectangle
        public ScreenObject(int x_pos, int y_pos, int width, int height, int speed)
        {
            this.x_pos = x_pos;
            this.y_pos = y_pos;
            this.width = width;
            this.height = height;
            this.speed = speed;
            this.rect = new Rectangle();
            rect.Height = height;
            rect.Width = width;
        }

        //Movement function - as long as it is not crossing a border move left
        public void MoveLeft()
        {
            if (x_pos - speed > left_border)
            {
                x_pos -= speed;
                Canvas.SetLeft(this.rect, x_pos);
            }
        }

        //Movement function - as long as it is not crossing a border move right
        public void MoveRight()
        {
            if (x_pos + speed < right_border - this.width)
            {
                x_pos += speed;
                Canvas.SetLeft(this.rect, x_pos);
            }
        }

        //Movement function - as long as it is not crossing a border move up
        public void MoveUp()
        {
            if (y_pos - speed > upper_border)
            {
                y_pos -= speed;
                Canvas.SetTop(this.rect, y_pos);
            }
        }

        //Movement function - as long as it is not crossing a border move down
        public void MoveDown()
        {
            if (y_pos + speed < lower_border - this.height)
            {
                y_pos += speed;
                Canvas.SetTop(this.rect, y_pos);
            }
        }

        // Checks if there is a collision.
        // if 2 objects collide return false else true.
        public bool CheckCollision(ScreenObject obj1, ScreenObject obj2)
        {
            if ((obj1.x_pos + obj1.width > obj2.x_pos) || (obj1.y_pos + obj1.height > obj2.y_pos))
            {
                return false;
            }
            return true;
        }

        //creating a rectangle and displaying it on the canvas
        public void GetRect(Canvas canvas)
        {
            Canvas.SetTop(rect, y_pos);
            Canvas.SetLeft(rect, x_pos);
            rect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@"source\Swiper.png"), UriKind.Absolute)) };
            canvas.Children.Add(rect);
        }

        //creating the player and displaying it on the canvas
        public void GetDora(Canvas canvas)
        {
            Canvas.SetTop(rect, y_pos);
            Canvas.SetLeft(rect, x_pos);
            rect.Fill = new ImageBrush { ImageSource = new BitmapImage(new Uri(System.IO.Path.GetFullPath(@"source\Dora.png"), UriKind.Absolute)) };
            canvas.Children.Add(rect);
        }

        //removing the rectangle from the canvas
        public void RemoveRect(Canvas canvas)
        {
            canvas.Children.Remove(rect); 
        }
    }
}
