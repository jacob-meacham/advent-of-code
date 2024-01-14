package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	//curPos := math.Vec2{}
	// TODO: Create a generic stack

	vm := &VM.VM{}
	outputFn := func(val int) {
		switch val {
		case 0: // TODO Wall
		case 1:
			// TODO: Space
		case 2:
			// TODO: Goal
			vm.Halt()
		}
	}

	inputFn := func() int {
		// TODO: We'll have to fully backtrack, this is maze finding not just floodfill
		return 1
	}

	vm.Init(VM.MemoryFromProgram(input))

	//vm.Run(VM.WithInputFunction(inputFn), VM.WithOutputFunction(outputFn))

	return -1
}

func part2(input string) int {
	return -1
}

func main() {
	content, _ := os.ReadFile("day15/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 15")
}
