class Vec2(object):
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def __add__(self, other):
        return Vec2(self.x + other.x, self.y + other.y)

    def __sub__(self, other):
        return Vec2(self.x - other.x, self.y - other.y)

    def __eq__(self, other):
        return self.x == other.x and self.y == other.y

    def __repr__(self):
        return f'({self.x}, {self.y})'

# Misnamed, this takes a vector and returns a vector in the same direction with different magnitude
# (not guaranteed to be length 1)
def make_unit(vec):
    x_factor = 1 if not vec.x else abs(vec.x)
    y_factor = 1 if not vec.y else abs(vec.y)
    return Vec2(int(vec.x / x_factor), int(vec.y / y_factor))