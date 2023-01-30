import argparse
import os
from pathlib import Path
import time


def create_all():
    for i in range(1, 26):
        root = Path(f'day-{i}')
        root.mkdir(parents=True, exist_ok=True)
        (root / 'input.txt').touch(exist_ok=True)
        (root / 'main.py').touch(exist_ok=True)
    pass


def benchmark(fn, iterations):
    start = time.perf_counter()
    for _ in range(0, iterations):
        answer = fn()
    end = time.perf_counter()
    return 1000 * ((end - start) / iterations), answer


def load_module(name):
    root = __import__(name)
    module = getattr(root, 'main')
    return getattr(module, 'main')

# could of course just time with the command line, but some niceties with the harness
def test_harness(show_timings, show_answers, timing_iterations, start_from):
    for i in range(start_from, 26):
        print(f'Day {i}')
        try:
            fn = load_module(f'day-{i}.main')
            cur_dir = os.getcwd()
            os.chdir(f'day-{i}')
            avg_millis, answer = benchmark(fn, timing_iterations)
            os.chdir(cur_dir)
        except Exception:
            print('  ❌ No Solution Detected')
            break

        if show_answers:
            print(
                f'  {"✅" if answer[0] != 0 else "❌"} Part 1: {answer[0]}, {"✅" if answer[1] != 0 else "❌"} Part 2: {answer[1]}')

        if show_timings:
            print(f'  {"✅" if avg_millis < 500 else "❌"} average ms: {avg_millis:.2f}')


def parse_args():
    parser = argparse.ArgumentParser(
        prog='Advent of Code Harness')
    parser.add_argument('-c', '--create', action='store_true')
    parser.add_argument('--iterations', default=10)
    parser.add_argument('--timings', action=argparse.BooleanOptionalAction, default=True)
    parser.add_argument('--answers', action=argparse.BooleanOptionalAction, default=True)
    parser.add_argument('--start-from', type=int, default=1)
    return parser.parse_args()


if __name__ == '__main__':
    args = parse_args()

    if args.create:
        create_all()

    test_harness(args.timings, args.answers, args.iterations, args.start_from)
