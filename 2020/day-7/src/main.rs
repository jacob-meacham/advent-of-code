#[macro_use]
extern crate lazy_static;
use regex::{Regex, Captures};
use common::benchmarking::benchmark_test;
use petgraph::graphmap::{DiGraphMap};
use std::str::FromStr;
use petgraph::{Direction};
use std::collections::{HashSet, VecDeque};

fn parse_child_bag(bag: &str) -> Option<(u32, &str)> {
    lazy_static! {
        static ref CHILD_BAG: Regex = Regex::new(r"(\d+)\s*(.*?)s?$").unwrap();
    }

    CHILD_BAG.captures(bag).and_then(|c| {
        Some((u32::from_str(&c[1]).expect("Could not parse number of bags"), c.get(2).unwrap().as_str()))
    })
}

fn dfs_incoming(graph: &DiGraphMap<&str, u32>, root: &str) -> u32 {
    let mut search_queue = VecDeque::with_capacity(1000);
    let mut visited_nodes = HashSet::new();
    search_queue.push_back(root);
    while !search_queue.is_empty() {
        let node = search_queue.pop_back().unwrap();
        visited_nodes.insert(node);
        for neighbor in graph.neighbors_directed(node, Direction::Incoming).filter(|n| !visited_nodes.contains(n)) {
            search_queue.push_back(neighbor)
        }
    }

    (visited_nodes.len() - 1) as u32
}

fn find_num_bags(graph: &DiGraphMap<&str, u32>, root: &str) -> u32 {
    graph.edges(root)
        .map(|(_, to, &edge)| edge + (edge * find_num_bags(graph, to)))
        .sum()
}

fn test<'a, I>(lines: I) -> (u32, u32) where I: Iterator<Item = &'a String> {
    lazy_static! {
        static ref PARENT_BAG: Regex = Regex::new(r"(.*?)(?:s contain | contain )(.*?)\.").unwrap();
    }

    let mut graph = DiGraphMap::new();
    lines.flat_map(|line| { // Get a list of all parents and their children
        let c : Captures = PARENT_BAG.captures(line).expect("Could not parse bag line");
        let parent = c.get(1).expect("Could not parse name").as_str();
        let children = c.get(2).expect("Could not parse children").as_str().split(", ");

        children.map(move |child| (parent, child))
    }).filter(|(_, child)| child != &"no other bags") // Filter out all leaves, since no outgoing edges are required
        .map(|(parent, child)| (parent, parse_child_bag(child).expect("Could not parse bag")))
        .for_each(|(parent, (num, child))| {
            graph.add_edge(parent, &child, num);
        });

    let p1 = dfs_incoming(&graph, "shiny gold bag");
    let p2 = find_num_bags(&graph, "shiny gold bag");
    (p1, p2)
}

fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}
