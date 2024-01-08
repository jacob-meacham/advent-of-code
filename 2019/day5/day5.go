package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	lastCode := -1

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(func() int {
		return 1
	}), VM.WithOutputFunction(func(val int) {
		lastCode = val
	}))

	vm.Run()

	return lastCode
}

func part2(input string) int {
	lastCode := -1

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(func() int {
		return 5
	}), VM.WithOutputFunction(func(val int) {
		lastCode = val
	}))

	vm.Run()

	return lastCode
}

func main() {
	content, _ := os.ReadFile("day5/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 5")
}
