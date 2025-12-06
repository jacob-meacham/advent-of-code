import re
from functools import reduce
from termcolor import colored


class INode:
    def __init__(self, name, parent):
        self.name = name
        self.parent = parent
        self.nodes = []
        self.dirty = True
        self.cached_size = 0

    def get_size(self):
        if not self.dirty:
            return self.cached_size

        size = 0
        for n in self.nodes:
            size += n.get_size()
        self.dirty = False
        self.cached_size = size
        return size

    def is_dir(self):
        return True

    def add_node(self, n):
        self.nodes.append(n)
        self.dirty = True

    def find_node(self, name):
        for n in self.nodes:
            if n.name == name:
                return n
        return None

    def print(self, depth=0):
        tabs = '\t' * depth
        print(f'{tabs} - {self.name} (dir)')
        for n in self.nodes:
            n.print(depth + 1)


class FileNode(INode):
    def __init__(self, name, size, parent):
        self.name = name
        self.size = int(size)
        self.parent = parent

    def get_size(self):
        return self.size

    def is_dir(self):
        return False

    def find_node(self, name):
        raise AssertionError("Can't find nodes on file")

    def add_node(self, n):
        raise AssertionError("Can't add nodes to file")

    def print(self, depth=0):
        tabs = '\t' * depth
        print(f'{tabs} - {self.name} (file, size={self.size})')


def p1(root_node):
    def recurse(cur_node, matching_nodes=[]):
        if cur_node.get_size() <= 100000:
            matching_nodes.append(cur_node)
        nodes_to_try = [n for n in cur_node.nodes if n.is_dir()]
        for n in nodes_to_try:
            recurse(n, matching_nodes)

    matching_nodes = []
    recurse(root_node, matching_nodes)

    return reduce(lambda acc, xs: acc + xs.get_size(), matching_nodes, 0)


def p2(root_node):
    unused_space = 70000000 - root_node.get_size()
    required_space = 30000000 - unused_space

    # Find all directories which we could delete to free up the space
    # For fun, we do it as a stack instead
    matching_nodes = []
    visit = [root_node]
    while len(visit) > 0:
        cur_node = visit.pop()
        if cur_node.get_size() > required_space:
            matching_nodes.append(cur_node)

        nodes_to_try = [n for n in cur_node.nodes if n.is_dir()]
        visit.extend(nodes_to_try)

    # Sort and take the smallest
    s = sorted(matching_nodes, key=lambda n: n.get_size())
    return s[0].get_size()


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    root_node = None
    current_node = None
    for line in input:
        # Command
        if line[0:2] == '$ ':
            command = line[2:4]
            command_in = line[5:]

            match command:
                case 'cd':
                    if not root_node:
                        root_node = INode(command_in, None)
                        current_node = root_node
                    elif command_in == '..':
                        current_node = current_node.parent
                    else:
                        current_node = current_node.find_node(command_in)
                case 'ls':
                    pass
        else:
            if line[0:3] == 'dir':
                new_node = INode(line[4:], current_node)
                current_node.add_node(new_node)
            else:
                match = re.match(r'(\d+) (.*)', line)
                new_node = FileNode(match.group(2), match.group(1), current_node)
                current_node.add_node(new_node)

    root_node.get_size()  # Pre-calculate sizes

    _p1 = p1(root_node)
    _p2 = p2(root_node)
    return _p1, _p2


if __name__ == '__main__':
    p1, p2 = main()
    print(colored('Part 1: ', 'white') + colored(str(p1), 'green', attrs=['bold']) + 
          colored(' Part 2: ', 'white') + colored(str(p2), 'green', attrs=['bold']))
