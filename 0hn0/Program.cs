using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;


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


        }

    }
}
