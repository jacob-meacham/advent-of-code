package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input))
	vm.Memory[1] = 12
	vm.Memory[2] = 2

	vm.Run()

	return vm.Memory[0]
}

func part2(input string) int {
	initialMemory := VM.MemoryFromProgram(input)

	for a := 0; a < 100; a++ {
		for b := 0; b < 100; b++ {
			vm := &VM.VM{}
			vm.Init(initialMemory)
			vm.Memory[1] = a
			vm.Memory[2] = b

			vm.Run()
			if vm.Memory[0] == 19690720 {
				return 100*a + b
			}
		}
	}

	return -1
}

func main() {
	content, _ := os.ReadFile("day2/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 2")
}
