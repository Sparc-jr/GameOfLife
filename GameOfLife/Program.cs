namespace GameOfLife
{
    internal class Program
    {


        public static Settings gameSettings = new Settings();
        public static bool exit = false;

        static void Main(string[] args)
        {
            Console.Title = "GAME OF LIFE";

            StartGame();
            

        }
        private static void StartGame()
        {
            int width = gameSettings.fieldSizeX;
            int height = gameSettings.fieldSizeY;
            Console.CursorVisible = false;
            Console.SetWindowSize(gameSettings.fieldSizeX > gameSettings.minConsoleWidth ? gameSettings.fieldSizeX + 2 : gameSettings.minConsoleWidth + 2,
                                  gameSettings.fieldSizeY > gameSettings.minConsoleHeight ? gameSettings.fieldSizeY + 1 : gameSettings.minConsoleHeight + 1);
            char[,] fieldCurrentState = new char[width,height];
            char[,] fieldNewState = new char[width, height];
            fieldCurrentState = GenerateStartField(width, height);                    
            DrawField(fieldCurrentState);

            ConsoleKeyInfo keyPressed;
            do
            {
                fieldNewState = CalculateNewStateField(fieldCurrentState);
                if (gameSettings.debugMode)
                {
                    DrawField(fieldNewState);
                    Console.ReadLine();
                }
                Array.Copy(fieldNewState,fieldCurrentState,fieldCurrentState.Length);
                Thread.Sleep(gameSettings.realSpeed[gameSettings.speed - 1]*8000/gameSettings.fieldSizeX/gameSettings.fieldSizeY);
                if (Console.KeyAvailable)
                {
                    keyPressed = Console.ReadKey(true);
                    switch (keyPressed.Key)
                    {
                        case ConsoleKey.Escape: SettingsMenu(); ReDrawFieldUnderMenu(fieldCurrentState); break;
                        case ConsoleKey.R: Console.Clear(); StartGame() ;break;
                        case ConsoleKey.H: gameSettings.debugMode = !gameSettings.debugMode; break;
                    }
                }
            }
            while (!exit);
        }
        private static void SettingsMenu()
        {
            ConsoleKeyInfo keyPressed;
            bool menu = true;
            bool restart = false;
            DrawMenuBorders();
            while (menu)
            {
                Console.SetCursorPosition(2, 1);
                Console.Write("Ширина поля  (" + (char)27 + (char)26 + "): {0}", gameSettings.fieldSizeX);
                Console.SetCursorPosition(2, 3);
                Console.Write("Высота поля (" + (char)25 + (char)24 + "): {0}", gameSettings.fieldSizeY);
                Console.SetCursorPosition(2, 5);
                Console.Write("Начальная плотность клеток");
                Console.SetCursorPosition(2, 6);
                Console.Write("на 20 ячеек (" + "A/Q" + "): {0}", gameSettings.density);
                Console.SetCursorPosition(2, 8);
                Console.Write("Скорость симуляции (W/S): {0}", gameSettings.speed);
                Console.SetCursorPosition(2, 10);
                Console.Write("Число видов клеток (E/D): {0}", gameSettings.populations);
                Console.SetCursorPosition(4, 12);
                Console.Write("Перезапустить игру: \"R\"");
                Console.SetCursorPosition( 7, 14);
                Console.Write("Выйти из игры: \"X\"");
                keyPressed = Console.ReadKey(true);
                switch (keyPressed.Key)
                {
                    case ConsoleKey.UpArrow: gameSettings.fieldSizeY += gameSettings.fieldSizeY < 60 ? 1 : 0; ResizeField(); restart = true; break;//увеличение высоты игрового поля
                    case ConsoleKey.DownArrow: gameSettings.fieldSizeY -= gameSettings.fieldSizeY > 5 ? 1 : 0; ResizeField(); restart = true; break;//уменьшение высоты игрового поля
                    case ConsoleKey.LeftArrow: gameSettings.fieldSizeX -= gameSettings.fieldSizeX > 5 ? 1 : 0; ResizeField(); restart = true; break;//уменьшение ширины игрового поля
                    case ConsoleKey.RightArrow: gameSettings.fieldSizeX += gameSettings.fieldSizeX < 230 ? 1 : 0; ResizeField(); restart = true; break;//увеличение ширины игрового поля
                    case ConsoleKey.A: gameSettings.density -=gameSettings.density > 1 ? 1 : 0;restart = true; break;
                    case ConsoleKey.Q: gameSettings.density += gameSettings.density < 19 ? 1 : 0; restart = true; break;
                    case ConsoleKey.W: gameSettings.speed += gameSettings.speed < 6 ? 1 : 0; break;//увеличение скорости игры
                    case ConsoleKey.S: gameSettings.speed -= gameSettings.speed > 1 ? 1 : 0; break;//уменьшение скорости игры
                    case ConsoleKey.E: gameSettings.populations += gameSettings.populations < 10 ? 1 : 0; restart = true; break;//увеличение числа видов 
                    case ConsoleKey.D: gameSettings.populations -= gameSettings.populations > 1 ? 1 : 0; restart = true; break;//уменьшение числа видов
                    case ConsoleKey.R: restart = true; menu = false; EraseMenuLeftovers(); break;
                    case ConsoleKey.X: ConfirmExit(); menu = false; break;                            //выход из игры
                    case ConsoleKey.Escape: menu = false; EraseMenuLeftovers(); break;                 //возврат в игру
                    default: break;
                }
            }
            if (restart)
            {
                Console.Clear();
                StartGame();
            }
        }
        public static void ResizeField()
        {
            Console.SetWindowSize(gameSettings.fieldSizeX > gameSettings.minConsoleWidth ? gameSettings.fieldSizeX + 2 : gameSettings.minConsoleWidth+2, 
                                  gameSettings.fieldSizeY > gameSettings.minConsoleHeight ? gameSettings.fieldSizeY + 1 : gameSettings.minConsoleHeight+1);
        }
        private static void DrawMenuBorders()
        {
            Console.ForegroundColor = gameSettings.menuBorderColor;
            Console.SetCursorPosition(0, 0);
            Console.Write("#");
            for(int i=0;i<gameSettings.menuSizeX-2;i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("#");
            for (int i = 0; i < gameSettings.menuSizeY-2; i++)
            {
                Console.Write("|");
                for (int j = 0; j < gameSettings.menuSizeX - 2; j++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine("|");
            }
            Console.Write("#");
            for (int i = 0; i < gameSettings.menuSizeX - 2; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("#");
            Console.ForegroundColor = gameSettings.fieldColor;
        }
        private static void EraseMenuLeftovers()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < gameSettings.menuSizeY; i++)
            {
                for (int j = 0; j < gameSettings.menuSizeX; j++)
                {
                    Console.Write(" ");
                }
                Console.WriteLine();
            }
        }
        private static void ConfirmExit()
        {
            DrawMenuBorders();
            ConsoleKeyInfo keyPressed;
            Console.SetCursorPosition( 9, 5);
            Console.WriteLine("Выйти из игры?");
            Console.SetCursorPosition( 14, 8);
            Console.WriteLine("y/n");
            Thread.Sleep(300);
            keyPressed = Console.ReadKey(true);
            switch (keyPressed.Key)
            {
                case ConsoleKey.Y: case ConsoleKey.Enter: exit = true; break;
                default: SettingsMenu(); break;                         //возврат в меню
            }
        }
        static char[,] GenerateStartField(int width, int height)
        {
            var rand = new Random();
            char[,] fieldNow = new char[width,height];
            for (int j = 0; j < height; j++)
            {

                for (int i = 0; i < width; i++)
                {
                    fieldNow[i,j]=(rand.Next(20) < gameSettings.density? gameSettings.aliveCell[rand.Next(gameSettings.populations)] : gameSettings.deadCell);
                }

            }
            return fieldNow;
        }
        static void DrawField(char[,] fieldState)
        {
            Console.ForegroundColor = gameSettings.cellColor;
            Console.SetCursorPosition(0,0);
            for (int j = 0; j <gameSettings.fieldSizeY; j++)
            {
                for (int i = 0; i < gameSettings.fieldSizeX; i++)
                {
                    if (fieldState[i, j] != gameSettings.deadCell)
                    {
                        Console.ForegroundColor = (ConsoleColor)(fieldState[i, j] - '0' + 1);
                    }
                    Console.Write(fieldState[i,j]);
                }
                Console.WriteLine();
            }
        }
        static void ReDrawFieldUnderMenu(char[,] fieldState)
        {
            Console.ForegroundColor = gameSettings.cellColor;
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j < (gameSettings.fieldSizeY > gameSettings.menuSizeY? gameSettings.menuSizeY : gameSettings.fieldSizeY); j++)
            {
                for (int i = 0; i < (gameSettings.fieldSizeX > gameSettings.menuSizeX ? gameSettings.menuSizeX : gameSettings.fieldSizeX); i++)
                {
                    if (fieldState[i, j] != gameSettings.deadCell)
                    {
                        Console.ForegroundColor = (ConsoleColor)(fieldState[i, j] - '0' + 1);
                    }
                    Console.Write(fieldState[i, j]);
                }
                Console.WriteLine();
            }
        }
        static char[,] CalculateNewStateField(char[,] oldStateField)
        {
            char[,] fieldNew = new char[gameSettings.fieldSizeX, gameSettings.fieldSizeY];
            int[] neighboursCount = new int[gameSettings.populations];
            for (int j = 0; j < gameSettings.fieldSizeY; j++)
            {
                for (int i = 0; i < gameSettings.fieldSizeX; i++)
                {
                    neighboursCount = CalculateNeighbours(i, j, oldStateField);
                    if (oldStateField[i, j] == gameSettings.deadCell) 
                    {
                        bool bornCell = false;
                        for (int n = 0; n < gameSettings.populations; n++)
                        { 
                            if (neighboursCount[n] == 3)
                            {
                                bornCell = true;
                                gameSettings.cellColor = (ConsoleColor)(n + 1);
                                Console.ForegroundColor = gameSettings.cellColor;
                                fieldNew[i, j] = gameSettings.aliveCell[n];
                                Console.SetCursorPosition(i, j);
                                if (gameSettings.debugMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(gameSettings.deadCell);
                                }
                                else
                                {
                                    Console.Write(gameSettings.aliveCell[n]);
                                }
                                break;
                            }
                        }
                        if (!bornCell)
                        {
                            fieldNew[i, j] = oldStateField[i, j];
                        }
                    }
                    else if (neighboursCount[int.Parse(oldStateField[i, j].ToString())] == 2 || neighboursCount[int.Parse(oldStateField[i, j].ToString())] == 3)
                    {
                        fieldNew[i, j] = oldStateField[i, j];
                    }
                    else
                    {
                        bool bornCell = false;
                        for (int n = 0; n < gameSettings.populations; n++)
                        {
                            if (neighboursCount[n] == 3)
                            {
                                bornCell = true;
                                gameSettings.cellColor = (ConsoleColor)(n + 1);
                                Console.ForegroundColor = gameSettings.cellColor;
                                fieldNew[i, j] = gameSettings.aliveCell[n];
                                Console.SetCursorPosition(i, j);
                                if (gameSettings.debugMode)
                                {
                                    Console.ForegroundColor = ConsoleColor.Yellow;
                                    Console.Write(gameSettings.deadCell);
                                }
                                else
                                {
                                    Console.Write(gameSettings.aliveCell[n]);
                                }
                                break;
                            }
                        }
                        if (!bornCell)
                        {
                            fieldNew[i, j] = gameSettings.deadCell;
                            Console.SetCursorPosition(i, j);
                            if (gameSettings.debugMode)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(oldStateField[i,j]);
                            }
                            else
                            {
                                Console.Write(gameSettings.deadCell);
                            }




                        }
                    }                 
                }
            }
            if (gameSettings.debugMode) Console.ReadLine();
            return fieldNew;
        }
        static int[] CalculateNeighbours (int xPos,int yPos, char[,] oldStateField)
        {
            int[] neighbours = new int[gameSettings.populations];
            for (int i = 0; i < gameSettings.populations; i++)
            {
                if (yPos>0 && xPos>0 && oldStateField[xPos-1,yPos-1]== gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos > 0 && oldStateField[xPos,yPos-1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos > 0 && xPos < gameSettings.fieldSizeX-1 && oldStateField[xPos + 1,yPos - 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if ( xPos > 0 && oldStateField[xPos-1,yPos] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (xPos < gameSettings.fieldSizeX-1 && oldStateField[xPos+1,yPos] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos < gameSettings.fieldSizeY-1 && xPos > 0 && oldStateField[xPos - 1,yPos + 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos < gameSettings.fieldSizeY-1 && oldStateField[xPos, yPos + 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos < gameSettings.fieldSizeY-1 && xPos < gameSettings.fieldSizeX-1 && oldStateField[xPos + 1, yPos + 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
            }

            return neighbours;
        }
    }
}