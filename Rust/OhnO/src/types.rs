#[derive(Debug)]
pub struct Cell {
    pub row: usize,
    pub col: usize,
    pub color: CellColor,
    pub number: Option<u8>, // Some(n) for blue dots with a number
}

#[derive(Debug, PartialEq)]
pub enum CellColor {
    Dot,
    Wall,
    Empty,
}
