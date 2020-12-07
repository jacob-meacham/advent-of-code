use std::collections::HashSet;
use common::benchmarking::benchmark_test;

fn test(input: &str) -> (usize, usize) {
    let groups = input.split("\n\n");

    let p1 = groups.clone()
        .map(|g| g.chars()
        .filter(|c| !c.is_whitespace())
        .collect::<HashSet<_>>()
        .len()).sum();

    // Intersect all answers with each person's answers
    let full_set : HashSet<char> = "abcdefghijklmnopqrstuvwxyz".chars().collect();
    let p2 = groups.clone().map(|group| {
        group.split_whitespace()
            .map(|s| s.chars().collect::<HashSet<char>>())
            .fold(full_set.clone(), |intersection, answers| {
                (&intersection) & (&answers)
        }).len()
    }).sum();

    (p1, p2)
}

fn main() {
    let mut input = String::new();
    common::input::open_input().read_to_string(&mut input).expect("Could not read input");

    let (p1, p2) = test(&input);
    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(&input); });
}
