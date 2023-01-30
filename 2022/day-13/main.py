import functools
import itertools


def do_compare(left, right):
    if isinstance(left, int) and isinstance(right, int):
        if left < right:
            return 1
        if left > right:
            return -1
        return 0

    if isinstance(left, int):
        return do_compare([left], right)
    elif isinstance(right, int):
        return do_compare(left, [right])
    else:
        zipped = itertools.zip_longest(left, right)
        for inner_left, inner_right in zipped:
            if inner_left is None:
                return 1
            elif inner_right is None:
                return -1
            c = do_compare(inner_left, inner_right)
            if c == 0:
                continue

            return c
        return 0


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    # Eval is scary but easy way to parse input that's already Pythonic!
    pairs = []
    total_input = []
    input_iterator = iter(input)
    while True:
        left = eval(next(input_iterator))
        right = eval(next(input_iterator))
        total_input.extend([left, right])
        pairs.append((left, right))

        # Skip space
        if next(input_iterator, -1) == -1:
            break

    order = []
    for left, right in pairs:
        order.append(do_compare(left, right))

    p1 = sum([i + 1 for i, o in enumerate(order) if o == 1])

    sentinel0 = [[2]]
    sentinel1 = [[6]]
    total_input.extend([sentinel0, sentinel1])

    ordered = sorted(total_input, key=functools.cmp_to_key(do_compare), reverse=True)
    p2 = (ordered.index(sentinel0) + 1) * (ordered.index(sentinel1) + 1)

    return p1, p2


if __name__ == '__main__':
    main()
