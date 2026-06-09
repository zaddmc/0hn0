mod browser_interactions;
mod solve;
mod types;

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let session = browser_interactions::BrowserSession::new("https://0hn0.com").await?;

    let cells = session.gather().await?;
    let solved = solve::solve(cells);
    let _result = session.click(solved).await?;

    println!("Done with all");
    Ok(())
}
