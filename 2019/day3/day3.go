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

type intersectionFunc func(math.Vec2, int)

func wireCoordinates(wire string, set map[math.Vec2]int, store bool, fn intersectionFunc) {
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

			storedStep, exists := set[curPos]
			if exists {
				fn(curPos, storedStep+step)
			} else if store {
				set[curPos] = step
			}
		}
	}
}

func part1(input []string) int {
	origin := math.Vec2{}

	set := make(map[math.Vec2]int)
	var intersections []int

	wireCoordinates(input[0], set, true, func(pos math.Vec2, step int) {})
	wireCoordinates(input[1], set, false, func(pos math.Vec2, step int) {
		intersections = append(intersections, math.ManhattanDistance(pos, origin))
	})

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

	wireCoordinates(input[0], set, true, func(pos math.Vec2, step int) {})
	wireCoordinates(input[1], set, false, func(pos math.Vec2, step int) {
		intersections = append(intersections, step)
	})

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
