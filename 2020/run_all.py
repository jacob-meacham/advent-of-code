import subprocess

for x in range(1, 25):
    print(f'Running Day {x}...')
    subprocess.run(["cargo", "run", "--package", f"day-{x}", "--bin", f"day-{x}", "--", f"day-{x}/input"])