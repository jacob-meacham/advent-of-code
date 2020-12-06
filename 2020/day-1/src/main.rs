use std::io::BufRead;
use std::str::FromStr;
use std::collections::HashMap;

fn main() {
    let reader = common::input::open_input();

    let nums = reader.lines().map(|line| i32::from_str(&line.unwrap()))
        .map(|res| res.expect("Not an integer"))
        .collect::<Vec<_>>();

    let sums: HashMap<i32, i32> = nums.iter()
        .flat_map(|a| nums.iter().map(move |b| (a + b, a * b)))
        .collect();
    println!("Part 1: {}", sums.get(&2020).expect("Couldn't find 2020"));

    // Find the triplet that sums to 2020
    let triple_sum = nums.iter()
        .flat_map(|a| sums.get(&(2020 - a)).map(|b| a * b))
        .next()
        .expect("Could not find value");
    println!("Part 2: {}", triple_sum);
}