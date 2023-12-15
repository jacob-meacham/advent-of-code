#!/usr/local/bin/python

import subprocess
import argparse
from pathlib import Path


def main(table_only):
    if table_only:
        print('| Day    | Timing (ms) | Good? |')
        print('|--------|-------------|-------|')
    for x in range(1, 26):
        directory = Path(f'Day{x}/bin/Release/net8.0/')
        if not directory.exists():
            exit(0)

        output = subprocess.run(f'./Day{x}', cwd=directory, text=True, capture_output=True)
        lines = output.stdout.split("\n")
        if lines[0] == 'Part 1: 0, Part 2: 0':
            exit(0)

        if table_only:
            print(lines[-2])
        else:
            for l in lines:
                print(l)


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Run all days")
    parser.add_argument('--table-only', action='store_true',
                        help='If set, only outputs the table of runtime results')

    # Parse arguments
    args = parser.parse_args()

    # Call the main function with the table-only argument
    main(args.table_only)
