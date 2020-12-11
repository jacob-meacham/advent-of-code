use common::benchmarking::benchmark_test;
use std::str::FromStr;
//use itertools::iproduct;

#[cfg(target_arch = "x86")]
use std::arch::x86::*;
#[cfg(target_arch = "x86_64")]
use std::arch::x86_64::*;

macro_rules! bit_mask_u64 {
    ($bitmask:tt, $bit:tt) => {
      (1 & $bitmask >> $bit) as u64
    };
}

fn test(input: &str) -> (u32, u64) {
    let mut all: Vec<_> = input.split_whitespace()
        .map(|n| u8::from_str(n).unwrap()).collect();
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
    let p2 = all.iter().rev().fold([(0, 0), (0, 0), (adapter_jolt, 1)], |acc, num| unsafe {
        // We accumulate the subtrees in reverse. We only need to keep track of the last three since nodes
        // are only connected for +1, +2, or +3.

        // One option:
        // let subtree_sum: u64 = iproduct!(&[num + 1, num + 2, num + 3], &acc).map(|(search_num, (subtree_num, size))| {
        //     if search_num == subtree_num { *size } else { 0 }
        // }).sum();

        // Another:
        // Unroll our comparisons then use simd to compare all at once and multiply the resulting bitmask against the appropriate sizes
        let a = _mm_set_epi8((num + 1) as i8, (num + 2) as i8, (num + 3) as i8,
                                      (num + 1) as i8, (num + 2) as i8, (num + 3) as i8,
                                       (num + 1) as i8, (num + 2) as i8, (num + 3) as i8, 0, 0, 0, 0, 0, 0, 0);
        let b = _mm_set_epi8(acc[0].0 as i8, acc[1].0 as i8, acc[2].0 as i8,
                                      acc[1].0 as i8, acc[2].0 as i8, acc[0].0 as i8,
                                       acc[2].0 as i8, acc[0].0 as i8, acc[1].0 as i8, 1, 1, 1, 1, 1, 1, 1);
        let bitmask = _mm_movemask_epi8(_mm_cmpeq_epi8(a, b));

        // Unroll bitmask. Could probably do this in SIMD as well
        let subtree_sum = (acc[0].1 * bit_mask_u64!(bitmask, 15)) + (acc[1].1 * bit_mask_u64!(bitmask, 14)) + (acc[2].1 * bit_mask_u64!(bitmask, 13)) +
                          (acc[1].1 * bit_mask_u64!(bitmask, 12)) + (acc[2].1 * bit_mask_u64!(bitmask, 11)) + (acc[0].1 * bit_mask_u64!(bitmask, 10)) +
                          (acc[2].1 * bit_mask_u64!(bitmask, 9)) + (acc[0].1 * bit_mask_u64!(bitmask, 8)) + (acc[1].1 * bit_mask_u64!(bitmask, 7));

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
