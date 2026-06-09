from enum import Enum


class Kind(Enum):
    Empty = "empty"
    Dot = "dot"
    Wall = "wall"
    Value = "value"

    def __str__(self):
        return self.name

    def __repr__(self):
        return self.name
