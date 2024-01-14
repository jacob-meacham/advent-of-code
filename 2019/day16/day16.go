package main

import (
	aoc "2019"
	mymath "2019/math"
	"math"
	"os"
	"strconv"
)

func arrayToNum(arr []int) int {
	result := arr[0]
	for i := 1; i < len(arr); i++ {
		result *= 10
		result += arr[i]
	}

	return result
}

func part1(input string, iterations int) int {
	cur := make([]int, len(input))
	next := make([]int, len(input))
	pattern := []int{0, 1, 0, -1}

	for i, c := range input {
		cur[i] = int(c - '0')
	}

	for i := 0; i < iterations; i++ {
		for j := range cur {
			sum := 0
			for k, d := range cur {
				p := pattern[((k+1)/(j+1))%len(pattern)]
				sum += d * p
			}
			next[j] = mymath.Abs(sum) % 10
		}

		copy(cur, next)
	}

	return arrayToNum(cur[:8])
}

func binomialCoefficients(max int, power int) []int {
	// Uses a running product to compute the coefficients. Found on AoC Reddit
	coeffients := make([]int, max)
	x := 1
	coeffients[0] = 1

	for n := 2; n <= max; n++ {
		x *= (n + power - 2)
		x = int(math.Floor(float64(x) / float64(n-1)))
		coeffients[n-1] = x % 10
	}

	return coeffients
}

// This relies on the insight that for offset >= len(signal)/2 the
// coefficient(i) = 0 for i < offset/2 and 1 for i >= offset/2
// which means we're just computing a running sum
func part2(input string, iterations int) int {
	nums := make([]int, len(input)*10000)
	offset, _ := strconv.Atoi(input[:7])

	for i, c := range input {
		nums[i] = int(c - '0')
	}

	for i := len(input); i < len(nums); i++ {
		nums[i] = nums[i%len(input)]
	}

	numsAtOffset := nums[offset:]

	for i := 0; i < iterations; i++ {
		numsAtOffset[len(numsAtOffset)-1] = mymath.Abs(numsAtOffset[len(numsAtOffset)-1]) % 10
		for i := len(numsAtOffset) - 2; i >= 0; i-- {
			numsAtOffset[i] = mymath.Abs(numsAtOffset[i+1]+numsAtOffset[i]) % 10
		}
	}

	return arrayToNum(numsAtOffset[:8])
}

func main() {
	content, _ := os.ReadFile("day16/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content), 100)
		b := part2(string(content), 100)

		return a, b
	}, "Day 16", aoc.WithIterations(3))
}
