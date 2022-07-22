using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    internal class Settings
    {
        public int populations = 3;                                     //число популяций клеток
        public enum cellsInteraction : int
        {
            neutral = 0,
            simbiotic,
            agressive
        } //взаимодействие популяций: 0 - нейтральны; 1 - симбиоты; 2 - агрессивны
        public char[] aliveCell = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };// живая клетка  
        public char deadCell = ' ';                                   // мертвая клетка
        public int density = 5;     //живых клеток на 20 ячеек поля       
        public int fieldSizeX = 80;//Console.LargestWindowWidth-2;           // размер игрового поля
        public int fieldSizeY = 40;// Console.LargestWindowHeight;          // размер игрового поля
        public ConsoleColor backgroundColor = ConsoleColor.Black;       // цвет фона консоли
        public ConsoleColor fieldColor = ConsoleColor.White;            // цвет символов игрового поля
        public ConsoleColor cellColor = ConsoleColor.DarkBlue;            // цвет клеток
        public ConsoleColor menuBorderColor = ConsoleColor.Blue;              // цвет рамки меню
        public int speed = 6;                                         //скорость игры
        public int[] realSpeed = { 300, 250, 200, 150, 100, 50 };
    }

}
