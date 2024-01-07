#!/usr/local/bin/python

import subprocess
import argparse
from pathlib import Path


def main():
    print('| Day    | Timing (ms) | Good? |')
    print('|--------|-------------|-------|')

    for x in range(1, 26):
        directory = Path(f'day{x}/')
        if not directory.exists():
            exit(0)

        output = subprocess.run(['/opt/homebrew/bin/go', 'run', f'day{x}/day{x}.go', '-bench'], text=True, capture_output=True)
        lines = output.stdout.split("\n")
        print(lines[-2])


if __name__ == "__main__":
    main()
