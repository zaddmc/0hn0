using OpenQA.Selenium;
using OpenQA.Selenium.DevTools.V121.LayerTree;
using OpenQA.Selenium.DevTools.V121.Overlay;
using System.Collections.Generic;
using static _0hn0.TileInfo;

namespace _0hn0;
public class TileInfo {
    public static List<TileInfo> notFulfilled { get; set; }
    public static Dictionary<string, TileInfo> TileDict { get; set; }
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
    public IWebElement WebElement { get; private set; }
    public Position Posistion { get; private set; }
    public bool IsLocked { get; private set; }
    public bool IsFulfilled { get; set; } = false;
    public int DesiredNumber { get; private set; }
    public int CurrentCount { get; set; }

    public TileInfo(IWebElement element, Position posistion) {
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

        TileDict.Add(posistion.ToString(), this);
    }
    public static TileState[][] CopyBoard(TileInfo[][] tiles) {
        TileState[][] states = new TileState[Program.gridSize][];
        for (int i = 0; i < Program.gridSize; i++) {
            states[i] = new TileState[Program.gridSize];
            for (int j = 0; j < Program.gridSize; j++) {
                states[i][j] = tiles[i][j].State;
            }
        }
        return states;
    }
    public static bool CompareBoard(TileInfo[][] tiles, TileState[][] states) {
        for (int i = 0; i < Program.gridSize; i++) {
            for (int j = 0; j < Program.gridSize; j++) {
                if (tiles[i][j].State != states[i][j]) return false;
            }
        }
        return true;
    }
}
public class Position(int i, int j) { // this is a an obejecct to resemble a posistion in a larger system
    public int I { get; set; } = i; // this is a variable in an possible object
    public int J { get; set; } = j; // this is a variable in an possible object
    public static Position operator +(Position main, Position other) {
        return new Position(main.I + other.I, main.J + other.J);
    }
    public static Position operator *(Position main, int x) {
        return new Position(main.I * x, main.J * x);
    }
    public bool IsInBounds() {
        if (I < 0) return false;
        if (I > Program.gridSize - 1) return false;
        if (J < 0) return false;
        if (J > Program.gridSize - 1) return false;
        return true;
    } 
    public override string ToString() {
        return $"{I}:{J}";
    }
    public TileInfo ToTile() {
        return TileInfo.TileDict[this.ToString()];
    }
    static public Position GrowthVector(TileInfo.Directions direction) {
        switch (direction) {
            case Directions.Up: return new(0, -1); 
            case Directions.Right: return new(1, 0); 
            case Directions.Down: return new(0, 1); 
            case Directions.Left: return new(-1, 0); 
            default: return new(0, 0); 
        }
    }
}

