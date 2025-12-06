#!/usr/local/bin/python

import subprocess
import argparse
import platform
from pathlib import Path


def detect_default_rid():
    """Detect the default RID based on the current platform."""
    system = platform.system()
    machine = platform.machine()
    
    if system == "Linux":
        if machine in ("x86_64", "AMD64"):
            return "linux-x64"
        elif machine in ("arm64", "aarch64"):
            return "linux-arm64"
        else:
            return "linux-x64"  # fallback
    elif system == "Darwin":
        if machine == "x86_64":
            return "osx-x64"
        elif machine == "arm64":
            return "osx-arm64"
        else:
            return "osx-x64"  # fallback
    elif system == "Windows":
        return "win-x64"
    else:
        return "linux-x64"  # default fallback


def get_executable_name(day_num, rid):
    """Get the executable name based on platform."""
    if rid.startswith("win"):
        return f"Day{day_num}.exe"
    else:
        return f"Day{day_num}"


def main(table_only, target_rid):
    total_timing = 0.0
    if table_only:
        print('| Day    | Timing (ms) | Good? |')
        print('|--------|-------------|-------|')
    for x in range(1, 26):
        directory = Path(f'Day{x}/bin/Release/net8.0/{target_rid}/')
        if not directory.exists():
            continue

        executable = get_executable_name(x, target_rid)
        executable_path = directory / executable
        
        if not executable_path.exists():
            continue

        # Use the executable path directly, handling Windows vs Unix
        if target_rid.startswith("win"):
            # On Windows, use the .exe directly
            cmd = [str(executable_path)]
        else:
            # On Unix, use ./executable
            cmd = [f'./{executable}']
        
        output = subprocess.run(cmd, cwd=directory, text=True, capture_output=True)
        lines = output.stdout.split("\n")
        if lines[0] == 'Part 1: 0, Part 2: 0':
            continue

        if table_only:
            row = lines[-2]
            print(row)
            # Parse timing value from the table row: | Day X  | Y.Z         |✅/❌     |
            parts = row.split('|')
            if len(parts) >= 3:
                timing_str = parts[2].strip()
                try:
                    timing_value = float(timing_str)
                    total_timing += timing_value
                except ValueError:
                    pass  # Skip if we can't parse the timing
        else:
            for l in lines:
                print(l)
    
    if table_only:
        print('|--------|-------------|-------|')
        print(f'| Total  | {total_timing:.1f}        |❌     |')


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description="Run all days")
    parser.add_argument('--table-only', action='store_true',
                        help='If set, only outputs the table of runtime results')
    parser.add_argument('--target', type=str, default=None,
                        help='Target RID (e.g., linux-x64, osx-x64, win-x64). Defaults to auto-detect.')

    # Parse arguments
    args = parser.parse_args()
    
    # Determine target RID
    target_rid = args.target if args.target else detect_default_rid()
    if not args.target:
        print(f"Auto-detected RID: {target_rid}")

    # Call the main function with the arguments
    main(args.table_only, target_rid)
