import re


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    sensors = []
    for line in input:
        m = re.match(r'Sensor at x=(-?\d*), y=(-?\d*): closest beacon is at x=(-?\d*), y=(-?\d*)', line)
        sensors.append(((m.group(1), m.group(2)), (m.group(3), m.group(4))))

    p1 = 0
    p2 = 0

    return p1, p2


if __name__ == '__main__':
    main()