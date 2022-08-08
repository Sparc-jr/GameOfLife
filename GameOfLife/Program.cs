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
        internal static Settings gameSettings = SettingsMenu.gameSettings;
        internal static bool exit = false;
        static void Main(string[] args)
        {
            Console.Title = "GAME OF LIFE";
            Program.StartGame();          
        }
        internal static void StartGame()
        {
            int width = gameSettings.fieldSizeX;
            int height = gameSettings.fieldSizeY;
            Console.CursorVisible = false;
            GameField.ResizeField();
            char[,] fieldCurrentState = new char[width,height];
            fieldCurrentState = GameField.GenerateStartField(width, height);
            GameField.DrawField(fieldCurrentState);
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
                    GameField.DrawFieldUpdateDebugMode(fieldCurrentState,cellsWithChanges);
                    Console.ReadLine();
                    GameField.DrawFieldUpdate(cellsWithChanges);
                    Console.ReadLine();
                } else
                {
                    GameField.DrawFieldUpdate(cellsWithChanges);
                }
                GameField.UpdateFieldState(fieldCurrentState, cellsWithChanges);
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(gameSettings.realSpeed[gameSettings.speed - 1] * gameSettings.speedCorrection /
                             gameSettings.fieldSizeX / gameSettings.fieldSizeY);
                cellsWithChanges.Clear();
                if (Console.KeyAvailable)
                {
                    keyPressed = Console.ReadKey(true);
                    switch (keyPressed.Key)
                    {
                        case ConsoleKey.Escape: SettingsMenu.OptionsMenu(); SettingsMenu.ReDrawFieldUnderMenu(fieldCurrentState); break;
                        case ConsoleKey.R: Console.Clear(); StartGame() ;break;
                        case ConsoleKey.H: gameSettings.debugMode = !gameSettings.debugMode; break;
                    }
                }
            }
            while (!exit);
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
                if (yPos < gameSettings.fieldSizeY - 1 && xPos > 0 && oldStateField[xPos - 1, yPos + 1] == 
                    gameSettings.aliveCell[i])
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