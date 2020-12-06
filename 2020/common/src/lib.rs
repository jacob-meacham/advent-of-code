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
}

pub fn why_not() { }