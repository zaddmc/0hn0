using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.Threading;


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
    static bool CloseFinishedEndsController() {



        return false;
    }
    static bool CloseFinishedEnds(TileInfo[] tiles) {



        return false;
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
