import subprocess

subprocess.run(["cargo", "build", "--release"])

for x in range(1, 26):
    print(f'Running Day {x}...')
    output = subprocess.run(["cargo", "run", "--release", "--package", f"day-{x}", "--bin", f"day-{x}", "--", f"day-{x}/input"], capture_output=True, text=True)
    for l in output.stdout.split("\n"):
        print(f"\t{l}")
