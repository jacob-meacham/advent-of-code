import re
from termcolor import colored


def is_assignment_contained(min0, max0, min1, max1):
    if min0 >= min1 and max0 <= max1:
        return 1

    if min1 >= min0 and max1 <= max0:
        return 1

    return 0


def is_overlap(min0, max0, min1, max1):
    if min0 > max1 or min1 > max0:
        return 0

    return 1


def part1(assignments):
    contained = [is_assignment_contained(min0, max0, min1, max1) for ((min0, max0), (min1, max1)) in assignments]
    return sum(contained)


def part2(assignments):
    overlaps = [is_overlap(min0, max0, min1, max1) for ((min0, max0), (min1, max1)) in assignments]
    return sum(overlaps)


def main():
    with open('input.txt', 'r') as f:
        matches = [re.match(r'(\d+)-(\d+),(\d+)-(\d+)', l.strip()) for l in f.readlines()]
        assignments = [((int(m.group(1)), int(m.group(2))), (int(m.group(3)), int(m.group(4)))) for m in matches]

    p1 = part1(assignments)
    p2 = part2(assignments)
    return p1, p2


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2: ', 'white') + colored(str(p2), 'green', attrs=['bold']))
