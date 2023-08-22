import sys

sys.path.append('..')

import math
import re

from util.vec import Vec2


def manhattan_distance(v1, v2):
    return abs(v1.x - v2.x) + abs(v1.y - v2.y)


def debug_print(min_segment, max_segment, covered_positions, active_beacons):
    line = ''
    for x in range(min_segment - 5, max_segment + 5):
        if x in covered_positions:
            line += '#'
        elif x in active_beacons:
            line += 'B'
        else:
            line += '.'

    min_s = str(min_segment)
    graph_line = f'     {min_s}{" " * (len(line) - 11 - len(min_s))}{str(max_segment)}'
    print(graph_line)
    print(line)


def clamp(n, low, high):
    if n < low:
        return low
    if n > high:
        return high
    return n

def get_line_optimized(sensors, y, min_y=-math.inf, max_y=math.inf):
    segments = []

    for s, b, distance_to_beacon in sensors:
        distance_to_y = manhattan_distance(s, Vec2(s.x, y))
        remaining_distance = distance_to_beacon - distance_to_y
        if remaining_distance >= 0:
            low = clamp(s.x - remaining_distance, min_y, max_y)
            high = clamp(s.x + remaining_distance, min_y, max_y)

            segments.append((low, high))

    # Sort the segments
    segments = sorted(segments, key=lambda s: s[0])
    total_covered = 0
    uncovered = -1 # Using the fact that we know there is at most 1 uncovered in any line
    current_segment = segments[0]
    for s in segments[1:]:
        if s[0] <= current_segment[1]:
            # These segments overlap
            current_segment = (current_segment[0], max(s[1], current_segment[1]))
        else:
            print(f'Not overlapped: {current_segment}, {s}')
            # These segments don't overlap, so count up!
            uncovered = current_segment[1]+1
            total_covered += current_segment[1] - current_segment[0]
            current_segment = s


    # Add up the last segment
    total_covered += current_segment[1] - current_segment[0]
    return total_covered, uncovered


def get_line(sensors, y):
    # Figure out which positions are marked by each sensor within the line
    # Could store more densely as segments instead of as a set
    covered_positions = set()

    # Need to remove all positions that have a beacon on them:
    active_beacons = set([b.x for _, b, _ in sensors if b.y == y])

    min_segment = math.inf
    max_segment = -math.inf
    for s, b, distance_to_beacon in sensors:
        # Closest point on y to the sensor is at the same x-coordinate Furthest point possible in both directions is
        # distance to sensor - distance to y, which creates a number of segments
        distance_to_y = manhattan_distance(s, Vec2(s.x, y))
        remaining_distance = distance_to_beacon - distance_to_y
        if remaining_distance >= 0:
            low = s.x - remaining_distance
            high = s.x + remaining_distance
            min_segment = min(min_segment, low)
            max_segment = max(max_segment, high)

            for n in range(low, high + 1):
                if n in active_beacons:
                    continue
                covered_positions.add(n)

    # debug_print(min_segment, max_segment, covered_positions, active_beacons)
    return len(covered_positions)

def find_uncovered(sensors, min_coord, max_coord):
    # Idea 1
    # The missing beacon must be on a sensor boundary; if it were not that would mean that there were multiple
    # uncovered spots. So, we need to check each location along a sensor boundary (constrained by our min, max)
    # This is still 83m coordinates to check - way too many.

    # Idea 2
    # Use our line-by-line approach, which is now pretty quick, to find the missing coordinate.
    answer = -1
    for y in range(min_coord, max_coord+1):
        total_covered, uncovered_x = get_line_optimized(sensors, y, min_coord, max_coord)
        if total_covered != max_coord:
            answer = uncovered_x * 4000000 + y
            break

    return answer


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    sensors = []
    for line in input:
        m = re.match(r'Sensor at x=(-?\d*), y=(-?\d*): closest beacon is at x=(-?\d*), y=(-?\d*)', line)
        sensor = Vec2(int(m.group(1)), int(m.group(2)))
        nearest_beacon = Vec2(int(m.group(3)), int(m.group(4)))
        distance_to_beacon = manhattan_distance(sensor, nearest_beacon)

        sensors.append((sensor, nearest_beacon, distance_to_beacon))

    # 5607466
    p1, _ = get_line_optimized(sensors, 2000000)
    p2 = find_uncovered(sensors, 0, 4000000)

    return p1, p2


if __name__ == '__main__':
    main()
