#!/usr/local/bin/python3
import re
import subprocess
import argparse
from pathlib import Path

def get_bests():
    with open('.bests', 'a+') as f:
        bests = [float(b) for b in f.readlines()]

    return bests + [10000.0] * (25 - len(bests))


def bold(text: str) -> str:
    return '\033[1m' + text + '\033[0m'


def pad(s, total_length):
    return s + ' ' * (total_length - len(s))

def main(start_from: int) -> None:
    subprocess.run(["cargo", "build", "--release"])

    print(f'| {bold("Day")}     | {bold("Timing (ms)")} | {bold("Good?")} |')
    print('|---------|-------------|-------|')

    bests = get_bests()
    total_time = 0.0
    num_complete = 0
    
    for x in range(start_from, 26):
        executable = Path('target') / 'release' / f'day-{x}'
        if not executable.exists():
            continue

        output = subprocess.run([str(executable), f'day-{x}/input'], text=True, capture_output=True)
        lines = output.stdout.split("\n")

        # Parse "✅ Average Milliseconds: 0.238336" or "❌ Average Milliseconds: 0.238336" from output
        elapsed = None
        good = None
        for line in lines:
            m = re.search(r'([✅❌])\s*Average Milliseconds:\s*([\d.]+)', line)
            if m:
                good = m.group(1)
                elapsed = float(m.group(2))
                break

        if elapsed is None:
            continue

        if elapsed < bests[x-1]:
            bests[x-1] = elapsed
        else:
            elapsed = bests[x-1]

        total_time += elapsed
        num_complete += 1

        elapsed_str = f'{elapsed:7.5f}'
        print(f'| {pad('Day ' + str(x), 8)}| {pad(elapsed_str, 12)}|{good}     |')

    if total_time / num_complete < 100:
        good = "✅"
    else:
        good = "❌"

    elapsed_str = f'{total_time:7.5f}'
    print(f'| {bold("Total")}   | {pad(elapsed_str, 12)}|{good}     |')

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
