using OpenQA.Selenium;
using System.Collections.Generic;

namespace _0hn0;
public class TileInfo {
    public static List<TileInfo> notFulfilled { get; set; }
    public enum TileState {
        Empty,
        Red,
        Blue,
    }
    public TileState State { get; set; }
    public enum Directions {
        Up,
        Right,
        Down,
        Left,
    }
    public List<Directions> OpenDirections { get; set; } = [Directions.Up, Directions.Right, Directions.Down, Directions.Left];
    public IWebElement WebElement { get; set; }
    public Posistion? Posistion { get; set; }
    public bool IsLocked { get; set; }
    public bool IsFulfilled { get; set; } = false;
    public int DesiredNumber { get; set; }
    public int CurrentCount { get; set; }

    public TileInfo(IWebElement element, Posistion posistion) {
        WebElement = element;
        Posistion = posistion;
        switch (element.GetAttribute("class")) {
            case "tile tile-":
                State = TileState.Empty;
                IsLocked = false;
                break;
            case "tile tile-1":
                State = TileState.Red;
                IsLocked = true;
                break;
            case "tile tile-2":
                State = TileState.Blue;
                IsLocked = true;
                int.TryParse(element.Text, out int temp);
                DesiredNumber = temp;
                notFulfilled.Add(this);
                break;
            default:
                break;
        }
        if (Posistion.I == 0) OpenDirections.Remove(Directions.Left);
        if (Posistion.I == Program.gridSize - 1) OpenDirections.Remove(Directions.Right);
        if (Posistion.J == 0) OpenDirections.Remove(Directions.Up);
        if (Posistion.J == Program.gridSize - 1) OpenDirections.Remove(Directions.Down);
    }
}
public class Posistion(int i, int j) { // this is a an obejecct to resemble a posistion in a larger system
    public int I { get; set; } = i; // this is a variable in an possible object
    public int J { get; set; } = j; // this is a variable in an possible object
}

