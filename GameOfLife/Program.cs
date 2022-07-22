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
            int generation = 0;
            int width = gameSettings.fieldSizeX;
            int height = gameSettings.fieldSizeY;
            Console.CursorVisible = false;
            Console.SetWindowSize(width + 2, height+1);
            char[,] fieldCurrentState = new char[width,height];
            char[,] fieldNewState = new char[width, height];
            fieldCurrentState = GenerateStartField(width, height);                    
            DrawField(fieldCurrentState);
            ConsoleKeyInfo keyPressed;
            do
            {
                if (Console.KeyAvailable)
                {
                    keyPressed = Console.ReadKey(true);
                    switch (keyPressed.Key)
                    {
                        case ConsoleKey.Escape: SettingsMenu(); DrawField(fieldCurrentState); break;
                        case ConsoleKey.R: Console.Clear(); StartGame() ;break;
                        case ConsoleKey.N: ShowNeighbours(fieldCurrentState);Console.ReadLine(); DrawField(fieldCurrentState); break;
                    }
                }
                generation++;
                generation %= 10;
                //gameSettings.cellColor = (ConsoleColor)(generation+1);
                //Console.ForegroundColor = gameSettings.cellColor;
                fieldNewState = CalculateNewStateField(fieldCurrentState);
                Array.Copy(fieldNewState,fieldCurrentState,fieldCurrentState.Length);
                Thread.Sleep(gameSettings.realSpeed[gameSettings.speed - 1]);
            }
            while (!exit);
        }
        private static void ShowNeighbours(char[,] fieldState)
        {
            ConsoleColor defaultColor = Console.ForegroundColor;
            for(int n=0; n<gameSettings.populations; n++)
            {
                int neighboursCount = 0;
                for (int j = 0; j <= fieldState.GetUpperBound(1); j++)
                {
                    for (int i = 0; i <= fieldState.GetUpperBound(0); i++)
                    {
                        neighboursCount = CalculateNeighbours(i, j, fieldState,n);
                        if (fieldState[i, j] == gameSettings.aliveCell[n])
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                        }
                        Console.SetCursorPosition(i, j);
                        Console.Write(neighboursCount);
                    }
                }
                Console.ForegroundColor = defaultColor;
            }

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
                    case ConsoleKey.UpArrow: gameSettings.fieldSizeY += gameSettings.fieldSizeY < 62 ? 1 : 0; ResizeField(); restart = true; break;//увеличение высоты игрового поля
                    case ConsoleKey.DownArrow: gameSettings.fieldSizeY -= gameSettings.fieldSizeY > 5 ? 1 : 0; ResizeField(); restart = true; break;//уменьшение высоты игрового поля
                    case ConsoleKey.LeftArrow: gameSettings.fieldSizeX -= gameSettings.fieldSizeX > 5 ? 1 : 0; ResizeField(); restart = true; break;//уменьшение ширины игрового поля
                    case ConsoleKey.RightArrow: gameSettings.fieldSizeX += gameSettings.fieldSizeX < 230 ? 1 : 0; ResizeField(); restart = true; break;//увеличение ширины игрового поля
                    case ConsoleKey.A: gameSettings.density -=gameSettings.density > 1 ? 1 : 0;restart = true; break;
                    case ConsoleKey.Q: gameSettings.density += gameSettings.density < 19 ? 1 : 0; restart = true; break;
                    case ConsoleKey.W: gameSettings.speed += gameSettings.speed < 6 ? 1 : 0; break;//увеличение скорости игры
                    case ConsoleKey.S: gameSettings.speed -= gameSettings.speed > 1 ? 1 : 0; break;//уменьшение скорости игры
                    case ConsoleKey.E: gameSettings.populations += gameSettings.populations < 10 ? 1 : 0; break;//увеличение числа видов 
                    case ConsoleKey.D: gameSettings.populations -= gameSettings.populations > 1 ? 1 : 0; break;//уменьшение числа видов
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
            Console.WindowHeight = gameSettings.fieldSizeY < 5 ? 14 : gameSettings.fieldSizeY;
            Console.WindowWidth = gameSettings.fieldSizeX < 5 ? 14 : gameSettings.fieldSizeX+2;
        }
        private static void DrawMenuBorders()
        {
            Console.ForegroundColor = gameSettings.menuBorderColor;
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("#=============================#");
            for (int i = 0; i < 14; i++)
            {
                Console.WriteLine("|                             |");
            }
            Console.WriteLine("#=============================#");
            Console.ForegroundColor = gameSettings.fieldColor;
        }
        private static void EraseMenuLeftovers()
        {
            Console.SetCursorPosition(0, 0);
            for (int i = 0; i < 16; i++)
            {
                Console.WriteLine("                               ");
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
            for (int j = 0; j <= fieldState.GetUpperBound(1); j++)
            {
                for (int i = 0; i <= fieldState.GetUpperBound(0); i++)
                {
                    if (fieldState[i, j] != ' ')
                    {
                        gameSettings.cellColor = (ConsoleColor)(fieldState[i, j] - '0' + 1);
                        Console.ForegroundColor = gameSettings.cellColor;
                    }
                    Console.Write(fieldState[i,j]);
                }
                Console.WriteLine();
            }
        }
        static char[,] CalculateNewStateField(char[,] oldStateField)
        {
            char[,] fieldNew = new char[oldStateField.GetUpperBound(0) + 1, oldStateField.GetUpperBound(1) + 1];
            int neighboursCount = 0;
            for(int n=gameSettings.populations-1; n>=0;n--)
            {
                gameSettings.cellColor = (ConsoleColor)(n+1);
                Console.ForegroundColor = gameSettings.cellColor;

                for (int j = 0; j <= oldStateField.GetUpperBound(1); j++)
                {
                    for (int i = 0; i <= oldStateField.GetUpperBound(0); i++)
                    {
                        neighboursCount = CalculateNeighbours(i, j, oldStateField, n);
                        if (oldStateField[i, j] == gameSettings.deadCell && neighboursCount == 3)
                        {
                            fieldNew[i, j] = gameSettings.aliveCell[n];
                            Console.SetCursorPosition(i, j);
                            Console.Write(gameSettings.aliveCell[n]);
                        }
                        else if (oldStateField[i, j] == gameSettings.aliveCell[n] && (neighboursCount < 2 || neighboursCount > 3))
                        {
                            fieldNew[i, j] = gameSettings.deadCell;
                            Console.SetCursorPosition(i, j);
                            Console.Write(gameSettings.deadCell);
                        }
                        else
                        {
                            fieldNew[i, j] = oldStateField[i, j];
                        }
                    }
                }
            }

            return fieldNew;
        }
        static int CalculateNeighbours (int xPos,int yPos, char[,] oldStateField, int population)
        {
            int neighbours = 0;
            if (yPos>0 && xPos>0 && oldStateField[xPos-1,yPos-1]== gameSettings.aliveCell[population])
            { neighbours++; }
            if (yPos > 0 && oldStateField[xPos,yPos-1] == gameSettings.aliveCell[population])
            { neighbours++; }
            if (yPos > 0 && xPos < gameSettings.fieldSizeX-1 && oldStateField[xPos + 1,yPos - 1] == gameSettings.aliveCell[population])
            { neighbours++; }
            if ( xPos > 0 && oldStateField[xPos-1,yPos] == gameSettings.aliveCell[population])
            { neighbours++; }
            if (xPos < gameSettings.fieldSizeX-1 && oldStateField[xPos+1,yPos] == gameSettings.aliveCell[population])
            { neighbours++; }
            if (yPos < gameSettings.fieldSizeY-1 && xPos > 0 && oldStateField[xPos - 1,yPos + 1] == gameSettings.aliveCell[population])
            { neighbours++; }
            if (yPos < gameSettings.fieldSizeY-1 && oldStateField[xPos, yPos + 1] == gameSettings.aliveCell[population])
            { neighbours++; }
            if (yPos < gameSettings.fieldSizeY-1 && xPos < gameSettings.fieldSizeX-1 && oldStateField[xPos + 1, yPos + 1] == gameSettings.aliveCell[population])
            { neighbours++; }
            return neighbours;
        }
    }
}