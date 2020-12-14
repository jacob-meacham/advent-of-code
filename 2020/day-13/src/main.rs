use common::benchmarking::benchmark_test;
use std::str::FromStr;

fn test(lines: &[String]) -> (u32, u64) {
    let timestamp = u32::from_str(&lines[0]).unwrap();
    let parsed_lines = &lines[1].split(',').map(|s| {
        match u32::from_str(s) {
            Ok(x) => Some(x),
            Err(_) => None
        }
    }).collect::<Vec<_>>();

    let (min_n, min_timestamp) = parsed_lines.iter().filter_map(|x| *x)
        .map(|n| {
        (n, (timestamp / n + 1) * n)
    }).min_by(|(_, xt), (_, yt)| xt.cmp(yt)).unwrap();

    // Sieve-esque
    let buses: Vec<(u64, u64)> = parsed_lines.iter().enumerate()
        .filter_map(|(index, n)| n.map_or(None,|b| Some((b as u64, index as u64)))).collect();

    let p2 = buses[1..].iter().fold((buses[0].0, buses[0].0), |(cur_num, multiple), bus| {
        let new_num = (cur_num..).step_by(multiple as usize)
            .find(|n| (n + bus.1) % bus.0 == 0).unwrap();
        (new_num, multiple * bus.0)
    });

    ((min_timestamp - timestamp) * min_n, p2.0)
}
fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(&lines);

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(&lines); });
}
