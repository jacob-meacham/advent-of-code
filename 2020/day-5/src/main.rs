use std::collections::HashSet;
use common::benchmarking::benchmark_test;

// Just treat the slices as binary numbers
fn test<'a, I>(lines: I) -> (i32, i32) where I: Iterator<Item = &'a String> {
    let seats: HashSet<i32> = lines.map(|seat| {
        let row_str = String::from(&seat[0..7])
            .replace("B", "1")
            .replace("F", "0");
        let row = isize::from_str_radix(&row_str, 2).unwrap();

        let column_str = String::from(&seat[7..10])
            .replace("R", "1")
            .replace("L", "0");
        let column = isize::from_str_radix(&column_str, 2).unwrap();

        (row * 8 + column) as i32
    }).collect();

    // Get missing seat ids
    let max_seat = seats.iter().max().unwrap();

    let all_seats : HashSet<i32> = (0..=*max_seat).collect();
    let p2 = all_seats.difference(&seats).filter(|seat| {
        *seat > &50 && *seat < &(max_seat - 50)
    }).next().unwrap();

    (*max_seat, *p2)
}
fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}
