from functools import reduce


def calculate_visibilty_from_dir(grid, visibility_grid, start, dir):
    row = start[0]
    col = start[1]
    highest_in_dir = -1

    while True:
        row = row + dir[0]
        col = col + dir[1]

        if row < 0 or col < 0 or row >= len(grid) or col >= len(grid[row]):
            break

        if grid[row][col] >= highest_in_dir:
            highest_in_dir = grid[row][col]

    return highest_in_dir

def calculate_scenic_distance_from_dir(grid, memoization, start, dir):
    row = start[0]
    col = start[1]
    our_height = grid[row][col]
    viewing_distance = 0

    while True:
        row = row + dir[0]
        col = col + dir[1]

        if row < 0 or col < 0 or row >= len(grid) or col >= len(grid[row]):
            break

        viewing_distance += 1
        if grid[row][col] >= our_height:
            break

        # if 'viewing_distance' in memoization[row][col]:
        #     viewing_distance = memoization[row][col]['viewing_distance'] + viewing_distance
        #     break

    memoization[start[0]][start[1]] = viewing_distance
    return viewing_distance

def calculate_visibility_for_position(row, col, grid, visibility_grid):
    highest = []
    highest.append(calculate_visibilty_from_dir(grid, visibility_grid, (row, col), (0, 1)))
    highest.append(calculate_visibilty_from_dir(grid, visibility_grid, (row, col), (0, -1)))
    highest.append(calculate_visibilty_from_dir(grid, visibility_grid, (row, col), (1, 0)))
    highest.append(calculate_visibilty_from_dir(grid, visibility_grid, (row, col), (-1, 0)))

    if grid[row][col] > sorted(highest)[0]:
        visibility_grid[row][col] = True
    else:
        visibility_grid[row][col] = False

def calc_scenic_distance_for_position(row, col, grid):
    memoization = []
    for r in grid:
        memoization.append([{}] * len(r))

    distances = []
    distances.append(calculate_scenic_distance_from_dir(grid, memoization, (row, col), (0, 1)))
    distances.append(calculate_scenic_distance_from_dir(grid, memoization, (row, col), (0, -1)))
    distances.append(calculate_scenic_distance_from_dir(grid, memoization, (row, col), (1, 0)))
    distances.append(calculate_scenic_distance_from_dir(grid, memoization, (row, col), (-1, 0)))

    return reduce(lambda acc, xs: acc * xs, distances, 1)

def calc_visibility(grid):
    visibility_grid = []
    for r in grid:
        visibility_grid.append([None]*len(r))

    # First attempt, cast a ray in all directions for each position in the grid
    for row in range(0, len(grid)):
        for col in range(0, len(grid[0])):
            calculate_visibility_for_position(row, col, grid, visibility_grid)

    num_visible = 0
    for row in range(0, len(visibility_grid)):
        for col in range(0, len(visibility_grid[0])):
            if visibility_grid[row][col] is True:
                num_visible = num_visible + 1

    return num_visible

def calc_scenic_distance(grid):
    highest_scenic_distance = 0
    for row in range(0, len(grid)):
        for col in range(0, len(grid[0])):
            scenic_distance = calc_scenic_distance_for_position(row, col, grid)
            if scenic_distance > highest_scenic_distance:
                highest_scenic_distance = scenic_distance

    return highest_scenic_distance

def main():
    with open('day-8/input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    grid = []
    for l in input:
        row = []
        for num in l:
            row.append(int(num))
        grid.append(row)

    p1 = calc_visibility(grid)
    #p1 = 0
    p2 = calc_scenic_distance(grid)
    return p1, p2


if __name__ == '__main__':
    main()
