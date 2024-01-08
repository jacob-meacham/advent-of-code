package main

import (
	aoc "2019"
	"os"
	"strings"
	"time"
)

func part1(input []string) int {
	time.Sleep(51 * time.Millisecond)
	return -1
}

func part2(input []string) int {
	time.Sleep(51 * time.Millisecond)
	return -1
}

func main() {
	content, _ := os.ReadFile("day6/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 6")
}
