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


            isRunning = false; // to only get it run once, could have been done with a do-while loop, but idc
        }
    }
    static void Algorithm() {// this is the algorithm htat solves the game

    }
    static void updateDirectionsController(TileInfo[][] tiles) {
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                foreach (Directions direction in tiles[i][j].OpenDirections) {
                    switch (direction) {
                        case Directions.Up:
                            updateDirections(tiles[i][j++],tiles[i][j++].State, Directions.Up);
                            break;
                        case Directions.Right:
                            updateDirections(tiles[i++][j], tiles[i++][j].State, Directions.Right);
                            break;
                        case Directions.Down:
                            updateDirections(tiles[i][j--], tiles[i][j--].State, Directions.Down);
                            break;
                        case Directions.Left:
                            updateDirections(tiles[i--][j], tiles[i--][j].State, Directions.Left);
                            break;
                        default:
                            break;

                    }
                }
            }
        }
    }
    static bool updateDirections(TileInfo tile, TileState color, Directions direction) {
        
        switch (tile.State) {
            case TileState.Empty:
                return false;
                break;
            case TileState.Red:
                return true;
                break;
            case TileState.Blue:
                //return true;
                tiles
                break;
            default:
                break;
        } return false;
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


    static bool CloseFinishedEndsController() {



        return false;
    }
    static bool CloseFinishedEnds() {
        foreach (var tile in TileInfo.notFulfilled) {
            foreach (var direction in tile.OpenDirections) {

            }





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
        Posistion growth; // functions like a vector in given direction
        switch (direction) {
            case TileInfo.Directions.Up: growth = new(-1, 0); break;
            case TileInfo.Directions.Right: growth = new(0, 1); break;
            case TileInfo.Directions.Down: growth = new(1, 0); break;
            case TileInfo.Directions.Left: growth = new(0, -1); break;
            default: growth = new(0, 0); break;
        }
        int count = 0;
        Posistion target = tile.Posistion;
        while (true) {
            target += growth;
            if (!target.IsInBounds()) return count;
            count++;
        }
    }
    static TileInfo[][] ReadWebTiles(IWebDriver webDriver) {

        TileInfo.notFulfilled = new List<TileInfo>(); //initialzing or clearing the static internal list


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
