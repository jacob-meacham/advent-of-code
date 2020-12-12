use common::benchmarking::benchmark_test;
use std::str::FromStr;

fn rotate(x: i32, y: i32, angle: i32) -> (i32, i32) {
    let (sin, cos) = match angle {
        0 => (0, 1),
        90 => (1, 0),
        180 => (0, -1),
        270 => (-1, 0),
        _ => panic!("Unexpected angle")
    };

    let nx = (cos * x) + (sin * y);
    let ny = (cos * y) - (sin * x);

    (nx as i32, ny as i32)
}

fn test(lines: Vec<String>) -> (i32, i32) {
    let (x1, y1, _) = lines.iter().fold((0i32, 0i32, 0i32), |(x, y, facing), line| {
        let val = i32::from_str(&line[1..]).unwrap();
        match line.chars().nth(0) {
            Some('N') => (x, y + val, facing),
            Some('S') => (x, y - val, facing),
            Some('E') => (x + val, y, facing),
            Some('W') => (x - val, y, facing),
            Some('L') => {
                let mut new_facing = facing - val;
                if new_facing < 0 { new_facing = 360 + new_facing }
                (x, y, new_facing)
            },
            Some('R') => (x, y, (facing + val) % 360),
            Some('F') => {
                match facing {
                    0 => (x + val, y, facing), // East
                    90 => (x, y - val, facing),
                    180 => (x - val, y, facing),
                    270 => (x, y + val, facing),
                    _ => panic!("Oh no!")
                }
            }
            _ => panic!("Oh no!")
        }
    });

    let p1 = x1.abs() + y1.abs();

    let ((x2, y2), _) = lines.iter().fold(((0i32, 0i32), (10i32, 1i32)), |((ship_x, ship_y), (wx, wy)), line| {
        let val = i32::from_str(&line[1..]).unwrap();
        match line.chars().nth(0) {
            Some('N') => ((ship_x, ship_y), (wx, wy + val)),
            Some('S') => ((ship_x, ship_y), (wx, wy - val)),
            Some('E') => ((ship_x, ship_y), (wx + val, wy)),
            Some('W') => ((ship_x, ship_y), (wx - val, wy)),
            Some('L') => {
                let (nwx, nwy) = rotate(wx, wy, 360 - val);
                ((ship_x, ship_y), (nwx, nwy))
            },
            Some('R') => {
                let (nwx, nwy) = rotate(wx, wy, val);
                ((ship_x, ship_y), (nwx, nwy))
            }
            Some('F') => {
                ((ship_x + wx * val, ship_y + wy * val), (wx, wy))
            }
            _ => panic!("Oh no!")
        }
    });

    let p2 = x2.abs() + y2.abs();

    (p1, p2)
}
fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.clone());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.clone()); });
}
