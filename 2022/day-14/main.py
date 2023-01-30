import copy
import sys

sys.path.append('..')
import itertools
from util.vec import Vec2, make_unit

dirs = [Vec2(0, 1), Vec2(-1, 1), Vec2(1, 1)]
def drop_grains(world, max_y, source, has_floor=False):
    num_grains = 0
    cur_pos = source
    while True:
        if has_floor and world[source.x][source.y] == 2:
            # Blocked the source
            break

        possible_paths = [cur_pos + dir for dir in dirs]
        possible_paths = [p for p in possible_paths if world[p.x][p.y] == 0]
        if len(possible_paths) == 0:
            # No place to move, record our position
            world[cur_pos.x][cur_pos.y] = 2
            num_grains += 1
            cur_pos = source  # Drop the next grain
            continue

        cur_pos = possible_paths[0]
        if cur_pos.y >= max_y + 1:
            if not has_floor:
                # into the abyss
                break
            world[cur_pos.x][cur_pos.y] = 2
            num_grains += 1
            cur_pos = source  # Drop the next grain
            continue

    return num_grains


def drop_grains_optimized(world, max_y, source, has_floor=False):
    num_grains = 0
    cur_pos = Vec2(source.x, source.y)
    while True:
        if has_floor and world[source.x][source.y] == 2:
            # Blocked the source
            break

        if world[cur_pos.x][cur_pos.y + 1] == 0:
            cur_pos.y += 1
        elif world[cur_pos.x - 1][cur_pos.y + 1] == 0:
            cur_pos.x -= 1
            cur_pos.y += 1
        elif world[cur_pos.x + 1][cur_pos.y + 1] == 0:
            cur_pos.x += 1
            cur_pos.y += 1
        else:
            # No place to move, sand at rest.
            world[cur_pos.x][cur_pos.y] = 2
            num_grains += 1
            cur_pos.x = source.x  # Drop the next grain
            cur_pos.y = source.y
            continue

        if cur_pos.y >= max_y + 1:
            if not has_floor:
                # into the abyss
                break
            world[cur_pos.x][cur_pos.y] = 2
            num_grains += 1
            cur_pos.x = source.x  # Drop the next grain
            cur_pos.y = source.y
            continue

    return num_grains


def draw_world(world, top_left, bottom_right, has_floor=False):
    for col in range(0, bottom_right.y + 5):
        row_str = ''
        for row in range(top_left.x - 10, bottom_right.x + 10):
            if world[row][col] == 1:
                row_str += '# '
            elif world[row][col] == 2:
                row_str += 'o '
            elif has_floor and col == bottom_right.y + 2:
                row_str += '# '
            else:
                row_str += '. '
        print(row_str)


def get_world(input):
    # Just making a nice big world
    world = []
    for _ in range(1000):
        world.append([0] * 1000)

    top_left = Vec2(10000, 10000)
    bottom_right = Vec2(-1, -1)
    for l in input:
        start_points = l.split(' -> ')
        for start, end in itertools.pairwise(start_points):
            start = Vec2(*(int(s) for s in start.split(',')))
            end = Vec2(*(int(s) for s in end.split(',')))

            dir = make_unit(end - start)
            cursor = start
            while True:
                world[cursor.x][cursor.y] = 1

                if cursor == end:
                    break

                cursor += dir

            top_left.x = min([start.x, end.x, top_left.x])
            top_left.y = min([start.y, end.y, top_left.y])
            bottom_right.x = max([start.x, end.x, bottom_right.x])
            bottom_right.y = max([start.y, end.y, bottom_right.y])

    return world, top_left, bottom_right


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    world1, top_left, bottom_right = get_world(input)
    p1 = drop_grains_optimized(world1, bottom_right.y, Vec2(500, 0))

    world2, top_left, bottom_right = get_world(input)
    p2 = drop_grains_optimized(world2, bottom_right.y, Vec2(500, 0), True)

    return p1, p2


if __name__ == '__main__':
    main()
