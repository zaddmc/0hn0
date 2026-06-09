use chromiumoxide::browser::{Browser, BrowserConfig};
use chromiumoxide::page::Page;
use futures::StreamExt;
use tokio::task::JoinHandle;

use crate::types::Cell;

pub struct BrowserSession {
    pub browser: Browser,
    pub page: Page,
    handle: JoinHandle<()>,
}
impl BrowserSession {
    pub async fn new(url: &str) -> Result<Self, Box<dyn std::error::Error>> {
        let (browser, mut handler) = Browser::launch(
            BrowserConfig::builder()
                .with_head() // use .headless() for no window
                .build()?,
        )
        .await?;

        // The handler drives the browser event loop — run it in the background
        let handle = tokio::spawn(async move {
            loop {
                let _event = handler.next().await.unwrap();
            }
        });

        let page = browser.new_page(url).await?;
        tokio::time::sleep(std::time::Duration::from_secs(1)).await;
        println!("Opened {}", url);

        Ok(Self { browser, page, handle })
    }

    pub async fn gather(&self, size: i32) -> Result<Vec<Cell>, Box<dyn std::error::Error>> {
        self.page.evaluate("Game.startGame(9)").await?; // TODO: Make it dependant on size
        println!("Called Game.startGame(9)");

        // Give the game a moment to render the grid
        tokio::time::sleep(std::time::Duration::from_secs(1)).await;

        let result = self.page.evaluate("Game.grid.getValues()").await?;
        let json = result.value();
        if json.is_none() {
            println!("Failed to fetch, fix me later");
            return Err("Failed to get the fucking stupid peace of shit".into());
        }
        println!("{}", json.unwrap());

        let cells: Vec<Cell> = json
            .unwrap()
            .as_array()
            .iter()
            .enumerate()
            .map(|(i, v)| Cell {
                row: i % size,
                col: i / size,
                color: match v {0=>}, number = v
            })
            .collect();

        println!("\nParsed {} cells:", cells.len());
        for cell in &cells {
            println!("  ({}, {}) {:?} {:?}", cell.row, cell.col, cell.color, cell.number);
        }

        Ok(cells)
    }

    pub async fn click(&self, cells: Vec<Cell>) -> Result<bool, Box<dyn std::error::Error>> {
        for cell in &cells {
            let elm = self
                .page
                .find_element(format!("#tile-{}-{}", cell.row, cell.col))
                .await?;
            elm.click().await?;
        }

        // TODO: Wait and check if the ojoo comes around
        Ok(false)
    }

    // pub async fn close(mut self) -> Result<(), Box<dyn std::error::Error>> {
    //     self.browser.close().await?;
    //     self.handle.await?;
    //     Ok(())
    // }
}
