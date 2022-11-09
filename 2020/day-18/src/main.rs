use common::benchmarking::benchmark_test;


// TODO: Lots of ways to do this, but for funsies I'll write my own recursive descent parser
fn test<'a, I>(_lines: I) -> (i32, i32) where I: Iterator<Item = &'a String> {
    (0, 0)
}
fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("❌ Part 1: {}", p1);
    println!("❌ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}
