import math


class Vec2(object):
    def __init__(self, x, y):
        self.x = x
        self.y = y

    def __add__(self, other):
        return Vec2(self.x + other.x, self.y + other.y)

    def __sub__(self, other):
        return Vec2(self.x - other.x, self.y - other.y)

    def __repr__(self):
        return f'({self.x}, {self.y})'

    # Not Euclidean distance
    def distance(self, other):
        return abs(self.x - other.x) + abs(self.y - other.y)


# Misnamed, this takes a vector and returns a vector in the same direction with different magnitude (not guaranteed to be length 1)
def make_unit(vec):
    x_factor = 1 if not vec.x else abs(vec.x)
    y_factor = 1 if not vec.y else abs(vec.y)
    return Vec2(int(vec.x / x_factor), int(vec.y / y_factor))


def step_knot_pair(lead, follow):
    move_vec = lead - follow
    if abs(move_vec.x) <= 1 and abs(move_vec.y) <= 1:
        return follow

    return follow + make_unit(move_vec)


def step_rope(dir, rope):
    rope[0] += dir
    for i in range(len(rope)-1):
        rope[i+1] = step_knot_pair(rope[i], rope[i+1])
    print(rope)


def p2(input):
    visited = {(0, 0), }
    rope = [Vec2(0, 0) for _ in range(10)]
    for l in input:
        dir_name = l[0]
        magnitude = int(l[2:])

        match dir_name:
            case 'U':
                dir = Vec2(0, 1)
            case 'D':
                dir = Vec2(0, -1)
            case 'L':
                dir = Vec2(-1, 0)
            case 'R':
                dir = Vec2(1, 0)

        for _ in range(magnitude):
            step_rope(dir, rope)
            visited.add((rope[-1].x, rope[-1].y))

    print(len(visited))

def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    visited = {(0, 0), }
    head_position = Vec2(0, 0)
    tail_position = Vec2(0, 0)
    for l in input:
        dir_name = l[0]
        magnitude = int(l[2:])

        match dir_name:
            case 'U':
                dir = Vec2(0, 1)
            case 'D':
                dir = Vec2(0, -1)
            case 'L':
                dir = Vec2(-1, 0)
            case 'R':
                dir = Vec2(1, 0)

        for _ in range(magnitude):
            head_position += dir

            move_vec = head_position - tail_position
            if abs(move_vec.x) <= 1 and abs(move_vec.y) <= 1:
                continue

            tail_position += make_unit(move_vec)

            visited.add((tail_position.x, tail_position.y))

    print(len(visited))
    p2(input)
    return 0, 0


if __name__ == '__main__':
    main()
