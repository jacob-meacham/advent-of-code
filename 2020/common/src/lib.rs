pub mod input {
    use std::io::{self, BufRead};
    use std::fs::File;
    use std::env;

    // This will pull from env args or else stdin
    pub fn open_input() -> Box<dyn BufRead> {
        let reader : Box<dyn BufRead> = match env::args().nth(1) {
            None => Box::new(io::BufReader::new(io::stdin())),
            Some(path) => Box::new(io::BufReader::new(File::open(path).expect("Could not open file")))
        };
        return reader;
    }

    pub fn open_input_as_vector() -> Vec<String> {
        open_input().lines().map(|l| l.unwrap()).collect::<Vec<String>>()
    }
}

pub mod benchmarking {
    use std::time::Instant;

    pub fn time<F>(f: F, iterations: u128) -> f64
        where F: Fn() {
        let start = Instant::now();
        for _ in 0..iterations {
            bencher::black_box(f());
        }

        let end = Instant::now();

        let avg_nanos = end.duration_since(start).as_nanos() / iterations;
        let avg_secs = avg_nanos as f64 / 1_000_000_000.0;

        avg_secs
    }

    pub fn benchmark_test<F>(f: F)
        where F: Fn() {
        let avg_seconds = time(f, 20);
        if avg_seconds > 0.5 {
            println!("❌ Average Seconds: {}", avg_seconds);
        } else {
            println!("✅ Average Seconds: {}", avg_seconds);
        }
    }
}