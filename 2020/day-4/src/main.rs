#[macro_use]
extern crate lazy_static;
use regex::Regex;
use std::str::FromStr;
use std::collections::HashMap;
use common::benchmarking::benchmark_test;

fn is_valid1(s: &str) -> bool {
    ["byr", "iyr", "eyr", "hgt", "hcl", "ecl", "pid"].iter().all(|k| s.contains(k))
}

fn is_valid2(s: &str) -> bool {
    lazy_static! {
        static ref HEIGHT: Regex = Regex::new(r"(\d+)(cm|in)").unwrap();
        static ref HAIR: Regex = Regex::new(r"#[0-9a-f]{6}").unwrap();
        static ref ID: Regex = Regex::new(r"\d{9}").unwrap();
    }
    fn year_valid(year: &str, min: u32, max: u32) -> bool {
        let iyear = u32::from_str(year).expect("Could not parse year");
        min <= iyear && iyear <= max
    }
    let result : HashMap<&str, bool> = s.split_whitespace().map(|pair| {
        let kv = pair.split(':').collect::<Vec<&str>>();
        let (key, value): (&str, &str) = (kv[0], kv[1]);
        let is_valid = match key {
            "byr" => year_valid(value, 1920, 2002),
            "iyr" => year_valid(value, 2010, 2020),
            "eyr" => year_valid(value, 2020, 2030),
            "hgt" => {
                HEIGHT.captures(value).map_or(false, |c| {
                    let height = u32::from_str(&c[1]).unwrap();
                    match &c[2] {
                        "cm" => 150 <= height && height <= 193,
                        "in" => 59 <= height && height <= 76,
                        _ => panic!("Unknown unit")
                    }
                })
            },
            "hcl" => HAIR.is_match(value),
            "ecl" => {
                ["amb", "blu", "brn", "gry", "grn", "hzl", "oth"].contains(&value)
            },
            "pid" => ID.is_match(value),
            "cid" => true,
            _ => panic!("Unknown key")
        };

        (key, is_valid)
    }).collect();

    is_valid1(s) && result.values().all(|v| v == &true)
}

fn test(input: &str) -> (usize, usize) {
    let lines = input.split("\n\n").collect::<Vec<&str>>();

    let p1 = lines.iter().filter(|l| is_valid1(*l)).count();
    let p2 = lines.iter().filter(|l| is_valid2(*l)).count();

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
