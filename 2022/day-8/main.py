import operator
from functools import reduce


def calc_scenic_distance_from_dir(grid, start, dir):
    (row, col) = start
    pos_height = grid[row][col]
    viewing_distance = 0

    while True:
        row = row + dir[0]
        col = col + dir[1]

        if row < 0 or col < 0 or row >= len(grid) or col >= len(grid[row]):
            break

        viewing_distance += 1
        if grid[row][col] >= pos_height:
            break

    return viewing_distance


def calc_scenic_distance_for_position(row, col, grid):
    dirs = [(0, 1), (0, -1), (1, 0), (-1, 0)]
    distances = [calc_scenic_distance_from_dir(grid, (row, col), dir) for dir in dirs]
    return reduce(operator.mul, distances, 1)


def calc_visibility_from_dir(grid, vis_set, start, dir):
    (row, col) = start
    highest_in_dir = -1

    while True:
        row = row + dir[0]
        col = col + dir[1]

        if row < 0 or col < 0 or row >= len(grid) or col >= len(grid[row]):
            break

        if grid[row][col] > highest_in_dir:
            highest_in_dir = grid[row][col]
            vis_set.add((row, col))


def calc_visibility(grid):
    vis_set = set()

    # Left/right
    for row in range(0, len(grid)):
        calc_visibility_from_dir(grid, vis_set, (row, -1), (0, 1))
        calc_visibility_from_dir(grid, vis_set, (row, len(grid)), (0, -1))

    # Top/bottom
    for col in range(0, len(grid[0])):
        calc_visibility_from_dir(grid, vis_set, (-1, col), (1, 0))
        calc_visibility_from_dir(grid, vis_set, (len(grid[0]), col), (-1, 0))

    return len(vis_set)


def calc_scenic_distance(grid):
    highest_scenic_distance = 0
    for row in range(0, len(grid)):
        for col in range(0, len(grid[0])):
            scenic_distance = calc_scenic_distance_for_position(row, col, grid)
            if scenic_distance > highest_scenic_distance:
                highest_scenic_distance = scenic_distance

    return highest_scenic_distance


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    grid = []
    for l in input:
        row = []
        for num in l:
            row.append(int(num))
        grid.append(row)

    p1 = calc_visibility(grid)
    p2 = calc_scenic_distance(grid)
    return p1, p2


if __name__ == '__main__':
    main()
