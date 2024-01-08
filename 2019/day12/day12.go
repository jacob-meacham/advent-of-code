package main

import (
	aoc "2019"
	"os"
	"strings"
)

func part1(input []string) int {
	return -1
}

func part2(input []string) int {
	return -1
}

func main() {
	content, _ := os.ReadFile("day12/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 12")
}
