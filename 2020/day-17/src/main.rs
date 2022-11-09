#[macro_use]
extern crate lazy_static;
use common::benchmarking::benchmark_test;
use multiarray::Array3D;
use itertools::Itertools;

// Use kd-tree if necessary
fn step_voxel(world: &Array3D<bool>, x: usize, y: usize, z: usize) -> bool {
    lazy_static! {
        // Compute the product of 2d to 3d
        static ref SEARCH_VECTORS: Vec<[i32; 3]> = [[0,0], [1, 0], [-1, 0], [0, 1], [0, -1], [1, 1], [-1, -1], [1, -1], [-1, 1]].iter()
        .cartesian_product(&[0, 1, -1])
        .map(|(xy, z)| {
            [ xy[0], xy[1], *z ]
        })
        .filter(|coord| *coord != [0, 0, 0])
        .collect();
    }

    let voxel = world[[x, y, z]];
    [0].iter().len()

    let active_neighbors = SEARCH_VECTORS.iter()
        .filter(|coord| world[[x + coord[0] as usize, y + coord[0] as usize, z + coord[0] as usize]])
        .len();

    match voxel {
        true => if active_neighbors == 2 || active_neighbors == 3 { false } else { true },
        false => if active_neighbors == 3 { true } else { false },
    }
}

fn test(input: &str) -> (u32, u64) {
    let mut current_world = Array3D::new([100,100,100], false);

    step_voxel(&current_world, 0, 0, 0);

    // Set the initial world up. We'll need to rebase upwards into the world.

    // Create the next world according to the rules
    //current_world.eliminated_dim()
    (0, 0)
}

fn main() {
    let mut input = String::new();
    common::input::open_input().read_to_string(&mut input).expect("Could not read input");

    let (p1, p2) = test(&input);

    println!("❌ Part 1: {}", p1);
    println!("❌ Part 2: {}", p2);

    //benchmark_test(|| { test(&input); });
}
