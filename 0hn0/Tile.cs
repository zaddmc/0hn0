using OpenQA.Selenium;

namespace _0hn0;
public class TileInfo {
    public enum TileState {
        Empty,
        Blue,
        Red,
    }
    public TileState State { get; set; }
    public IWebElement WebElement { get; set; }
    public Posistion? Posistion { get; set; }
    public bool IsLocked { get; set; }

    public TileInfo(IWebElement element) {
        WebElement = element;
        switch (element.GetAttribute("class")) {
            case "tile tile-":
                State = TileState.Empty;
                IsLocked = false;
                break;
            case "tile tile-1":
                State = TileState.Blue;
                IsLocked = true;
                break;
            case "tile tile-2":
                State = TileState.Red;
                IsLocked = true;
                break;
            default:
                break;
        }
    }
}
public class Posistion(int i, int j) { // this is a an obejecct to resemble a posistion in a larger system
    public int I { get; set; } = i; // this is a variable in an possible object
    public int J { get; set; } = j; // this is a variable in an possible object
}

