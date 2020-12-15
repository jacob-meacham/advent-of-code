use common::benchmarking::benchmark_test;
use hashbrown::HashMap;

fn _find_target(input: &[u32], target: u32) -> u32 {
    // We don't put the very last item of the input in, as that is our starting number to consider
    let mut previously_seen: HashMap<u32, u32> = input[0..input.len()-1].iter()
        .enumerate()
        .map(|(index, &n)| (n, index as u32 + 1))
        .collect();
    previously_seen.reserve(1000000); // Attempt to reduce the memory allocation costs

    (input.len() as u32..target).fold(*input.last().unwrap(), |last_seen, n| {
        n - previously_seen.insert(last_seen, n).unwrap_or(n) // If it does exist, return it. If not, return n which then folds to 0.
    })
}

// This takes around 1500ms
fn _test(input: &[u32]) -> (u32, u32) {
    (_find_target(input, 2020), _find_target(input, 30000000))
}

// This takes around 600ms
fn _test_fast(input: &[u32]) -> (u32, u32) {
    // Relies on knowing how big to make the vector.
    let mut previously_seen = vec![-1; 30000000];

    input[0..input.len()-1].iter()
        .enumerate()
        .for_each(|(index, &n)| previously_seen[n as usize] = index as i32 + 1);

    let mut p1 = 0;
    let p2 = (input.len() as u32..30000000).fold(*input.last().unwrap(), |last_seen, n| {
        if n == 2020 {
            p1 = last_seen;
        }
        let x = previously_seen[last_seen as usize];
        previously_seen[last_seen as usize] = n as i32;
        match x {
            -1 => 0,
            _ => n - x as u32,
        }
    });

    (p1, p2)
}
fn main() {
    let input = [18,11,9,0,5,1];

    let (p1, p2) = _test_fast(&input);

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { _test_fast(&input); });
}
