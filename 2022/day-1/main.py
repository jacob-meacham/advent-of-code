from functools import reduce


def part1(elf_inventories):
    def fn(acc, xs):
        s = sum(xs)
        return max(s, acc)
    return reduce(fn, elf_inventories, 0)


# Obviously could solve p1 with this too
def part2(elf_inventories):
    sums = sorted([sum(e) for e in elf_inventories], reverse=True)
    return sum(sums[0:3])


def main():
    elf_inventories = [[]]
    with open('input.txt', 'r') as f:
        for l in f.readlines():
            l = l.strip()
            if not l:
                elf_inventories.append([])
            else:
                elf_inventories[-1].append(int(l))

    p1 = part1(elf_inventories)
    p2 = part2(elf_inventories)
    return p1, p2


if __name__ == '__main__':
    main()
