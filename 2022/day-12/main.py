import queue
import sys
from collections import defaultdict
from termcolor import colored


def get_path(path_map, cur_node):
    path = [cur_node]
    while cur_node in path_map:
        cur_node = path_map[cur_node]
        path.append(cur_node)
    return path

# Djikstra/ A*
def find_path(starts, goal, grid, stop_at_goal = False, h = None):
    if not h:
        h = lambda x: 0

    visited = set()
    pq = queue.PriorityQueue()

    path_map = {}
    gscore = defaultdict(lambda: sys.maxsize)

    for s in starts:
        visited.add(s)
        pq.put((0, s))
        gscore[s] = 0

    while not pq.empty():
        _, cur_node = pq.get()
        cur_x, cur_y = cur_node

        if stop_at_goal and cur_node == goal:
            break

        neighbors = [(cur_x + 1, cur_y), (cur_x - 1, cur_y), (cur_x, cur_y + 1), (cur_x, cur_y - 1)]
        neighbors = [n for n in neighbors if 0 <= n[0] < len(grid) and 0 <= n[1] < len(grid[0])]
        for neighbor in neighbors:
            nx, ny = neighbor
            height = ord(grid[nx][ny]) - ord(grid[cur_x][cur_y])
            if height > 1:
                # impassible
                continue

            gs = gscore[cur_node] + 1
            if gs < gscore[neighbor]:
                path_map[neighbor] = cur_node
                gscore[neighbor] = gs
                fscore = gs + h(neighbor)
                if neighbor not in visited:
                    visited.add(neighbor)
                    pq.put((fscore, neighbor))

    return get_path(path_map, goal)


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    grid = []
    all_starts = []
    for ir, r in enumerate(input):
        row = []
        for ic, c in enumerate(r):
            if c == 'S':
                start = (ir, ic)
                c = 'a'

            elif c == 'E':
                goal = (ir, ic)
                c = 'z'

            if c == 'a':
                all_starts.append((ir, ic))
            row.append(c)
        grid.append(row)

    def h(node):
        height_cost = ord(grid[goal[0]][goal[1]]) - ord(grid[node[0]][node[1]])
        return abs(goal[0] - node[0]) + abs(goal[1] - node[1]) - height_cost

    p1 = find_path([start], goal, grid, True, h)
    p2 = find_path(all_starts, goal, grid)

    return len(p1)-1, len(p2)-1 # Steps, not nodes so we subtract 1


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2: ', 'white') + colored(str(p2), 'green', attrs=['bold']))
