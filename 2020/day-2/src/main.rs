use std::io::BufRead;
use parse_display::{Display, FromStr};

#[derive(Display, FromStr, PartialEq, Debug)]
#[display("{min}-{max} {letter}: {password}")]
struct PasswordLine {
    min: usize,
    max: usize,
    letter: char,
    password: String
}

fn main() {
    let (p1, p2) = common::input::open_input()
        .lines()
        .fold((0 as usize, 0 as usize), |(p1, p2), line| {
            let l : PasswordLine = line.unwrap().parse().unwrap();

            let new_p1 = {
                let num_letters = l.password.chars().filter(|c| c == &l.letter).count();
                match num_letters >= l.min && num_letters <= l.max {
                    true => p1 + 1,
                    false => p1
                }
            };

            let new_p2 = {
                match (l.password.chars().nth(l.min - 1).unwrap() == l.letter)
                    != (l.password.chars().nth(l.max - 1).unwrap() == l.letter) {
                    true => p2 + 1,
                    false => p2
                }
            };

            (new_p1, new_p2)
    });

    println!("Part 1: {}", p1);
    println!("Part 2: {}", p2);
}