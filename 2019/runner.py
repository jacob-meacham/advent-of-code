#!/usr/local/bin/python
import re
import subprocess
import argparse
from pathlib import Path

def get_bests():
    with open('.bests', 'r') as f:
        bests = [float(b) for b in f.readlines()]

    return bests + [10000.0] * (25 - len(bests))


def bold(text: str) -> str:
    return '\033[1m' + text + '\033[0m'


def pad(s, total_length):
    return s + ' ' * (total_length - len(s))

def main():
    print(f'| {bold("Day")}    | {bold("Timing (ms)")} | {bold("Good?")} |')
    print('|--------|-------------|-------|')

    bests = get_bests()
    total_time = 0.0
    num_complete = 0
    for x in range(1, 26):
        directory = Path(f'day{x}/')
        if not directory.exists():
            break

        executable = directory / 'bin' / f'day{x}'
        if not executable.exists():
            subprocess.run(['/opt/homebrew/bin/go', 'build', '-o', f'day{x}/bin/day{x}', f'day{x}/day{x}.go'], text=True,
                           capture_output=True)

        output = subprocess.run([f'day{x}/bin/day{x}', '-bench'], text=True, capture_output=True)
        lines = output.stdout.split("\n")

        m = re.match(r'\|(\s*Day \d+\s*)\|(\s*\S+\s*)\|(.*?)\|', lines[-2])

        elapsed = float(m.group(2).strip())
        if elapsed < bests[x-1]:
            bests[x-1] = elapsed
        else:
            elapsed = bests[x-1]

        total_time += elapsed
        num_complete += 1

        elapsed_str = f'{elapsed:7.5f}'
        print(f'|{m.group(1)}| {pad(elapsed_str, 12)}|{m.group(3)}|')

    if total_time / num_complete < 100:
        good = "✅"
    else:
        good = "❌"

    elapsed_str = f'{total_time:7.5f}'
    print(f'| {bold("Total")}  | {pad(elapsed_str, 12)}|{good}     |')

    with open('.bests', 'w') as f:
        for x in range(num_complete):
            f.write(f'{str(bests[x])}\n')


if __name__ == "__main__":
    main()
