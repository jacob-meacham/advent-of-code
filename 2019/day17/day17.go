package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	image := make([][]string, 0)
	image = append(image, []string{})
	curLine := 0

	vm := &VM.VM{}
	outputFn := func(val int) {
		if val == 10 {
			curLine++
		} else {
			// Output ends with a few \n, so wait to add to the image until we get something else
			if curLine >= len(image) {
				image = append(image, []string{})
			}

			image[curLine] = append(image[curLine], string(rune(val)))
		}
	}

	vm.Init(VM.MemoryFromProgram(input), VM.WithOutputFunction(outputFn))
	vm.Run()

	alignmentSum := 0
	for y := 1; y < len(image)-1; y++ {
		for x := 1; x < len(image[y])-1; x++ {
			if image[y][x] == "#" && image[y-1][x] == "#" && image[y+1][x] == "#" && image[y][x-1] == "#" && image[y][x+1] == "#" {
				alignmentSum += x * y
			}
		}
	}
	return alignmentSum
}

// ........................#############......................
// ........................#...........#......................
// ........................#...........#......................
// ........................#...........#......................
// ........................#...........#############..........
// ........................#.......................#..........
// ........................#.......................#..........
// ........................#.......................#..........
// ........................#.......................#..........
// ........................#.......................#..........
// ........................#.......................#..........
// ........................#.......................#..........
// ....................#####.......................#..........
// ....................#...........................#..........
// ....................#.........................#####.#......
// ....................#.........................#.#.#.#......
// ........#####.....#########.................#####.#.#......
// ........#...#.....#.#.....#.................#.#...#.#......
// ........#...#.....#.#.....#.................#.############^
// ........#...#.....#.#.....#.................#.....#.#......
// #######.#...#############.#.................#.....#.#......
// #.....#.#.........#.#...#.#.................#.....#.#......
// #.....#.#.........#.#.#####.................#########......
// #.....#.#.........#.#.#.#.........................#........
// #.....#############.#####.........................#........
// #.......#.............#...........................#........
// #.......#.............#.......................#####........
// #.......#.............#.......................#............
// #########.............#.......................#............
// ......................#.......................#............
// ......................#.......................#............
// ......................#.......................#............
// ......................#.......................#............
// ......................#.......................#............
// ......................#############...........#............
// ..................................#...........#............
// ..................................#...........#............
// ..................................#...........#............
// ..................................#############............
// Segments:
// L,12,R,4,R,4,R,12,R,4,L,12,R,12,R,4,L,12,R,12,R,4,L,6,L,8,L,8,R,12,R,4,L,6,L,8,L,8,L,12,R,4,R,4,L,12,R,4,R,4,R,12,R,4,L,12,R,12,R,4,L,12,R,12,R,4,L,6,L,8,L,8
//  A    B   B   C   B    A    C    B   A    C    B   D   E   E   C   B    D   E   E    A   B   B    A   B   B   C    B   A    C    B   A    C    B   D   E   E
// Condensed
// ABBCBACBACBDEECBDEEABBABBCBACBCCBDEE

// X = ABB
// Y = CBA
// Z = CBDEE

// Final = XYYZZXXYYZ (ABBCCAABBC)

func part2(input string) int {
	image := make([][]string, 0)
	image = append(image, []string{})

	vm := &VM.VM{}
	memory := VM.MemoryFromProgram(input)
	memory[0] = 2

	in := make(chan int)
	defer close(in)
	
	result := 0
	vm.Init(memory, VM.WithChannelInput(in), VM.WithOutputFunction(func(val int) {
		result = val
	}))
	wg := vm.RunAsync()

	movementRoutine := "A,B,B,C,C,A,A,B,B,C\n"
	functionA := "L,12,R,4,R,4\n"
	functionB := "R,12,R,4,L,12\n"
	functionC := "R,12,R,4,L,6,L,8,L,8\n"
	runes := []rune(movementRoutine + functionA + functionB + functionC + "n\n")
	for _, r := range runes {
		in <- int(r)
	}

	wg.Wait()
	return result
}

func main() {
	content, _ := os.ReadFile("day17/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 17")
}
