package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	code := -1

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(func() int {
		return 1
	}), VM.WithOutputFunction(func(val int) {
		code = val
	}))

	vm.Run()

	return code
}

func part2(input string) int {
	code := -1

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(func() int {
		return 2
	}), VM.WithOutputFunction(func(val int) {
		code = val
	}))

	vm.Run()

	return code
}

func main() {
	content, _ := os.ReadFile("day9/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 9")
}
