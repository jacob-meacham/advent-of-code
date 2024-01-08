package main

import (
	aoc "2019"
	"2019/math"
	VM "2019/vm"
	"fmt"
	"github.com/jpillora/ansi"
	"os"
)

var directions = map[uint8]math.Vec2{
	'N': {1, 0},
	'S': {-1, 0},
	'E': {0, 1},
	'W': {0, -1},
}

type RobotState int

const (
	Painting RobotState = iota
	Moving
)

func getNextDirection(robotDirection uint8, val int) uint8 {
	switch val {
	case 0:
		switch robotDirection {
		case 'N':
			return 'W'
		case 'E':
			return 'N'
		case 'S':
			return 'E'
		case 'W':
			return 'S'
		}
	case 1:
		switch robotDirection {
		case 'N':
			return 'E'
		case 'E':
			return 'S'
		case 'S':
			return 'W'
		case 'W':
			return 'N'
		}
	}

	panic("Did not find a direction!")
}

func createRobotStateMachine(painted map[math.Vec2]*Panel, curPos *math.Vec2) func(val int) {
	robotDirection := uint8('N')
	robotState := Painting

	stateMachine := func(val int) {
		switch robotState {
		case Painting:
			if painted[*curPos] == nil {
				painted[*curPos] = &Panel{}
			}

			painted[*curPos].color = val
			robotState = Moving
		case Moving:
			robotDirection = getNextDirection(robotDirection, val)
			dir := directions[robotDirection]
			curPos.X += dir.X
			curPos.Y += dir.Y
			robotState = Painting
		}
	}

	return stateMachine
}

type Panel struct {
	color int
}

func part1(input string) int {
	painted := make(map[math.Vec2]*Panel)
	curPos := math.Vec2{}

	stateMachine := createRobotStateMachine(painted, &curPos)

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(func() int {
		if painted[curPos] == nil {
			return 0
		}

		return painted[curPos].color
	}), VM.WithOutputFunction(stateMachine))

	vm.Run()

	count := 0
	for range painted {
		count += 1
	}
	return count
}

func part2(input string) int {
	painted := make(map[math.Vec2]*Panel)
	curPos := math.Vec2{}

	stateMachine := createRobotStateMachine(painted, &curPos)
	painted[math.Vec2{0, 0}] = &Panel{1}

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(func() int {
		if painted[curPos] == nil {
			return 0
		}

		return painted[curPos].color
	}), VM.WithOutputFunction(stateMachine))

	vm.Run()

	minX, minY, maxX, maxY := 10000, 10000, -10000, -10000
	for pos := range painted {
		if pos.X < minX {
			minX = pos.X
		}
		if pos.X > maxX {
			maxX = pos.X
		}
		if pos.Y < minY {
			minY = pos.Y
		}
		if pos.Y > maxY {
			maxY = pos.Y
		}
	}

	for x := maxX; x >= minX; x-- {
		for y := minY; y <= maxY; y++ {
			pos := math.Vec2{X: x, Y: y}
			panel := painted[pos]
			if panel == nil || panel.color == 0 {
				fmt.Print(" ")
			} else {
				fmt.Print(ansi.Red.String("#"))
			}
		}
		fmt.Println()
	}

	count := 0
	for range painted {
		count += 1
	}
	return 0
}

func main() {
	content, _ := os.ReadFile("day11/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 11")
}
