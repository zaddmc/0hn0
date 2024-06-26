﻿using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Threading;
using static _0hn0.Direction;
using static _0hn0.TileInfo;


namespace _0hn0;

internal class Program {
    public static int gridSize = 9;

    static void Main(string[] args) {



        WebDriver webDriver = new ChromeDriver();
        webDriver.Navigate().GoToUrl("https://0hn0.com/");


        bool isRunning = true;
        int runs = 100;
        while (isRunning) {
            webDriver.ExecuteScript($"Game.startGame({gridSize},0)");
            Thread.Sleep(1000);

            TileInfo[][] tiles = ReadWebTiles(webDriver);
            TileInfo[][] tilesRot = RotateMatrix(tiles);


            bool block = !Algorithm(tiles); // this will solve the game
            
            PrintResult(tiles, webDriver);

            if (block) { //wait for interraction
                Console.WriteLine("do interraction");
                Console.ReadLine();
                Algorithm(tiles); // to test in boards where it failed
            }
            Thread.Sleep(1000);

            if (runs <= 0) isRunning = false;
            runs--;

        }
        Console.WriteLine("done the deed");
    }
    static bool Algorithm(TileInfo[][] tiles) {// this is the algorithm that solves the game
        TileState[][] preStates;
        do {
            preStates = CopyBoard(tiles);
            UpdateDirectionsController(tiles);
            DeadEndController(tiles);
            foreach (TileInfo tile in notFulfilled) {
                SimpleFill(tile);
                FillWithOpenEnds(tile);
                OverflowSolver(tile);
                //LastOpenFill(tile);
            }

            for (int i = notFulfilled.Count - 1; i >= 0; i--)
                if (notFulfilled[i].DesiredNumber == notFulfilled[i].CurrentCount)
                    notFulfilled.RemoveAt(i);

        } while (!CompareBoard(tiles, preStates));
        return IsDone(tiles);

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
    static void UpdateDirectionsController(TileInfo[][] tiles) {//Controls. makes an daouble array, that then seaches for endpoints in every direction
        for (int i = 0; i < gridSize; i++) {
            for (int j = 0; j < gridSize; j++) {
                for (int number = tiles[i][j].Direction.OpenDirections.Count; number > 0; number--) { //searches open directions and then checks the opendirections if there are red in them
                    if (UpdateDirections(new Position(i, j), tiles[i][j].Direction.OpenDirections[number - 1], tiles[i][j])) {
                        tiles[i][j].Direction.OpenDirections.Remove(tiles[i][j].Direction.OpenDirections[number - 1]); //deletes the direction if there are red
                    }
                }
            }
        }
    }
    static bool UpdateDirections(Position position, Directions direction, TileInfo tile) {
        position += Position.GrowthVector(direction);
        if (!position.IsInBounds()) return true;
        switch (TileDict[position.ToString()].State) { //switch to differentiate between colors, if it is red, then it deletes the direction bcuz it returns true.
            case TileState.Empty: return false;
            case TileState.Red: return true;
            case TileState.Blue:
                tile.Direction.Add(tile, position.ToTile(), direction);
                return UpdateDirections(position, direction, tile);
            default: break;
        }
        return false;
    }
    static bool DeadEndController(TileInfo[][] tiles) {
        bool returnState = false;
        for (byte i = 0; i < gridSize; i++) {
            for (byte j = 0; j < gridSize; j++) {
                uint amountOfDeadEnds = 0;
                foreach (Directions direction in Direction.AllDirections) {
                    if (IsDeadEnd(tiles, tiles[i][j].Position, direction)) {
                        amountOfDeadEnds++;
                    }
                }
                if (amountOfDeadEnds == 4) {
                    tiles[i][j].State = TileState.Red;
                }
            }
        }
        return returnState;
    }

    static bool IsDeadEnd(TileInfo[][] tiles, Position position, Directions direction) {
        position += Position.GrowthVector(direction);
        if (!position.IsInBounds()) return true;
        switch (tiles[position.I][position.J].State) {
            case TileState.Empty: return false;
            case TileState.Red: return true;
            case TileState.Blue: return false;
            default: return false;
        }
    }
    static bool LastOpenFill(TileInfo tile) {
        bool returnState = false;
        if (tile.Direction.OpenDirections.Count is 0 or >1) {
            return returnState;
        }
        int amountOfBlue = DoCount(tile).BlueCount;
        Directions direction = tile.Direction.OpenDirections[0];
        returnState = MarkBlueTiles(tile, tile.Direction.OpenDirections[0], tile.DesiredNumber - amountOfBlue + CountDirection(tile, direction).BlueCount);
        MarkRedTile(tile, tile.Direction.OpenDirections[0], tile.DesiredNumber - amountOfBlue + CountDirection(tile, direction).BlueCount); //change true to this later, so the markasred is not needed
        return returnState;
    }

    static bool OverflowSolver(TileInfo tile) {

        for (int i = 0; i < tile.Direction.OpenDirections.Count; i++) {
            Directions direction = tile.Direction.OpenDirections[i];
            Position growthVector = Position.GrowthVector(direction);
            Position target = tile.Position;

            int potentielAddCount = 0;
            while (target.IsInBounds() && target.ToTile().State != TileState.Red) {
                if (target.ToTile().State == TileState.Empty) {
                    potentielAddCount += CountDirection(target.ToTile(), direction).BlueCount;
                    break;
                }
                target += growthVector;
            }
            if(DoCount(tile).BlueCount + potentielAddCount + 1> tile.DesiredNumber) {
                MarkRedTile(tile, direction, CountDirection(tile, direction).BlueCount);
            }
        }

        return false;
    }
    static bool FillWithOpenEnds(TileInfo tile) {
        List<int> openEnds = new List<int>();
        for (int i = 0; i < tile.Direction.SemiOpenDirections.Count; i++)
            openEnds.Add(CountDirection(tile, tile.Direction.SemiOpenDirections[i]).AvailableCount);

        for (int i = 0; i < openEnds.Count; i++) {
            int tally = 0;
            for (int j = 0; j < openEnds.Count; j++) {
                if (i == j) continue;
                tally += openEnds[j];
            }
            if (tally <= tile.DesiredNumber)
                MarkBlueTiles(tile, tile.Direction.SemiOpenDirections[i], tile.DesiredNumber - tally);
        }
        return false;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="direction"></param>
    /// <param name="count"></param>
    /// <param name="markRed"></param>
    /// <returns></returns>
    static bool MarkBlueTiles(TileInfo tile, Directions direction, int count, bool markRed = false) {
        Position growth = Position.GrowthVector(direction);
        Position target = tile.Position;
        for (int i = 0; i < count; i++) {
            target += growth;
            if (!target.IsInBounds()) return false;

            TileInfo targetTile = target.ToTile();
            if (targetTile.State == TileState.Red) return false;

            if (targetTile.State == TileState.Blue) continue;

            targetTile.State = TileState.Blue;
            //target.CurrentCount++;
        }
        if (markRed)
            MarkRedTile(tile, direction, count);

        return true;
    }
    static bool SimpleFill(TileInfo tile) {
        (int blueCount, int availCount) theCount = DoCount(tile);
        if (theCount.blueCount == tile.DesiredNumber) {
            foreach (Directions direction in tile.Direction.OpenDirections) {
                int countDirection = CountDirection(tile, direction).BlueCount;
                MarkBlueTiles(tile, direction, countDirection);
                MarkRedTile(tile, direction, countDirection);
            }
        }
        else if (theCount.availCount == tile.DesiredNumber) {
            foreach (Directions direction in tile.Direction.OpenDirections) {
                int countDirection = CountDirection(tile, direction).AvailableCount;
                MarkBlueTiles(tile, direction, countDirection);
                MarkRedTile(tile, direction, countDirection);
            }
        }
        else return false;

        return true;
    }
    /// <summary>
    /// Will mark 1 tile red at the end of count
    /// </summary>
    /// <param name="tile">The tile of origin</param>
    /// <param name="direction">The direction of the tile to mark a Red tile</param>
    /// <param name="count">How far out the red tile should be</param>
    /// <returns>Returns a bool where true would mean it succesfully marked a tile red or already red, and false if it failed to mark red</returns>
    /// <exception cref="ArgumentException">If the tile that should be marked red was blue it will throw this exception</exception>
    static bool MarkRedTile(TileInfo tile, Directions direction, int count) {
        //if (DoCount(tile).BlueCount != tile.DesiredNumber) return false;
        Position growth = Position.GrowthVector(direction);
        Position target = tile.Position + growth * (count + 1);
        if (!target.IsInBounds()) return false;
        switch (target.ToTile().State) {
            case TileState.Empty:
                target.ToTile().State = TileState.Red;
                return true;
            case TileState.Red: return true;
            case TileState.Blue: throw new ArgumentException("wants to make red, but it blue");
            default: return false;
        }
    }
    /// <summary>
    /// Counts all possible directions of given tile
    /// </summary>
    /// <param name="tile">The tile of which will be orign of count</param>
    /// <returns>Bluecount all continous blueconnected to the origin tile, Availcount any open tile conneceted to the origin tile</returns>
    static (int BlueCount, int AvailableCount) DoCount(TileInfo tile) {
        int blueCount = 0, availableCount = 0;
        foreach (Directions direction in Direction.AllDirections) {
            var temp = CountDirection(tile, direction);
            blueCount += temp.BlueCount;
            availableCount += temp.AvailableCount;
        }
        return (blueCount, availableCount);
    }
    /// <summary>
    /// Count given direction
    /// </summary>
    /// <param name="tile">The tile of Origin to count from</param>
    /// <param name="direction">The direction in which to count</param>
    /// <returns>BlueCount is how many Blues in given direction that is connected to the origin tile, AvailableCount is all free tiles including blue and gray, only stops at red tiles, which arent</returns>
    static (int BlueCount, int AvailableCount) CountDirection(TileInfo tile, Directions direction, bool includeLatch = true) {
        Position growth = Position.GrowthVector(direction); // functions like a vector in given direction
        int blueCount = 0, availableCount = 0;
        Position target = tile.Position;
        bool latch = false;
        while (true) {
            target += growth;
            if (!target.IsInBounds()) return (blueCount, availableCount);
            if (target.ToTile().State == TileState.Red) return (blueCount, availableCount);
            availableCount++;
            if (target.ToTile().State == TileState.Empty || latch && includeLatch) { latch = true; continue; }
            blueCount++;
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
