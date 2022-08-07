using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameOfLife
{
    internal class Settings
    {
        public int populations = 2;                                     //число видов популяций клеток
        public int maxPopulations;
        public enum cellsInteraction
        {
            neutral = 0,
            simbiotic = 1,
            agressive = 2,
        } //взаимодействие видов: 0 - нейтральны; 1 - симбиоты; 2 - агрессивны
        public char[] aliveCell = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };// живая клетка  
        public char deadCell = 'x';                                   // мертвая клетка
        public int density = 15;     //живых клеток на 20 ячеек поля
        public int maxDensity = 20;  
        public int fieldSizeX = 32;           // размер игрового поля
        public int fieldSizeY = 17;          // размер игрового поля
        public int menuSizeX = 32;
        public int menuSizeY = 16;
        public int maxFieldSizeX = 230;
        public int maxFieldSizeY = 60;
        public int minFieldSize = 5;
        public ConsoleColor backgroundColor = ConsoleColor.Black;       // цвет фона консоли
        public ConsoleColor fieldColor = ConsoleColor.White;            // цвет символов игрового поля
        public ConsoleColor menuBorderColor = ConsoleColor.Blue;              // цвет рамки меню
        public int speed = 3;                                         //скорость игры
        public int speedCorrection = 800;
        public int[] realSpeed = { 300, 200, 100, 50, 20, 10 };
        public bool debugMode = false;
    }

}
