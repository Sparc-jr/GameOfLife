namespace GameOfLife
{
    internal class GameField
    {
        public static Settings gameSettings = SettingsMenu.gameSettings;
        internal static void ResizeField()
        {
            Console.SetWindowSize(width: gameSettings.fieldSizeX > gameSettings.menuSizeX ? gameSettings.fieldSizeX + 1 : gameSettings.menuSizeX + 1,
                                  height: gameSettings.fieldSizeY > gameSettings.menuSizeY-2 ? gameSettings.fieldSizeY + 3 : gameSettings.menuSizeY + 1);
        }
        internal static char[,] GenerateStartField(int width, int height)
        {
            var rand = new Random();
            char[,] fieldNow = new char[width, height];
            for (int j = 0; j < height; j++)
            {
                for (int i = 0; i < width; i++)
                {
                    fieldNow[i, j] = (rand.Next(20) < gameSettings.density ? gameSettings.aliveCell[rand.Next(gameSettings.populations)] : gameSettings.deadCell);
                }

            }
            return fieldNow;
        }
        internal static void DrawField(char[,] fieldState)
        {
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j < gameSettings.fieldSizeY; j++)
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
                    Console.Write(fieldState[i, j]);
                }
                Console.WriteLine();
            }
            DrawStatusLine();
        }
        internal static void DrawFieldUpdate(List<Cells> changingCells)
        {
            foreach (Cells cell in changingCells)
            {
                Console.SetCursorPosition(cell.X, cell.Y);
                if (cell.State != gameSettings.deadCell)
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
        internal static void DrawFieldUpdateDebugMode(char[,] fieldState, List<Cells> changingCells)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (Cells cell in changingCells)
            {
                Console.SetCursorPosition(cell.X, cell.Y);
                Console.Write(fieldState[cell.X, cell.Y]);
            }
        }
        internal static void UpdateFieldState(char[,] fieldState, List<Cells> changingCells)
        {
            foreach (Cells cell in changingCells)
            {
                fieldState[cell.X, cell.Y] = cell.State;
            }
        }
        internal static void DrawStatusLine()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.SetCursorPosition(0, gameSettings.fieldSizeY);
            for (int i = 0; i < gameSettings.fieldSizeX; i++)
            {
                Console.Write("_");
            }
            Console.WriteLine();
            Console.WriteLine($"Популяций: {gameSettings.populations,2}"); 
            Console.WriteLine($"Взаимодействие: {gameSettings.cellsInteraction,14}");
            
        }
    }
}