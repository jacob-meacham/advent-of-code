use common::benchmarking::benchmark_test;
use std::str::FromStr;

fn test(input: &str) -> (u64, u64) {
    let mut all: Vec<_> = input.split_whitespace()
        .map(|n| u64::from_str(n).unwrap()).collect();
    all.sort_unstable();

    // start with 1 in threes to account for the built-in adapter
    let (ones, threes, _) = all.iter().fold((0, 1, 0), |(ones, threes, last_number), n| {
        match n - last_number {
            1 => (ones + 1, threes, *n),
            3 => (ones, threes + 1, *n),
            _ => (ones, threes, *n)
        }
    });

    let p1 = ones * threes;

    let adapter_jolt = all.iter().max().unwrap()+3;
    let p2 = all.iter().rev().fold([(0, 0), (0, 0), (adapter_jolt, 1)], |acc, num| {
        // We accumulate the subtrees in reverse. We only need to keep track of the last three since nodes
        // are only connected for +1, +2, or +3.
        let subtree_sum: u64 = [num + 1, num + 2, num + 3].iter().fold(0, |sum, search_num| {
            sum + if search_num == &acc[0].0 { acc[0].1 } else { 0 } +
                if search_num == &acc[1].0 { acc[1].1 } else { 0 } +
                if search_num == &acc[2].0 { acc[2].1 } else { 0 }
        });

        // Move the last node out of scope, add ours in with the subtree
        [(*num, subtree_sum), acc[0], acc[1]]
    })[0].1; // The final node we inserted will have the total.

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
