from termcolor import colored

# There is probably some kind of clever swizzle I could do here
win_loss_dict = {
    'A': {'X': 3, 'Y': 6, 'Z': 0},
    'B': {'Y': 3, 'Z': 6, 'X': 0},
    'C': {'Z': 3, 'X': 6, 'Y': 0}
}

score_per_choice = {
    'X': 1,
    'Y': 2,
    'Z': 3
}

choice_per_round = {
    'X': {'A': 'Z', 'B': 'X', 'C': 'Y'},
    'Y': {'A': 'X', 'B': 'Y', 'C': 'Z'},
    'Z': {'A': 'Y', 'B': 'Z', 'C': 'X'}
}


def win_loss_score(mine, theirs):
    return win_loss_dict[theirs][mine]


def main():
    p1_strategy = 0
    p2_strategy = 0
    with open('input.txt', 'r') as f:
        rounds = [l.strip().split(' ') for l in f.readlines()]
        for (theirs, mine) in rounds:
            p1_strategy = p1_strategy + win_loss_score(mine, theirs) + score_per_choice[mine]

            p2_choice = choice_per_round[mine][theirs]
            p2_strategy = p2_strategy + win_loss_score(p2_choice, theirs) + score_per_choice[p2_choice]

    return p1_strategy, p2_strategy


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2: ', 'white') + colored(str(p2), 'green', attrs=['bold']))
