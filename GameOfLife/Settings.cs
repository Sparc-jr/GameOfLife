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
        public int maxPopulations = 10;
        public enum CellsInteraction
        {
            нейтральное = 0,
            симбиотическое = 1,
            агрессивное = 2,
        } //взаимодействие видов: 0 - нейтральны; 1 - симбиоты; 2 - агрессивны
        public CellsInteraction cellsInteraction = new CellsInteraction();
        public char[] aliveCell = new char[10] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };// живая клетка  
        public char deadCell = ' ';                                   // мертвая клетка
        public int density = 15;     //живых клеток на 20 ячеек поля
        public int maxDensity = 20;  
        public int fieldSizeX = 32;           // размер игрового поля
        public int fieldSizeY = 17;          // размер игрового поля
        public int menuSizeX = 32;
        public int menuSizeY = 19;
        public int maxFieldSizeX = 230;
        public int maxFieldSizeY = 55;
        public int minFieldSize = 5;
        public ConsoleColor backgroundColor = ConsoleColor.Black;       // цвет фона консоли
        public ConsoleColor fieldColor = ConsoleColor.White;            // цвет символов игрового поля
        public ConsoleColor menuBorderColor = ConsoleColor.Blue;        // цвет рамки меню
        public int speed = 3;                                         //скорость игры
        public int speedCorrection = 800;
        public int[] realSpeed = { 300, 200, 100, 50, 20, 10 };
        public bool debugMode = false;
        // "правила игры"
        public int minAmountToAlive = 2; //минимальное количество соседей своего вида для выживания клетки
        public int maxAmountToAlive = 3; //максимальное количество соседей своего вида для выживания клетки
        public int maxAmountSimbioticToAlive = 4; //максимальное количество соседей в симбиотическом режиме для выживания клетки
        public int amountToBorn = 3; //количество соседей одного вида (или разных в симбиотическом режиме) для рождения в пустой клетке
        public int amountToBornAgressiveMode = 4; //количество соседей одного вида для рождения в пустой клетке, если рядом есть соседи другого вида, но их меньше
        public int amountOfExcessToCapture = 2; //на сколько должен превышать доминирующий сосед для захвата клетки
    }

}
