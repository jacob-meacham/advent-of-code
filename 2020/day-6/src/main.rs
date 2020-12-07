use std::collections::HashSet;
use std::iter::FromIterator;
use common::benchmarking::benchmark_test;

fn test(input: &String) -> (usize, usize) {
    let groups = input.split("\n\n");

    let p1 = groups.clone().fold(0, |count, group| {
        count + group.chars().filter(|c| !c.is_whitespace()).collect::<HashSet<char>>().len()
    });

    // Intersect all answers with each person's answers
    let p2 = groups.clone().fold(0, |count, group| {
        let full_set : HashSet<char> = "abcdefghijklmnopqrstuvwxyz".chars().collect();
        count + group.split_whitespace().fold(full_set, |full_set, answers| {
            let answer_set = answers.chars().collect::<HashSet<char>>();
            HashSet::from_iter(full_set.intersection(&answer_set).cloned())
        }).len()
    });

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
