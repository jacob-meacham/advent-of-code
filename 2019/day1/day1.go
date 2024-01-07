package main

import (
	aoc "2019"
	"os"
	"strconv"
	"strings"
)

func part2(input []string) int {
	masses := make([]int, len(input))
	for i, line := range input {
		num, _ := strconv.Atoi(line)
		masses[i] = num
	}

	totalFuel := 0
	for _, mass := range masses {
		fuel := mass/3 - 2
		for fuel > 0 {
			totalFuel += fuel
			fuel = fuel/3 - 2
		}
	}
	return totalFuel
}

func part1(input []string) int {
	total := 0
	for _, line := range input {
		num, _ := strconv.Atoi(line)
		total += num/3 - 2
	}

	return total
}

func main() {
	content, _ := os.ReadFile("day1/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 1")
}
