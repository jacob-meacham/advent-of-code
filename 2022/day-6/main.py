from termcolor import colored

def find_marker(code, length):
    # sliding window against code
    window = []
    for i, c in enumerate(code):
        window.append(c)
        if len(window) > length:
            window = window[1:]

        if len(window) == length and len(set(window)) == length:
            # Found all distinct characters
            return i + 1  # not 0th indexed

    return -1


def main():
    with open('input.txt', 'r') as f:
        code = f.readline()

    return find_marker(code, 4), find_marker(code, 14)


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2: ', 'white') + colored(str(p2), 'green', attrs=['bold']))
