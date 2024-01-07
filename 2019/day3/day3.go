package main

import (
	aoc "2019"
	"2019/math"
	"os"
	"strconv"
	"strings"
)

var directions = map[uint8]math.Vec2{
	'U': {1, 0},
	'D': {-1, 0},
	'L': {0, -1},
	'R': {0, 1},
}

type wireStep struct {
	pos  math.Vec2
	step int
}

func wireCoordinates(wire string, c chan wireStep) {
	curPos := math.Vec2{}
	step := 0
	instructions := strings.Split(wire, ",")
	for _, instruction := range instructions {
		num, _ := strconv.Atoi(instruction[1:])
		dir := directions[instruction[0]]
		for n := 0; n < num; n++ {
			curPos.X += dir.X
			curPos.Y += dir.Y
			step += 1

			c <- wireStep{pos: curPos, step: step}
		}
	}

	close(c)
}

func part1(input []string) int {
	origin := math.Vec2{}

	set := make(map[math.Vec2]bool)
	var intersections []int

	wire1 := make(chan wireStep)
	go wireCoordinates(input[0], wire1)
	for ws := range wire1 {
		set[ws.pos] = true
	}

	wire2 := make(chan wireStep)
	go wireCoordinates(input[1], wire2)
	for ws := range wire2 {
		if set[ws.pos] {
			intersections = append(intersections, math.ManhattanDistance(ws.pos, origin))
		}
	}

	minDistance := intersections[0]
	for _, distance := range intersections {
		if distance < minDistance {
			minDistance = distance
		}
	}

	return minDistance
}

func part2(input []string) int {
	set := make(map[math.Vec2]int)
	var intersections []int

	wire1 := make(chan wireStep)
	go wireCoordinates(input[0], wire1)
	for ws := range wire1 {
		set[ws.pos] = ws.step
	}

	wire2 := make(chan wireStep)
	go wireCoordinates(input[1], wire2)
	for ws := range wire2 {
		step, exists := set[ws.pos]
		if exists {
			intersections = append(intersections, step+ws.step)
		}
	}

	minStep := intersections[0]
	for _, step := range intersections {
		if step < minStep {
			minStep = step
		}
	}

	return minStep
}

func main() {
	content, _ := os.ReadFile("day3/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 3")
}
