import sys
sys.path.append('..')

from util.vec import Vec2, make_unit
from termcolor import colored

def step_knot_pair_optimized(lead, follow):
    move_vec = (lead.x - follow.x, lead.y - follow.y)

    x = (move_vec[0] >> 127) | (not not move_vec[0])
    y = (move_vec[1] >> 127) | (not not move_vec[1])

    return Vec2(follow.x + x, follow.y + y)


def step_knot_pair(lead, follow):
    move_vec = lead - follow
    if abs(move_vec.x) <= 1 and abs(move_vec.y) <= 1:
        return follow

    return follow + make_unit(move_vec)


def step_rope(dir, rope):
    rope[0] += dir
    for i in range(len(rope) - 1):
        rope[i + 1] = step_knot_pair_optimized(rope[i], rope[i + 1])


def move_rope(input, rope_length):
    visited = {(0, 0), }
    rope = [Vec2(0, 0) for _ in range(rope_length)]
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

    return len(visited)


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    p1 = move_rope(input, 2)
    p2 = move_rope(input, 10)
    return p1, p2


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2: ', 'white') + colored(str(p2), 'green', attrs=['bold']))
