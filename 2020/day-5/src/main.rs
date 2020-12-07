use std::io::BufRead;
use std::collections::HashSet;

// Just treat the slices as binary numbers
fn main() {
    let seats: HashSet<i32> = common::input::open_input().lines().map(|line| {
        let seat = line.unwrap();

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
        *seat > &10 && *seat < &(max_seat - 10)
    }).next().unwrap();

    println!("Part 1: {}", seats.iter().max().unwrap());
    println!("Part 2: {}", p2);
}

