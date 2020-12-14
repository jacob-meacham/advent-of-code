#[macro_use]
extern crate lazy_static;
use regex::Regex;
use common::benchmarking::benchmark_test;
use std::str::FromStr;
use hashbrown::HashMap;
use crate::ProgramOp::{Mask, Memory};

fn mask_from_str(mask: &str) -> u64 {
    mask.chars().rev().enumerate().fold(0, |masked_number, (index, c)| {
        match c {
            'X' => masked_number + (1 << index),
            '1' => masked_number,
            '0' => masked_number,
            _ => panic!("Unexpected mask!")
        }
    })
}

fn apply_bitmask(val: u64, mask: &str) -> u64 {
    mask.chars().rev().enumerate().fold(0, |masked_number, (index, c)| {
      match c {
          'X' => masked_number + ((1 << index) & val),
          '1' => masked_number + (1 << index),
          '0' => masked_number,
          _ => panic!("Unexpected mask!")
      }
    })
}

// Quite a bit slower!
// fn apply_bitmask2(val: u64, mask: &str) -> u64 {
//     let b = u64::from_str_radix(&mask.replace('X', "0"), 2).unwrap();
//     let c = u64::from_str_radix(&mask.replace('1', "0").replace('X', "1"), 2).unwrap();
//
//     b | (val & c)
// }

// Create all possible values of the mask to apply
fn calculate_masks(mask: &str) -> Vec<u64> {
    let mut masks= Vec::with_capacity(1024);
    masks.insert(0, 0);
    mask.chars().rev().enumerate().for_each(|(index, c)| {
        match c {
            'X' => {
                // The flat map is nicer but slower
                //masks = masks.iter().flat_map(|&num| [num, num + (1 << index)].to_vec()).collect();
                masks.append(&mut masks.iter().map(|&num| num + (1 << index)).collect::<Vec<_>>());
            },
            '1' => {
                masks = masks.iter().map(|num| num + (1 << index)).collect();
            },
            _ => ()
        };
    });

    masks
}

enum ProgramOp {
    Mask(String),
    Memory(u64, u64)
}

fn test(lines: &[String]) -> (u64, u64) {
    lazy_static! {
        static ref MEM: Regex = Regex::new(r"mem\[(\d+)\] = (\d+)").unwrap();
    }

    let program: Vec<ProgramOp> = lines.iter().map(|line| {
        match line.contains("mask") {
            true => Mask(String::from(line.split(" = ").nth(1).unwrap())),
            false => {
                MEM.captures(line).map(|c| {
                    let address = u64::from_str(&c[1]).unwrap();
                    let val = u64::from_str(&c[2]).unwrap();
                    Memory(address, val)
                }).unwrap()
            }
        }
    }).collect();

    let mut active_mask = String::new();
    let mut memory: HashMap<u64, u64> = HashMap::new();

    for op in program.iter() {
        match op {
            Mask(m) => active_mask = m.clone(),
            &Memory(address, value) => {
                memory.insert(address, apply_bitmask(value, &active_mask));
            }
        }
    }

    let p1 = memory.values().sum();

    let mut all_masks: Vec<u64> = Vec::new(); // All possible values the mask can take on
    let mut floating_mask: u64 = 0; // 1 for all floating values, zero otherwise
    memory = HashMap::with_capacity(2 << 18);

    for op in program.iter() {
        match op {
            Mask(m) => {
                floating_mask = mask_from_str(&m);
                all_masks = calculate_masks(&m);
            },
            &Memory(address, value) => {
                for m in &all_masks {
                    // The 3-variable truth table for val, mask, floating_mask corresponds to mask | (address & !floating_mask)
                    // We clear all floating bits from the value and then or that with each possible mask combination.
                    let address_to_write = *m | (address & !floating_mask);
                    memory.insert(address_to_write, value);
                }
            }
        }
    }

    let p2 = memory.values().sum();

    (p1, p2)
}

fn main() {
    let lines = common::input::open_input_as_vector();

    let (p1, p2) = test(&lines);

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(&lines); });

}
