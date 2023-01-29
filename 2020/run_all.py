#!/usr/local/bin/python
import subprocess

subprocess.run(["cargo", "build", "--release"])

for x in range(1, 26):
    output = subprocess.run(["cargo", "run", "--release", "--package", f"day-{x}", "--bin", f"day-{x}", "--", f"day-{x}/input"], capture_output=True, text=True)
    if 'No Solution Detected' in output.stdout:
        exit(0)

    print(f'Day {x}...')
    for l in output.stdout.split("\n"):
        print(f"\t{l}")
