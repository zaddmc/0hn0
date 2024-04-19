using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading;
using static _0hn0.TileInfo;


namespace _0hn0;

internal class Program {
    public static int gridSize = 4;

    static void Main(string[] args) {



        WebDriver webDriver = new ChromeDriver();
        webDriver.Navigate().GoToUrl("https://0hn0.com/");


        bool isRunning = true;
        while (isRunning) {
            webDriver.ExecuteScript($"Game.startGame({gridSize},0)");
            Thread.Sleep(1000);

            TileInfo[][] tiles = ReadWebTiles(webDriver);
            TileInfo[][] tilesRot = RotateMatrix(tiles);

            Algorithm(); // this will solve the game

            PrintResult(tiles, webDriver);

            isRunning = false; // to only get it run once, could have been done with a do-while loop, but idc
        }
    }
    static void Algorithm() {// this is the algorithm htat solves the game
        MarkBlueTiles(notFulfilled[1], Directions.Down, 2);
        MarkBlueTiles(notFulfilled[1], Directions.Left, 2);
    }
    static void PrintResult(TileInfo[][] tiles, WebDriver webDriver) {
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                TileInfo theTile = tiles[i][j];
                if (theTile.IsLocked) continue;
                switch (theTile.State) {
                    case TileState.Empty:
                        break;
                    case TileState.Blue:
                        theTile.WebElement.Click();
                        break;
                    case TileState.Red:
                        theTile.WebElement.Click();
                        theTile.WebElement.Click();
                        break;
                    default:
                        break;
                }
            }
        }
    }
    static void UpdateDirectionsController(TileInfo[][] tiles) {
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                foreach (Directions direction in tiles[i][j].OpenDirections) {
                    UpdateDirections(new Posistion(i, j), tiles, direction);
                    switch (direction) {
                        case Directions.Up:
                            UpdateDirections(new Posistion(i, j), tiles, Directions.Up);
                            break;
                        case Directions.Right:
                            UpdateDirections(new Posistion(i, j), tiles, Directions.Right);
                            break;
                        case Directions.Down:
                            UpdateDirections(new Posistion(i, j), tiles, Directions.Down);
                            break;
                        case Directions.Left:
                            UpdateDirections(new Posistion(i, j), tiles, Directions.Left);
                            break;
                        default:
                            break;

                    }
                }
            }
        }
    }
    static bool UpdateDirections(Posistion position, TileInfo[][] tiles, Directions direction) {

        position += Posistion.GrowthVector(direction);
        switch (tiles[position.I][position.J].State) {
            case TileState.Empty:
                return false;
                break;
            case TileState.Red:
                return true;
                break;
            case TileState.Blue:
                //return true;
                //tiles
                return UpdateDirections(new Posistion(11, 2), tiles, direction);
                break;
            default:
                break;
        }
        return false;
    }
    static bool DeadEndController(TileInfo[][] tiles) {
        bool returnState = false;
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                if (tiles[i][j].OpenDirections.Count == 0) {
                    tiles[i][j].State = TileState.Red;
                    returnState = true;
                }
            }
        }
        return returnState;
    }

    static bool MarkBlueTiles(TileInfo tile, Directions direction, int count) {
        Posistion growth = Posistion.GrowthVector(direction);
        Posistion target = tile.Posistion;
        for (int i = 0; i < count; i++) {
            target += growth;
            if (!target.IsInBounds()) return false;

            TileInfo targetTile = target.ToTile();
            if (targetTile.State == TileState.Red) return false;

            if (targetTile.State == TileState.Blue) continue;

            targetTile.State = TileState.Blue;
            targetTile.CurrentCount++;
        }



        return true;
    }
    static bool Fulfill(TileInfo tile) {
        int totalCount = DoCount(tile);



        return false;
    }
    static bool CloseFinishedEnd(TileInfo tile) {

        foreach (var direction in tile.OpenDirections) {

        }




        return false;
    }
    static int DoCount(TileInfo tile) {
        int totalCount = 0;
        foreach (var direction in tile.OpenDirections) {
            totalCount += CountDirection(tile, direction);
        }
        return totalCount;
    }
    static int CountDirection(TileInfo tile, TileInfo.Directions direction) {
        Posistion growth = Posistion.GrowthVector(direction); // functions like a vector in given direction
        int count = 0;
        Posistion target = tile.Posistion;
        while (true) {
            target += growth;
            if (!target.IsInBounds()) return count;
            if (target.ToTile().State == TileState.Red) return count;
            count++;
        }
    }
    static TileInfo[][] ReadWebTiles(IWebDriver webDriver) {

        TileInfo.notFulfilled = new List<TileInfo>(); //initialzing or clearing the static internal list
        TileInfo.TileDict = new Dictionary<string, TileInfo>();

        IWebElement board = webDriver.FindElement(By.Id("board"));

        TileInfo[][] tiles = new TileInfo[gridSize][];
        for (int i = 0; i < gridSize; i++) {
            tiles[i] = new TileInfo[gridSize];
            for (int j = 0; j < gridSize; j++) {
                tiles[i][j] = new TileInfo(board.FindElement(By.Id($"tile-{i}-{j}")), new Posistion(i, j));
            }
        }
        return tiles;
    }
    static TileInfo[][] RotateMatrix(TileInfo[][] tiles) {
        TileInfo[][] tilesRot = new TileInfo[gridSize][];
        for (int i = 0; i < gridSize; i++) {
            tilesRot[i] = new TileInfo[gridSize];
        }

        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                tilesRot[j][i] = tiles[i][j];
            }
        }


        return tilesRot;
    }
    static bool IsDone(TileInfo[][] tiles) {
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                if (tiles[i][j].State == TileInfo.TileState.Empty) return false;
            }
        }
        return true;
    }
}
