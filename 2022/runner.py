import argparse
import os
from pathlib import Path
import time
from termcolor import colored

def get_bests():
    bests = []
    try:
        with open('.bests', 'r') as f:
            for line in f.readlines():
                line = line.strip()
                if line:
                    bests.append(float(line))
    except FileNotFoundError:
        pass

    return bests + [10000.0] * (25 - len(bests))


def bold(text: str) -> str:
    return '\033[1m' + text + '\033[0m'


def pad(s, total_length):
    return s + ' ' * (total_length - len(s))

def create_all():
    for i in range(1, 26):
        root = Path(f'day-{i}')
        root.mkdir(parents=True, exist_ok=True)
        (root / 'input.txt').touch(exist_ok=True)
        (root / 'main.py').touch(exist_ok=True)
    pass


def benchmark(fn, iterations, warmup_cycles=3):
    # Warm-up cycles to ensure code is optimized and caches are warm
    for _ in range(warmup_cycles):
        fn()
    
    # Actual timing measurements
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
def test_harness(show_timings, show_answers, timing_iterations, start_from, skip, day=None):
    skip = skip or []
    bests = get_bests()
    total_time = 0.0
    num_complete = 0
    
    if show_timings:
        print(f'| {bold("Day")}     | {bold("Timing (ms)")} | {bold("Good?")} |')
        print('|---------|-------------|-------|')
    
    max_day = 0
    # If day is specified, only run that single day
    end_day = day + 1 if day else 26
    start_day = day if day else start_from
    for i in range(start_day, end_day):
        if i in skip:
            continue
        
        try:
            fn = load_module(f'day-{i}.main')
            cur_dir = os.getcwd()
            os.chdir(f'day-{i}')
            avg_millis, answer = benchmark(fn, timing_iterations)
            os.chdir(cur_dir)
            
            # Track best time
            if avg_millis < bests[i-1]:
                bests[i-1] = avg_millis
            else:
                avg_millis = bests[i-1]
            
            good = "✅" if avg_millis < 500 else "❌"
            total_time += avg_millis
            num_complete += 1
            max_day = max(max_day, i)
        except Exception:
            if show_timings:
                print(f'| {pad("Day " + str(i), 8)}| {"No Solve":12}| ❌     |')
            if show_answers:
                print(f'Day {i}: ❌ No Solution Detected')
            continue

        if show_answers:
            status1 = "✅" if answer[0] != 0 else "❌"
            status2 = "✅" if answer[1] != 0 else "❌"
            print(colored(f'Day {i}: {status1} ', 'white') + 
                  colored('Part 1: ', 'white') + colored(str(answer[0]), 'green', attrs=['bold']) + 
                  colored(f' {status2} ', 'white') + 
                  colored('Part 2: ', 'white') + colored(str(answer[1]), 'green', attrs=['bold']))

        if show_timings:
            elapsed_str = f'{avg_millis:7.2f}'
            print(f'| {pad("Day " + str(i), 8)}| {pad(elapsed_str, 12)}|{good}     |')
    
    if show_timings and num_complete > 0:
        avg_time = total_time / num_complete
        total_good = "✅" if avg_time < 100 else "❌"
        elapsed_str = f'{total_time:7.2f}'
        print(f'| {bold("Total")}   | {pad(elapsed_str, 12)}|{total_good}     |')
    
    # Write back bests for all days up to max_day
    if max_day > 0:
        with open('.bests', 'w') as f:
            for x in range(max_day):
                f.write(f'{str(bests[x])}\n')


def parse_args():
    parser = argparse.ArgumentParser(
        prog='Advent of Code Harness')
    parser.add_argument('-c', '--create', action='store_true')
    parser.add_argument('--iterations', default=10)
    parser.add_argument('--timings', action=argparse.BooleanOptionalAction, default=True)
    parser.add_argument('--answers', action=argparse.BooleanOptionalAction, default=True)
    parser.add_argument('--start-from', type=int, default=1)
    parser.add_argument('--skip', nargs='+', type=int, help='Days to skip')
    parser.add_argument('--day', type=int, help='Run only a single day (1-25)')
    return parser.parse_args()


if __name__ == '__main__':
    args = parse_args()

    if args.create:
        create_all()

    test_harness(args.timings, args.answers, int(args.iterations), args.start_from, args.skip, args.day)
