import copy
import itertools
import re
from functools import reduce


def crate_generator(line):
    for i in range(0, 9):
        crate = line[i * 4:4 + i * 4]
        if len(crate) == 0:
            break
        yield i, crate[1]


def perform_moves(buckets, moves, nine_zero_zero_zero_mode):
    for (num, fro, to) in moves:
        moved_items = buckets[fro][-num:]
        del buckets[fro][-num:]
        if nine_zero_zero_zero_mode:
            moved_items = reversed(moved_items)
        buckets[to].extend(moved_items)

    return reduce(lambda acc, xs: acc + xs[-1], buckets, '')


def main():
    buckets = [[] for _ in range(9)]
    moves = []
    with open('input.txt', 'r') as f:
        i = iter(f.readlines())
        bucket_lines = list(itertools.takewhile(lambda line: line[0] == '[', i))
        next(i) # skip the blank line
        for line in i:
            m = re.match(r'move (\d+) from (\d+) to (\d)', line)
            moves.append((int(m.group(1)), int(m.group(2)) - 1, int(m.group(3)) - 1)) # make it 0-indexed

        # reverse since we're pushing on as a stack
        for line in reversed(bucket_lines):
            for bucket, crate in [(b, c) for b, c in crate_generator(line) if c != ' ']:
                buckets[bucket].append(crate)

    p1 = perform_moves(copy.deepcopy(buckets), moves, True)
    p2 = perform_moves(copy.deepcopy(buckets), moves, False)
    return p1, p2


if __name__ == '__main__':
    main()