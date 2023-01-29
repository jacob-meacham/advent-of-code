class Monkey(object):
    def __init__(self, inventory, operation_fn, test_fn, worry_mod_fn, partners=None):
        if partners is None:
            partners = {}
        self.num_inspected = 0
        self.inventory = inventory
        self.operation_fn = operation_fn
        self.test_fn = test_fn
        self.partners = partners
        self.worry_mod_fn = worry_mod_fn

    def give_item(self, item_with_worry_level):
        self.inventory.append(item_with_worry_level)

    def take_turn(self):
        for item in self.inventory:
            self.num_inspected += 1
            worry_level = self.operation_fn(item)
            worry_level = self.worry_mod_fn(worry_level)
            outcome = self.test_fn(worry_level)
            new_monkey = self.partners[outcome]
            new_monkey.give_item(worry_level)

        # Clear out the inventory
        self.inventory = []


# There are only 8 monkeys so just hardcoding them
def do_rounds(num_rounds, worry_mod_fn):
    monkeys = [
        Monkey([98, 70, 75, 80, 84, 89, 55, 98], lambda x: x * 2, lambda x: x % 11 == 0, worry_mod_fn),
        Monkey([59], lambda x: x * x, lambda x: x % 19 == 0, worry_mod_fn),
        Monkey([77, 95, 54, 65, 89], lambda x: x + 6, lambda x: x % 7 == 0, worry_mod_fn),
        Monkey([71, 64, 75], lambda x: x + 2, lambda x: x % 17 == 0, worry_mod_fn),
        Monkey([74, 55, 87, 98], lambda x: x * 11, lambda x: x % 3 == 0, worry_mod_fn),
        Monkey([90, 98, 85, 52, 91, 60], lambda x: x + 7, lambda x: x % 5 == 0, worry_mod_fn),
        Monkey([99, 51], lambda x: x + 1, lambda x: x % 13 == 0, worry_mod_fn),
        Monkey([98, 94, 59, 76, 51, 65, 75], lambda x: x + 5, lambda x: x % 2 == 0, worry_mod_fn),
    ]

    monkeys[0].partners = {True: monkeys[1], False: monkeys[4]}
    monkeys[1].partners = {True: monkeys[7], False: monkeys[3]}
    monkeys[2].partners = {True: monkeys[0], False: monkeys[5]}
    monkeys[3].partners = {True: monkeys[6], False: monkeys[2]}
    monkeys[4].partners = {True: monkeys[1], False: monkeys[7]}
    monkeys[5].partners = {True: monkeys[0], False: monkeys[4]}
    monkeys[6].partners = {True: monkeys[5], False: monkeys[2]}
    monkeys[7].partners = {True: monkeys[3], False: monkeys[6]}

    # monkeys = [
    #     Monkey([79, 98], lambda x: x * 19, lambda x: x % 23 == 0, worry_mod_fn),
    #     Monkey([54, 65, 75, 74], lambda x: x + 6, lambda x: x % 19 == 0, worry_mod_fn),
    #     Monkey([79, 60, 97], lambda x: x * x, lambda x: x % 13 == 0, worry_mod_fn),
    #     Monkey([74], lambda x: x + 3, lambda x: x % 17 == 0, worry_mod_fn)
    # ]
    #
    # monkeys[0].partners = { True: monkeys[2], False: monkeys[3] }
    # monkeys[1].partners = { True: monkeys[2], False: monkeys[0] }
    # monkeys[2].partners = { True: monkeys[1], False: monkeys[3] }
    # monkeys[3].partners = { True: monkeys[0], False: monkeys[1] }

    for _ in range(num_rounds):
        for monkey in monkeys:
            monkey.take_turn()

    num_inspected = sorted([monkey.num_inspected for monkey in monkeys], reverse=True)
    return num_inspected[0] * num_inspected[1]


def main():
    p1 = do_rounds(20, lambda x: int(x / 3))
    p2 = do_rounds(10000, lambda x: int(x % (11 * 19 * 7 * 17 * 3 * 5 * 13 * 2)))  # Keep the numbers sane

    return p1, p2


if __name__ == '__main__':
    main()
