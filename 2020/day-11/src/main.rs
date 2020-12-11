use common::benchmarking::benchmark_test;
use array2d::Array2D;

fn num_adjacent(row_index: i32, col_index: i32, board: &Array2D<char>, lookahead: u32) -> u32 {
    [(1i32, 0i32), (-1, 0), (0, 1), (0, -1), (1, -1), (-1, 1), (1, 1), (-1, -1)].iter().map(|(x, y)| {
        // Cast our ray in this direction until we get to the extents, or hit a seat.
        let mut row = row_index + x;
        let mut col = col_index + y;
        let mut num_iterations = 0;
        loop {
            if num_iterations >= lookahead || row < 0 || col < 0 || row >= board.num_columns() as i32 || col >= board.num_columns() as i32 {
                break 0 // Got to the extents or our lookahead without ever hitting a seat
            } else {
                match board.get(row as usize, col as usize) {
                    Some('#') => break 1,
                    Some('L') => break 0,
                    _ => ()
                };

                row += x;
                col += y;
                num_iterations += 1;
            }
        }
    }).sum::<u32>()
}

fn permute(old_board: &Array2D<char>, tolerance: u32, lookahead: u32) -> Array2D<char> {
    let mut new_board = old_board.clone();
    for (row_index, row) in old_board.rows_iter().enumerate() {
        for (col_index, &item) in row.enumerate() {
            match item {
                '#' => {
                    if num_adjacent(row_index as i32, col_index as i32, old_board, lookahead) >= tolerance {
                        new_board[(row_index, col_index)] = 'L';
                    }
                },
                'L' => {
                    if num_adjacent(row_index as i32, col_index as i32, old_board, lookahead) == 0 {
                        new_board[(row_index, col_index)] = '#';
                    }
                },
                _ => { }
            };
        }
    }

    new_board
}

fn test_loop(rows: &[Vec<char>], tolerance: u32, lookahead: u32) -> usize {
    let mut board = Array2D::from_rows(&rows);
    loop {
        let new_board = permute(&board, tolerance, lookahead);

        let eq = board == new_board;
        board = new_board;

        if eq { break board }
    }.elements_row_major_iter().filter(|&&seat| seat == '#').count()
}

fn test<'a, I>(lines: I) -> (usize, usize) where I: Iterator<Item = &'a String> {
    let rows: Vec<Vec<char>> = lines.map(|line| line.chars().collect::<Vec<char>>()).collect();

    let p1 = test_loop(&rows, 4, 1);
    let p2 = test_loop(&rows, 5, 1000);

    (p1, p2)
}
fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}
