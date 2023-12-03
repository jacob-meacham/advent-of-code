import re

import networkx as nx


def debug_tick(graph, time_remaining, tick_length, total_flow):
    open_valves = []
    for n, v in graph.nodes.items():
        if not v['closed']:
            open_valves.append(n)
    for n in range(1, tick_length + 1):
        print(f'== Minute {30 - time_remaining + n} ==')
        if len(open_valves):
            print(f'{", ".join(open_valves)} are open, releasing {total_flow} pressure')
        else:
            print('No valves are open')


def travelling_salesman(nodes, distance_matrix, open_mask, time_remaining, current_node, cur_release=0, memo={}):
    max_release = 0

    if open_mask in memo:
        memo[open_mask] = max(memo[open_mask], cur_release)
    else:
        memo[open_mask] = cur_release

    for i in range(0, len(nodes)):
        if open_mask & (1 << i) != 0:
            continue

        n, flow_rate = nodes[i]
        cost = distance_matrix[current_node][n] + 1
        if cost > time_remaining:
            # Can't reach it in time
            continue

        new_time_remaining = time_remaining - cost
        pressure_release = flow_rate * new_time_remaining
        max_release = max(
            pressure_release + travelling_salesman(nodes, distance_matrix, open_mask | (1 << i), new_time_remaining, n,
                                                   cur_release + pressure_release, memo),
            max_release)

    return max_release


def solve_p1(nodes, start_node, distance_matrix):
    open_mask = 0
    time_remaining = 30

    return travelling_salesman(nodes, distance_matrix, open_mask, time_remaining, start_node)


def solve_p2(nodes, start_node, distance_matrix):
    # Idea from https://github.com/WinterCore/aoc2022/blob/main/day16/main.rs
    # The goal is to memoize the best flow from a given valve state and then find the best 2 such paths
    # That do not overlap
    open_mask = 0
    time_remaining = 26

    my_memo = {}
    elephant_memo = {}
    travelling_salesman(nodes, distance_matrix, open_mask, time_remaining, start_node, memo=my_memo)
    travelling_salesman(nodes, distance_matrix, open_mask, time_remaining, start_node, memo=elephant_memo)

    max_release = 0
    for my_mask, my_flow in my_memo.items():
        for elephant_mask, elephant_flow in elephant_memo.items():
            if my_mask & elephant_mask == 0 and my_flow + elephant_flow > max_release:
                max_release = my_flow + elephant_flow
    return max_release

    # possible_best = []
    # for my_mask, my_flow in my_memo.items():
    #     possible_best.extend([my_flow + elephant_flow for elephant_mask, elephant_flow in elephant_memo.items() if my_mask & elephant_mask == 0])
    #
    # return max(possible_best)


def main():
    with open('input.txt', 'r') as f:
        input = [l.strip() for l in f.readlines()]

    # Using a graph library instead of re-implementing a shortest path
    tunnel_graph = nx.Graph()
    for line in input:
        m = re.match(r'Valve ([A-Z][A-Z]) has flow rate=(\d*); tunnels? leads? to valves? (.*)', line)
        valve = m.group(1)
        tunnel_graph.add_node(valve, flow_rate=int(m.group(2)))
        edges = m.group(3).split(',')
        tunnel_graph.add_edges_from([(valve, e.strip()) for e in edges])

    # Floyd Warshall distance matrix
    distance_matrix = nx.floyd_warshall(tunnel_graph)
    nodes = [(n, v['flow_rate']) for n, v in list(tunnel_graph.nodes.data()) if v['flow_rate'] != 0]

    p1 = solve_p1(nodes, 'AA', distance_matrix)
    p2 = solve_p2(nodes, 'AA', distance_matrix)

    return p1, p2


if __name__ == '__main__':
    main()
