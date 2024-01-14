package main

import (
	aoc "2019"
	"2019/datatypes"
	mymath "2019/math"
	VM "2019/vm"
	"os"
)

type StatusCode int

const (
	Wall StatusCode = iota
	Empty
	Goal
)

type Node struct {
	pos   mymath.Vec2I
	steps int
	vm    *VM.VM
}

var directions = map[int]mymath.Vec2I{
	1: {0, -1},
	2: {0, 1},
	3: {-1, 0},
	4: {1, 0},
}

func explore(initialVM *VM.VM, start mymath.Vec2I, base mymath.Vec2I, stopAtGoal bool, grid [][]bool) (mymath.Vec2I, int) {
	visited := datatypes.NewSet[mymath.Vec2I]()
	queue := []Node{{pos: start, steps: 0, vm: initialVM}}
	found := false
	numSteps := 0
	var goalPos mymath.Vec2I
	for len(queue) > 0 && !found {
		node := queue[0]
		queue = queue[1:]

		if visited.Exists(node.pos) {
			continue
		}

		visited.Add(node.pos)
		for i := 1; i <= 4; i++ {
			in := make(chan int)
			out := make(chan int)

			newPos := mymath.Vec2I{node.pos.X + directions[i].X, node.pos.Y + directions[i].Y}
			steps := node.steps + 1

			newVM := node.vm.Clone()
			go newVM.RunAsync(VM.WithChannelInput(in), VM.WithChannelOut(out))
			in <- i
			status := StatusCode(<-out)

			// Hack to keep positive
			grid[newPos.X+base.X][newPos.Y+base.Y] = status != Wall
			switch status {
			case Wall:
				visited.Add(newPos)
			case Empty:
				// Push to queue
				queue = append(queue, Node{pos: newPos, steps: steps, vm: newVM})
			case Goal:
				if stopAtGoal {
					found = true
				}

				goalPos = newPos
				numSteps = steps
			}

			newVM.Halt()
			close(in)
		}
	}

	return goalPos, numSteps
}

func part1(input string) int {
	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithTotalMemory(2048))

	start := mymath.Vec2I{X: 0, Y: 0}
	grid := make([][]bool, 50)
	for i := range grid {
		grid[i] = make([]bool, 50)
	}

	_, numSteps := explore(vm, start, mymath.Vec2I{25, 25}, true, grid)

	return numSteps
}

type OxygenNode struct {
	pos     mymath.Vec2I
	minutes int
}

func part2(input string) int {
	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithTotalMemory(2048))

	start := mymath.Vec2I{X: 0, Y: 0}
	grid := make([][]bool, 50)
	for i := range grid {
		grid[i] = make([]bool, 50)
	}

	base := mymath.Vec2I{25, 25}
	start, _ = explore(vm, start, base, false, grid)

	visited := datatypes.NewSet[mymath.Vec2I]()
	queue := []OxygenNode{{pos: mymath.Vec2I{start.X + base.X, start.Y + base.Y}, minutes: 0}}
	maxMinutes := 0

	for len(queue) > 0 {
		node := queue[0]
		queue = queue[1:]

		if visited.Exists(node.pos) {
			continue
		}

		if !grid[node.pos.X][node.pos.Y] {
			continue
		}

		visited.Add(node.pos)
		if node.minutes > maxMinutes {
			maxMinutes = node.minutes
		}

		for i := 1; i <= 4; i++ {
			newPos := mymath.Vec2I{node.pos.X + directions[i].X, node.pos.Y + directions[i].Y}
			queue = append(queue, OxygenNode{
				pos:     newPos,
				minutes: node.minutes + 1,
			})
		}
	}

	return maxMinutes
}

func main() {
	content, _ := os.ReadFile("day15/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 15")
}
