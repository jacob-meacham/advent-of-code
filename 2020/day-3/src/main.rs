use common::benchmarking::benchmark_test;

struct Toboggan {
    x: u32,
    y: u32,
    position: u32,
    num_trees: u32
}

impl Toboggan {
    fn new(x: u32, y: u32) -> Self {
        Toboggan {
            x,
            y,
            position: 0,
            num_trees: 0
        }
    }
}

fn test<'a, I>(lines: I) -> (u32, u32) where I: Iterator<Item = &'a String> {
    let toboggans = [Toboggan::new(3, 1), Toboggan::new(1, 1), Toboggan::new(5, 1), Toboggan::new(7, 1), Toboggan::new(1, 2)];
    let traveled_toboggans = lines.skip(1)
        .enumerate()
        .fold(toboggans, |mut toboggans, (idx, line)| {
            // Fold over all of the slopes at the same time.
            // We mutate in place because the idea of creating n new vectors for each line of input was icky.
            toboggans.iter_mut().for_each(|toboggan| {
                if (idx + 1) as u32 % toboggan.y != 0 {
                    // We don't need to touch this toboggan on this line
                    return
                }

                let is_tree = match line.chars().nth((toboggan.position + toboggan.x) as usize % line.len()).unwrap() {
                    '.' => 0,
                    '#' => 1,
                    _ => 0
                };

                toboggan.position += toboggan.x;
                toboggan.num_trees += is_tree;
            });

            toboggans
        });

    (traveled_toboggans[0].num_trees, traveled_toboggans.iter().map(|t| t.num_trees).product::<u32>())
}

fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}
