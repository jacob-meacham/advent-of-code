use common::benchmarking::benchmark_test;
use std::collections::HashSet;
use std::str::FromStr;

#[derive(Copy, Clone, PartialEq)]
enum OpCode {
    NOP,
    JMP,
    ACC
}

fn compile<'a, I>(lines: I) -> Vec<(OpCode, i32)> where I: Iterator<Item = &'a String> {
    lines.map(|instruction| {
        let v: Vec<&str> = instruction.splitn(2, ' ').collect();
        let opcode = match v[0] {
            "nop" => OpCode::NOP,
            "jmp" => OpCode::JMP,
            "acc" => OpCode::ACC,
            _ => panic!("Unknown instruction")
        };

        (opcode, i32::from_str(v[1]).unwrap())
    }).collect::<Vec<_>>()
}

fn execute_instruction(instruction: &(OpCode, i32), instruction_pointer: i32, accumulator: i32) -> (i32, i32) {
    let (opcode, value) = instruction;
    match opcode {
        OpCode::NOP => (instruction_pointer + 1, accumulator),
        OpCode::JMP => (instruction_pointer + value, accumulator),
        OpCode::ACC => (instruction_pointer + 1, accumulator + value)
    }
}

fn find_loop(program: &[(OpCode, i32)]) -> (bool, i32) {
    let mut visited_instructions: HashSet<usize> = HashSet::new();
    let mut instruction_pointer: i32 = 0;
    let mut accumulator: i32 = 0;

    loop {
        if !visited_instructions.insert(instruction_pointer as usize) {
            break (true, accumulator);
        }

        let (nip, na) = execute_instruction(&program[instruction_pointer as usize], instruction_pointer, accumulator);
        instruction_pointer = nip;
        accumulator = na;
        if instruction_pointer >= program.len() as i32 {
            break (false, accumulator);
        }
    }
}

fn test<'a, I>(lines: I) -> (i32, i32) where I: Iterator<Item = &'a String> {
    let compiled_program = compile(lines);
    let (_, p1) = find_loop(&compiled_program);

    // Although there are probably some cool searches to do this, we can brute force all possible programs in the space very quickly (<40ms avg runtime)
    // One option would be to memoize searches for instructions that halt. We could also attempt to search backwards
    let p2 = (0..compiled_program.len() as usize).find_map(|i| {
        match compiled_program[i].0 {
            OpCode::NOP | OpCode::JMP => {
                let mut v = compiled_program.clone();
                v[i].0 = if v[i].0 == OpCode::JMP { OpCode::NOP } else { OpCode::JMP };
                Some(v)
            },
            _ => None
        }.and_then(|program| {
            let (is_loop, accumulator) = find_loop(&program);
            match is_loop {
                true => None,
                false => Some(accumulator) // No loop, we found a program that terminates
            }
        })
    }).unwrap();

    (p1, p2)
}

fn main() {
    let lines = common::input::open_input_as_vector();
    let (p1, p2) = test(lines.iter());

    println!("✅ Part 1: {}", p1);
    println!("✅ Part 2: {}", p2);

    benchmark_test(|| { test(lines.iter().clone()); });
}
