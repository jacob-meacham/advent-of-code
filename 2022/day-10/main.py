from termcolor import colored

def get_signal_strength(input):
    register_history = [1]
    for l in input:
        cur_value = register_history[-1]
        match l[0:4]:
            case 'noop':
                register_history.append(cur_value)
            case 'addx':
                register_history.extend([cur_value, cur_value + int(l[4:])])

    signal_strength = sum([register_history[x - 1] * x for x in [20, 60, 100, 140, 180, 220]])
    return signal_strength


def emulator(input):
    register = 1
    # Either None if the instruction finished on the last cycle or
    # tuple of (instruction, start_cycle) where start_cycle is the cycle it started on
    cur_instruction = None
    cycle = 0
    crt = []
    instruction_iter = iter(input)

    while True:
        if not cur_instruction:
            next_instruction = next(instruction_iter, None)
            if not next_instruction:
                break

            cur_instruction = (next_instruction, cycle)

        # Draw screen
        if abs(register - (cycle % 40)) <= 1:
            crt.append('#')
        else:
            crt.append('.')

        # Tick
        cycle += 1

        # Falling edge, execute instruction
        label, cycle_start = cur_instruction
        match label[0:4]:
            case 'noop':
                # Done with instruction
                cur_instruction = None
            case 'addx':
                if cycle - cycle_start >= 2:
                    register += int(label[4:])
                    cur_instruction = None

    rows = [' '.join(crt[40 * i:39 + 40 * i]) for i in range(6)]
    s = '\n'.join(rows)
    return s


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    p1 = get_signal_strength(input)
    p2 = emulator(input)
    return p1, '\n' + p2


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2:', 'white') + colored(str(p2), 'green', attrs=['bold']))
