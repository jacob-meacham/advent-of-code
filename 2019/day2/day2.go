package main

import (
	aoc "2019"
	"os"
	"strconv"
	"strings"
)

type Opcode int

const (
	_ Opcode = iota
	OpAdd
	OpMul
	Halt = 99
)

type VM struct {
	memory []int
}

func (vm *VM) Init(vals []int) {
	vm.memory = make([]int, len(vals))
	copy(vm.memory, vals)
}

func (vm *VM) Run() {
	ip := 0
program:
	for {
		opcode := Opcode(vm.memory[ip])
		switch opcode {
		case OpAdd:
			vm.memory[vm.memory[ip+3]] = vm.memory[vm.memory[ip+1]] + vm.memory[vm.memory[ip+2]]
			ip += 4
		case OpMul:
			vm.memory[vm.memory[ip+3]] = vm.memory[vm.memory[ip+1]] * vm.memory[vm.memory[ip+2]]
			ip += 4
		case Halt:
			break program
		}
	}
}

func part1(input []string) int {
	memory := make([]int, len(input))
	for i, line := range input {
		num, _ := strconv.Atoi(line)
		memory[i] = num
	}

	vm := &VM{}
	vm.Init(memory)
	vm.memory[1] = 12
	vm.memory[2] = 2

	vm.Run()

	return vm.memory[0]
}

func part2(input []string) int {
	initialMemory := make([]int, len(input))
	for i, line := range input {
		num, _ := strconv.Atoi(line)
		initialMemory[i] = num
	}

	for a := 0; a < 100; a++ {
		for b := 0; b < 100; b++ {
			vm := &VM{}
			vm.Init(initialMemory)
			vm.memory[1] = a
			vm.memory[2] = b

			vm.Run()
			if vm.memory[0] == 19690720 {
				return 100*a + b
			}
		}
	}

	return -1
}

func main() {
	content, _ := os.ReadFile("day2/input.txt")
	input := strings.Split(string(content), ",")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 2")
}
