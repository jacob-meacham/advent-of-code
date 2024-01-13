package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func part1(input string) int {
	in := make(chan int)
	out := make(chan int)
	defer close(in)

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithChannelInput(in), VM.WithChannelOut(out))
	go vm.Run()

	in <- 1

	return <-out
}

func part2(input string) int {
	in := make(chan int)
	out := make(chan int)
	defer close(in)

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithChannelInput(in), VM.WithChannelOut(out))
	go vm.Run()
	in <- 2

	return <-out
}

func main() {
	content, _ := os.ReadFile("day9/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 9")
}
