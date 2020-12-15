use common::benchmarking::benchmark_test;
use std::str::FromStr;
use hashbrown::HashMap;

fn test(input: &str) -> (u32, u32) {
    let input_vec: Vec<_> = input.split(',')
        .map(|n| u32::from_str(n).unwrap())
        .collect();
    // We don't put the very last item of the input in, as that is our starting number to consider
    let mut previously_seen_map: HashMap<u32, u32> = input_vec[0..input_vec.len()-1].iter()
        .enumerate()
        .map(|(index, &n)| (n, index as u32 + 1))
        .collect();
    previously_seen_map.reserve(20000);

    // TODO: Cleanup
    let mut last_seen = *input_vec.last().unwrap();
    let mut p1 = 0;
    for n in input_vec.len() as u32..30000000 {
        if n == 2020 {
            p1 = last_seen;
        }
        if let Some(&x) = previously_seen_map.get(&last_seen) {
            previously_seen_map.insert(last_seen, n);
            last_seen = n - x;
        } else {
            previously_seen_map.insert(last_seen, n);
            last_seen = 0;
        }
    }

    (p1, last_seen)
}
fn main() {
    let input = "18,11,9,0,5,1";

    let (p1, p2) = test(&input);

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(&input); });
}
