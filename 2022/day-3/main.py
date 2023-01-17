from functools import reduce


def letter_to_score(letter):
    n = ord(letter)
    return n - 96 if n >= ord('a') else n - 65 + 27


def part1(rucksacks):
    inventory = [({*r[0:int(len(r) / 2)]}, {*r[int(len(r) / 2):]}) for r in rucksacks]
    overlaps = [list(a.intersection(b)) for a, b in inventory]

    def fn(acc, xs):
        assert (len(xs) == 1)
        return acc + letter_to_score(xs[0])

    return reduce(fn, overlaps, 0)


def part2(rucksacks):
    groups = list(zip(*[iter(rucksacks)] * 3))
    badges = [list(set(a).intersection(b).intersection(c)) for a, b, c in groups]

    def fn(acc, xs):
        assert (len(xs) == 1)
        return acc + letter_to_score(xs[0])

    return reduce(fn, badges, 0)


def main():
    # TODO: Need to fix the folder issue
    with open('day-3/input.txt', 'r') as f:
        rucksacks = [l.strip() for l in f.readlines()]

    p1 = part1(rucksacks)
    p2 = part2(rucksacks)
    return p1, p2


if __name__ == '__main__':
    main()
