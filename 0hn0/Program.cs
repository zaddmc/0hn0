using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


namespace _0hn0;

internal class Program {
    public static int gridSize = 4;
    public static TileInfo[][] tiles;

    static void Main(string[] args) {



        WebDriver webDriver = new ChromeDriver();
        webDriver.Navigate().GoToUrl("https://0hn0.com/");


        bool isRunning = true;
        while (isRunning) {
            webDriver.ExecuteScript($"Game.startGame({gridSize},0)");
            Thread.Sleep(1000);

            ReadWebTiles(webDriver);

            Algorithm(); // this will solve the game


            isRunning = false; // to only get it run once, could have been done with a do-while loop, but idc
        }
    }
    static void Algorithm() {// this is the algorithm htat solves the game

    }
    static void ReadWebTiles(IWebDriver webDriver) {
        IWebElement board = webDriver.FindElement(By.Id("board"));

        tiles = new TileInfo[gridSize][];
        for (int i = 0; i < gridSize; i++) {
            tiles[i] = new TileInfo[gridSize];
            for (int j = 0; j < gridSize; j++) {
                tiles[i][j] = new TileInfo(board.FindElement(By.Id($"tile-{i}-{j}")));
            }
        }
    }
    static bool IsDone() {
        return false;
    }
}
