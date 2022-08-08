namespace GameOfLife
{
    internal class SettingsMenu
    {
        public static Settings gameSettings = new Settings();
        internal static void OptionsMenu()
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
                Console.SetCursorPosition(7, 17);
                Console.Write("Выйти из игры: \"X\"");
                keyPressed = Console.ReadKey(true);
                switch (keyPressed.Key)
                {
                    case ConsoleKey.UpArrow: gameSettings.fieldSizeY += gameSettings.fieldSizeY < gameSettings.maxFieldSizeY ? 1 : 0; GameField.ResizeField(); restart = true; break;//увеличение высоты игрового поля
                    case ConsoleKey.DownArrow: gameSettings.fieldSizeY -= gameSettings.fieldSizeY > gameSettings.minFieldSize ? 1 : 0; GameField.ResizeField(); restart = true; break;//уменьшение высоты игрового поля
                    case ConsoleKey.LeftArrow: gameSettings.fieldSizeX -= gameSettings.fieldSizeX > gameSettings.minFieldSize ? 1 : 0; GameField.ResizeField(); restart = true; break;//уменьшение ширины игрового поля
                    case ConsoleKey.RightArrow: gameSettings.fieldSizeX += gameSettings.fieldSizeX < gameSettings.maxFieldSizeX ? 1 : 0; GameField.ResizeField(); restart = true; break;//увеличение ширины игрового поля
                    case ConsoleKey.A: gameSettings.density -= gameSettings.density > 1 ? 1 : 0; restart = true; break;
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
                Program.StartGame();
            }
        }
        private static void DrawMenuBorders()
        {
            Console.ForegroundColor = gameSettings.menuBorderColor;
            Console.SetCursorPosition(0, 0);
            Console.Write("#");
            for (int i = 0; i < gameSettings.menuSizeX - 2; i++)
            {
                Console.Write("=");
            }
            Console.WriteLine("#");
            for (int i = 0; i < gameSettings.menuSizeY - 2; i++)
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
        internal static void ReDrawFieldUnderMenu(char[,] fieldState)
        {
            Console.SetCursorPosition(0, 0);
            for (int j = 0; j <= (fieldState.GetUpperBound(1) > gameSettings.menuSizeY ? gameSettings.menuSizeY : fieldState.GetUpperBound(1)); j++)
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
            if (gameSettings.fieldSizeY<gameSettings.menuSizeY)
            {
                GameField.DrawStatusLine();
            }
        }
        private static void ConfirmExit()
        {
            DrawMenuBorders();
            ConsoleKeyInfo keyPressed;
            Console.SetCursorPosition(9, 5);
            Console.WriteLine("Выйти из игры?");
            Console.SetCursorPosition(14, 8);
            Console.WriteLine("y/n");
            Thread.Sleep(300);
            keyPressed = Console.ReadKey(true);
            switch (keyPressed.Key)
            {
                case ConsoleKey.Y: case ConsoleKey.Enter: Program.exit = true; break;
                default: OptionsMenu(); break;                         //возврат в меню
            }
        }
    }
}