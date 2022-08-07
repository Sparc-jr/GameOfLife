namespace GameOfLife
{
    internal class Cells
    {
        public int X;
        public int Y;
        public char State;
    }    
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
            Console.SetWindowSize(gameSettings.fieldSizeX > gameSettings.menuSizeX ? gameSettings.fieldSizeX + 1 : gameSettings.menuSizeX+1,
                                  gameSettings.fieldSizeY > gameSettings.menuSizeY ? gameSettings.fieldSizeY + 2 : gameSettings.menuSizeY+2);
            char[,] fieldCurrentState = new char[width,height];
            fieldCurrentState = GenerateStartField(width, height);                    
            DrawField(fieldCurrentState);
            List<Cells> cellsWithChanges = new List<Cells>();
            ConsoleKeyInfo keyPressed;
            do
            {
                switch (gameSettings.cellsInteraction)
                {
                    case Settings.CellsInteraction.нейтральное: cellsWithChanges = CalculateCellsToChangeState(fieldCurrentState); break;
                    case Settings.CellsInteraction.симбиотическое: cellsWithChanges = CalculateSimbioticCellsToChangeState(fieldCurrentState); break;
                    case Settings.CellsInteraction.агрессивное: cellsWithChanges = CalculateAgressiveCellsToChangeState(fieldCurrentState); break;
                }
                if (gameSettings.debugMode)
                {
                    DrawFieldUpdateDebugMode(fieldCurrentState,cellsWithChanges);
                    Console.ReadLine();
                    DrawFieldUpdate(cellsWithChanges);
                    Console.ReadLine();
                } else
                {
                    DrawFieldUpdate(cellsWithChanges);
                }
                UpdateFieldState(fieldCurrentState, cellsWithChanges);
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(gameSettings.realSpeed[gameSettings.speed - 1] * gameSettings.speedCorrection / gameSettings.fieldSizeX / gameSettings.fieldSizeY);
                cellsWithChanges.Clear();
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
                Console.Write("Ширина поля  (" + (char)27 + (char)26 + $"): {gameSettings.fieldSizeX,2}");
                Console.SetCursorPosition(2, 3);
                Console.Write($"Высота поля (" + (char)25 + (char)24 + $"): {gameSettings.fieldSizeY,2}");
                Console.SetCursorPosition(2, 5);
                Console.Write("Начальная плотность клеток");
                Console.SetCursorPosition(2, 6);
                Console.Write("на 20 ячеек (" + "A/Q" + $"): {gameSettings.density,2}");
                Console.SetCursorPosition(2, 8);
                Console.Write($"Скорость симуляции (W/S):  {gameSettings.speed}");
                Console.SetCursorPosition(2, 10);
                Console.Write($"Число видов клеток (E/D): {gameSettings.populations,2}");
                Console.SetCursorPosition(2, 12);
                Console.Write("Взаимодействие популяций");
                Console.SetCursorPosition(2, 13);
                Console.Write($" \"Tab\": {gameSettings.cellsInteraction,14}");
                Console.SetCursorPosition(4, 15);
                Console.Write("Перезапустить игру: \"R\"");
                Console.SetCursorPosition( 7, 17);
                Console.Write("Выйти из игры: \"X\"");
                keyPressed = Console.ReadKey(true);
                switch (keyPressed.Key)
                {
                    case ConsoleKey.UpArrow: gameSettings.fieldSizeY += gameSettings.fieldSizeY < gameSettings.maxFieldSizeY ? 1 : 0; ResizeField(); restart = true; break;//увеличение высоты игрового поля
                    case ConsoleKey.DownArrow: gameSettings.fieldSizeY -= gameSettings.fieldSizeY > gameSettings.minFieldSize ? 1 : 0; ResizeField(); restart = true; break;//уменьшение высоты игрового поля
                    case ConsoleKey.LeftArrow: gameSettings.fieldSizeX -= gameSettings.fieldSizeX > gameSettings.minFieldSize ? 1 : 0; ResizeField(); restart = true; break;//уменьшение ширины игрового поля
                    case ConsoleKey.RightArrow: gameSettings.fieldSizeX += gameSettings.fieldSizeX < gameSettings.maxFieldSizeX ? 1 : 0; ResizeField(); restart = true; break;//увеличение ширины игрового поля
                    case ConsoleKey.A: gameSettings.density -=gameSettings.density > 1 ? 1 : 0;restart = true; break;
                    case ConsoleKey.Q: gameSettings.density += gameSettings.density < gameSettings.maxDensity ? 1 : 0; restart = true; break;
                    case ConsoleKey.W: gameSettings.speed += gameSettings.speed < 6 ? 1 : 0; break;//увеличение скорости игры
                    case ConsoleKey.S: gameSettings.speed -= gameSettings.speed > 1 ? 1 : 0; break;//уменьшение скорости игры
                    case ConsoleKey.E: gameSettings.populations += gameSettings.populations < gameSettings.maxPopulations ? 1 : 0; restart = true; break;//увеличение числа видов 
                    case ConsoleKey.D: gameSettings.populations -= gameSettings.populations > 1 ? 1 : 0; restart = true; break;//уменьшение числа видов
                    case ConsoleKey.Tab: gameSettings.cellsInteraction = (int)(gameSettings.cellsInteraction) < 2 ? (Settings.CellsInteraction)((int)(gameSettings.cellsInteraction) + 1) : Settings.CellsInteraction.нейтральное; restart = true; break;//изменение взаимодействия популяций
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
            Console.SetWindowSize(gameSettings.fieldSizeX > gameSettings.menuSizeX ? gameSettings.fieldSizeX + 1 : gameSettings.menuSizeX + 1,
                                  gameSettings.fieldSizeY > gameSettings.menuSizeY ? gameSettings.fieldSizeY + 1 : gameSettings.menuSizeY + 1);
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
            Console.SetCursorPosition(0,0);
            for (int j = 0; j <gameSettings.fieldSizeY; j++)
            {
                for (int i = 0; i < gameSettings.fieldSizeX; i++)
                {
                    if (fieldState[i, j] != gameSettings.deadCell)
                    {
                        if (fieldState[i, j] != '7')
                        {
                            Console.ForegroundColor = (ConsoleColor)(fieldState[i, j] - '0' + 1);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write(fieldState[i,j]);
                }
                Console.WriteLine();
            }
        }
        static void DrawFieldUpdate(List<Cells> changingCells)
        {
            foreach(Cells cell in changingCells)
            {
                Console.SetCursorPosition(cell.X,cell.Y);
                if(cell.State!=gameSettings.deadCell)
                {
                    if (cell.State != '7')
                    {
                        Console.ForegroundColor = (ConsoleColor)(cell.State - '0' + 1);
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                Console.Write(cell.State);
            }
        }
        static void DrawFieldUpdateDebugMode(char[,] fieldState, List<Cells> changingCells)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Cells cell in changingCells)
            {
                Console.SetCursorPosition(cell.X, cell.Y);
                Console.Write(fieldState[cell.X, cell.Y]);
            }
        }
        static void UpdateFieldState(char[,] fieldState, List<Cells> changingCells)
        {
            foreach (Cells cell in changingCells)
            {
                fieldState[cell.X,cell.Y] = cell.State;
            }
        }
        static void ReDrawFieldUnderMenu(char[,] fieldState)
        {
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j <= (fieldState.GetUpperBound(1) > gameSettings.menuSizeY? gameSettings.menuSizeY : fieldState.GetUpperBound(1)); j++)
            {
                for (int i = 0; i <= (fieldState.GetUpperBound(0) > gameSettings.menuSizeX ? gameSettings.menuSizeX : fieldState.GetUpperBound(0)); i++)
                {
                    if (fieldState[i, j] != gameSettings.deadCell)
                    {
                        if (fieldState[i, j] != '7')
                        {
                            Console.ForegroundColor = (ConsoleColor)(fieldState[i, j] - '0' + 1);
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Magenta;
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                    }
                    Console.Write(fieldState[i, j]);
                }
                Console.WriteLine();
            }
        }
        static List<Cells> CalculateCellsToChangeState(char[,] oldStateField)
        {
            List<Cells> cellsToUpdate = new List<Cells>(); 
            int[] neighboursCount = new int[gameSettings.populations];
            for (int j = 0; j < gameSettings.fieldSizeY; j++)
            {
                for (int i = 0; i < gameSettings.fieldSizeX; i++)
                {
                    neighboursCount = CalculateNeighbours(i, j, oldStateField);
                    if (oldStateField[i, j] == gameSettings.deadCell)
                    {
                        for (int n = 0; n < gameSettings.populations; n++)
                        {
                            if (neighboursCount[n] == gameSettings.amountToBorn)
                            {
                                Cells cell = new Cells() { X = i, Y = j, State = (char)('0' + n)};
                                cellsToUpdate.Add(cell);
                                break;
                            }
                        }

                    }
                    else if (neighboursCount[oldStateField[i, j]-'0'] != gameSettings.minAmountToAlive && neighboursCount[oldStateField[i, j]-'0'] != gameSettings.minAmountToAlive)
                    {                    
                        bool bornCell = false;
                        for (int n = 0; n < gameSettings.populations; n++)
                        {
                            if (neighboursCount[n] == gameSettings.amountToBorn)
                            {
                                bornCell = true;
                                Cells cell = new Cells() { X = i, Y = j, State = (char)('0' + n) };
                                cellsToUpdate.Add(cell);
                                break;
                            }
                        }
                        if (!bornCell)
                        {
                            Cells cell = new Cells() { X = i, Y = j, State = gameSettings.deadCell };
                            cellsToUpdate.Add(cell); 
                        }
                    }
                }
            }
            return cellsToUpdate;
        }
        static List<Cells> CalculateSimbioticCellsToChangeState(char[,] oldStateField)
        {
            List<Cells> cellsToUpdate = new List<Cells>();
            int[] neighboursCount = new int[gameSettings.populations];
            for (int j = 0; j < gameSettings.fieldSizeY; j++)
            {
                for (int i = 0; i < gameSettings.fieldSizeX; i++)
                {
                    neighboursCount = CalculateNeighbours(i, j, oldStateField);
                    int allNeighboursCount = neighboursCount.Sum();
                    int dominantPopulation = Array.IndexOf(neighboursCount, neighboursCount.Max());
                    bool bornCell = false;
                    bool dieCell = false;
                    if (oldStateField[i, j] == gameSettings.deadCell)
                    {
                        bool bornInFreeCell = false;
                        for (int n = 0; n < gameSettings.populations; n++)
                        {
                            if (neighboursCount[n] == gameSettings.amountToBorn)
                            {
                                bornInFreeCell = true;
                                Cells cell = new Cells() { X = i, Y = j, State = (char)('0' + n) };
                                cellsToUpdate.Add(cell);
                                break;
                            }
                        }
                        if (!bornInFreeCell && allNeighboursCount == gameSettings.amountToBorn)
                        {
                            bornCell = true;
                        }
                    }
                    else if (neighboursCount[oldStateField[i, j] - '0'] < gameSettings.minAmountToAlive)
                    {

                        if (allNeighboursCount < gameSettings.minAmountToAlive || allNeighboursCount > gameSettings.maxAmountSimbioticToAlive)
                        {
                            dieCell = true;
                        }
                        else if (allNeighboursCount == gameSettings.amountToBorn)
                        {
                            bornCell = true;
                        }
                        else if (allNeighboursCount > gameSettings.amountToBorn && allNeighboursCount <= gameSettings.maxAmountSimbioticToAlive)
                        {
                            for (int n = 0; n < gameSettings.populations; n++)
                            {
                                if (neighboursCount[n] == gameSettings.amountToBorn)
                                {
                                    bornCell = true;
                                    break;
                                }
                            }
                        }
                    }
                    else if (neighboursCount[oldStateField[i, j] - '0'] >= gameSettings.minAmountToAlive && neighboursCount[oldStateField[i, j] - '0'] <= gameSettings.maxAmountToAlive)
                    {
                        if (allNeighboursCount > gameSettings.maxAmountSimbioticToAlive)
                        {
                            dieCell = true;
                        }
                        else
                        {
                            for (int n = 0; n < gameSettings.populations; n++)
                            {
                                if (neighboursCount[n] == gameSettings.amountToBorn)
                                {
                                    bornCell = true;
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        dieCell = true;
                    }
                    if (dieCell)
                    {
                        Cells cell = new Cells() { X = i, Y = j, State = gameSettings.deadCell };
                        cellsToUpdate.Add(cell);
                    }
                    if (bornCell)
                    {
                        Cells cell = new Cells() { X = i, Y = j, State = (char)('0' + dominantPopulation) };
                        cellsToUpdate.Add(cell);
                    }                    
                }
            }
            return cellsToUpdate;
        }
        static List<Cells> CalculateAgressiveCellsToChangeState(char[,] oldStateField)
        {
            List<Cells> cellsToUpdate = new List<Cells>();
            int[] neighboursCount = new int[gameSettings.populations];
            for (int j = 0; j < gameSettings.fieldSizeY; j++)
            {
                for (int i = 0; i < gameSettings.fieldSizeX; i++)
                {
                    neighboursCount = CalculateNeighbours(i, j, oldStateField);
                    int allNeighboursCount = neighboursCount.Sum();
                    int amountOfDominantPopulation = neighboursCount.Max();
                    int dominantPopulation = Array.IndexOf(neighboursCount, amountOfDominantPopulation);
                    bool bornCell = false;
                    bool dieCell = false;

                    if (oldStateField[i, j] == gameSettings.deadCell)
                    {
                        for (int n = 0; n < gameSettings.populations; n++)
                        {
                            int otherNeighboursCount = allNeighboursCount - neighboursCount[n];
                            if (neighboursCount[n] == gameSettings.amountToBorn && neighboursCount[n] >= otherNeighboursCount)
                            {
                                bornCell = true;
                                break;
                            }
                            else if (neighboursCount[n] == gameSettings.amountToBornAgressiveMode && neighboursCount[n] > otherNeighboursCount && otherNeighboursCount > 0)
                            {
                                bornCell = true;
                                break;
                            }
                        }

                    }
                    else
                    {
                        int thisNeighboursCount = neighboursCount[oldStateField[i, j] - '0'];
                        int otherNeighboursCount = allNeighboursCount - thisNeighboursCount;
                        if (thisNeighboursCount < gameSettings.minAmountToAlive)
                        {
                            if (amountOfDominantPopulation == gameSettings.amountToBorn || amountOfDominantPopulation == gameSettings.amountToBornAgressiveMode)
                            {
                                bornCell=true;
                            }
                            else
                            {
                                dieCell=true;
                            }
                        }
                        else if (thisNeighboursCount <= gameSettings.maxAmountToAlive)
                        {
                            if (amountOfDominantPopulation >= thisNeighboursCount + gameSettings.amountOfExcessToCapture)
                            {
                                bornCell = true;
                            }
                            else if (allNeighboursCount > gameSettings.maxAmountSimbioticToAlive)
                            {
                                dieCell = true;
                            }
                        }
                        else
                        {
                            dieCell = true;
                        }
                    }
                    if (dieCell)
                    {
                        Cells cell = new Cells() { X = i, Y = j, State = gameSettings.deadCell };
                        cellsToUpdate.Add(cell);
                    }
                    if (bornCell)
                    {
                        Cells cell = new Cells() { X = i, Y = j, State = (char)('0' + dominantPopulation) };
                        cellsToUpdate.Add(cell);
                    }
                }
            }
            return cellsToUpdate;
        }
        static int[] CalculateNeighbours(int xPos, int yPos, char[,] oldStateField)
        {
            int[] neighbours = new int[gameSettings.populations];
            for (int i = 0; i < gameSettings.populations; i++)
            {
                if (yPos > 0 && xPos > 0 && oldStateField[xPos - 1, yPos - 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos > 0 && oldStateField[xPos, yPos - 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos > 0 && xPos < gameSettings.fieldSizeX - 1 && oldStateField[xPos + 1, yPos - 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (xPos > 0 && oldStateField[xPos - 1, yPos] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (xPos < gameSettings.fieldSizeX - 1 && oldStateField[xPos + 1, yPos] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos < gameSettings.fieldSizeY - 1 && xPos > 0 && oldStateField[xPos - 1, yPos + 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos < gameSettings.fieldSizeY - 1 && oldStateField[xPos, yPos + 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
                if (yPos < gameSettings.fieldSizeY - 1 && xPos < gameSettings.fieldSizeX - 1 && oldStateField[xPos + 1, yPos + 1] == gameSettings.aliveCell[i])
                { neighbours[i]++; }
            }
            return neighbours;
        }
    }
}