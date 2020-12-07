use std::str::FromStr;
use std::collections::HashMap;
use common::benchmarking::benchmark_test;

fn test<'a, I>(lines: I) -> (i32, i32) where I: Iterator<Item = &'a String> {
    let nums = lines.map(|line| i32::from_str(&line))
        .map(|res| res.expect("Not an integer"))
        .collect::<Vec<_>>();

    let sums: HashMap<i32, i32> = nums.iter()
        .flat_map(|a| nums.iter().map(move |b| (a + b, a * b)))
        .collect();

    // Find the triplet that sums to 2020
    let triple_sum = nums.iter()
        .flat_map(|a| sums.get(&(2020 - a)).map(|b| a * b))
        .next()
        .expect("Could not find value");

    (*sums.get(&2020).expect("Couldn't find 2020"), triple_sum)
}

fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}