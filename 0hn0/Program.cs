using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
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

            Console.WriteLine("do interraction");
            Console.ReadLine();

            TileInfo[][] tiles = ReadWebTiles(webDriver);
            TileInfo[][] tilesRot = RotateMatrix(tiles);

            Algorithm(tiles); // this will solve the game

            PrintResult(tiles, webDriver);

            isRunning = false; // to only get it run once, could have been done with a do-while loop, but idc
        }
    }
    static void Algorithm(TileInfo[][] tiles) {// this is the algorithm htat solves the game
        foreach (var tile in notFulfilled) {
            //Fulfill(tile);

        }
        DeadEndController(tiles);
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
                    UpdateDirections(new Position(i, j), direction);
                    
                }
            }
        }
    }
    static bool UpdateDirections(Position position, Directions direction) {

        position += Position.GrowthVector(direction);
        switch (TileDict[position.ToString()].State) {
            case TileState.Empty:
                return false;
                break;
            case TileState.Red:
                return true;
                break;
            case TileState.Blue:
                //return true;
                //tiles
                return UpdateDirections(position, direction);
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
        Position growth = Position.GrowthVector(direction);
        Position target = tile.Posistion;
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
        if (totalCount != tile.DesiredNumber) return false;
        foreach (Directions direction in tile.OpenDirections) {
            MarkBlueTiles(tile, direction, CountDirection(tile, direction));
        }

        TileInfo.notFulfilled.Remove(tile);
        tile.IsFulfilled = true;
        return true;
    }
    static bool CloseFinishedEnd(TileInfo tile) {
        if (DoCount(tile) != tile.DesiredNumber) return false;


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
        Position growth = Position.GrowthVector(direction); // functions like a vector in given direction
        int count = 0;
        Position target = tile.Posistion;
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
                tiles[i][j] = new TileInfo(board.FindElement(By.Id($"tile-{i}-{j}")), new Position(i, j));
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
