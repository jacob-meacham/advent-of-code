use common::benchmarking::benchmark_test;
use std::str::FromStr;
use std::collections::{VecDeque, HashSet};
use std::iter::FromIterator;

fn find_p1(all: &[i64]) -> i64 {
    let mut last_25: VecDeque<&i64> = VecDeque::from_iter(&all[0..25]);
    for n in &all[25..] {
        let difference: HashSet<_> = last_25.iter().map(|&d| n - d).collect();
        // Ideally, I'd figure out difference.is_disjoint(set)
        if last_25.iter().find(|&&d| difference.contains(d)).is_none() {
            return *n;
        }

        last_25.pop_front();
        last_25.push_back(n);
    }
    0
}

fn find_p2(all: &[i64], needle: i64) -> i64 {
    let mut start_index = 0;
    let mut sum = 0;

    // Could probably fold/scan instead
    for (end_index, n) in all.iter().enumerate() {
        sum += n;
        while sum > needle {
            sum -= all[start_index];
            start_index += 1;
        }

        if sum == needle {
            return all[start_index..end_index].iter().min().unwrap() + all[start_index..end_index].iter().max().unwrap();
        }
    }
    0
}

// Just treat the slices as binary numbers
fn test(input: &str) -> (i64, i64) {
    let all: Vec<_> = input.split_whitespace()
        .map(|n| i64::from_str(n).unwrap()).collect();

    let p1 = find_p1(&all);
    let p2 = find_p2(&all, p1);

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
