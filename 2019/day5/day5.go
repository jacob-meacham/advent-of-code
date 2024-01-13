package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	in := make(chan int)
	defer close(in)
	lastCode := -1

	program := VM.MemoryFromProgram(input)
	vm := &VM.VM{}
	vm.Init(program,
		VM.WithChannelInput(in),
		VM.WithOutputFunction(func(val int) {
			lastCode = val
		}), VM.WithTotalMemory(len(program)))
	go vm.Run()
	in <- 1

	return lastCode
}

func part2(input string) int {
	in := make(chan int)
	out := make(chan int)
	defer close(in)

	program := VM.MemoryFromProgram(input)
	vm := &VM.VM{}
	vm.Init(program,
		VM.WithChannelInput(in),
		VM.WithChannelOut(out), VM.WithTotalMemory(len(program)))
	go vm.Run()
	in <- 5

	return <-out
}

func main() {
	content, _ := os.ReadFile("day5/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 5")
}
