from time import sleep

from selenium import webdriver
from selenium.webdriver.common.by import By

from my_types import Kind


def main(url: str, size: int):
    driver = webdriver.Firefox()
    driver.get(url)
    assert "0h n0" in driver.title

    driver.execute_script(f"Game.startGame({size})")
    sleep(1)

    tile_state = driver.execute_script("return Game.grid.getValues();")
    tile_state = parse_tile_state(tile_state, size)

    input("Stalling...")
    driver.close()


def parse_tile_state(orig: list[int], size: int):
    output = []
    for i, v in enumerate(orig):
        match v:
            case 0:
                kind = Kind.Empty
            case 1:
                kind = Kind.Wall
            case 2:
                kind = Kind.Dot
            case _:
                kind = Kind.Value
                v -= 2

        output.append({"row": i % size, "col": i // size, "kind": kind, "value": v if v > 0 else None})
    return output


if __name__ == "__main__":
    main("https://0hn0.com", 9)
