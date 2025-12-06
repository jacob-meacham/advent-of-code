#!/usr/local/bin/python3
import re
import subprocess
import argparse
import time
from pathlib import Path

def get_bests():
    with open('.bests', 'a+') as f:
        bests = [float(b) for b in f.readlines()]

    return bests + [10000.0] * (25 - len(bests))


def bold(text: str) -> str:
    return '\033[1m' + text + '\033[0m'


def pad(s, total_length):
    return s + ' ' * (total_length - len(s))

def run_benchmark(executable: Path) -> float:
    # Warmup loop
    for _ in range(3):
        subprocess.run([executable], text=True, capture_output=True)
    
    # Benchmark iterations
    times = []
    for _ in range(10):
        start = time.perf_counter()
        subprocess.run([executable], text=True, capture_output=True)
        end = time.perf_counter()
        elapsed_ms = (end - start) * 1000.0  # Convert to milliseconds
        times.append(elapsed_ms)
    
    # Return average time
    return sum(times) / len(times)

def main(start_from: int) -> None:
    print(f'| {bold("Day")}    | {bold("Timing (ms)")} | {bold("Good?")} |')
    print('|--------|-------------|-------|')

    bests = get_bests()
    total_time = 0.0
    num_complete = 0
    for x in range(start_from, 26):
        directory = Path(f'day-{x}/')
        if not directory.exists():
            continue

        executable = directory / 'run.sh'
        if not executable.exists():
            continue

        # TODO: Run this as a benchmark
        elapsed = run_benchmark(executable)
        if elapsed < bests[x-1]:
            bests[x-1] = elapsed
        else:
            elapsed = bests[x-1]

        total_time += elapsed
        num_complete += 1

        if (elapsed < 100):
            good = "✅"
        else:
            good = "❌"

        elapsed_str = f'{elapsed:7.5f}'
        print(f'| Day {x}  | {pad(elapsed_str, 12)}|{good}     |')

    if total_time / num_complete < 100:
        good = "✅"
    else:
        good = "❌"

    elapsed_str = f'{total_time:7.5f}'
    print(f'| {bold("Total")}  | {pad(elapsed_str, 12)}|{good}     |')

    with open('.bests', 'w') as f:
        for x in range(num_complete):
            f.write(f'{str(bests[x])}\n')

def parse_args():
    parser = argparse.ArgumentParser(
        prog='Advent of Code Harness')
    parser.add_argument('--start-from', type=int, default=1)
    return parser.parse_args()


if __name__ == '__main__':
    args = parse_args()

    main(args.start_from)
