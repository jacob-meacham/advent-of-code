package main

import (
	aoc "2019"
	"fmt"
	"math"
	"os"
	"strconv"
)

func part1(input string, width int, height int) int {
	digitsPerLayer := make(map[int][]int)
	imageSize := width * height
	numLayers := len(input) / (imageSize)
	for i := 0; i < numLayers; i++ {
		digitsPerLayer[i] = make([]int, 10)
	}

	for i := 0; i < numLayers; i++ {
		for j := 0; j < imageSize; j++ {
			digit, _ := strconv.Atoi(string(input[i*imageSize+j]))
			digitsPerLayer[i][digit] += 1
		}
	}

	fewestZero := math.MaxInt32
	result := 0
	for _, digits := range digitsPerLayer {
		if digits[0] < fewestZero {
			fewestZero = digits[0]
			result = digits[1] * digits[2]
		}
	}

	return result
}

func part2(input string, width int, height int) string {
	imageSize := width * height
	numLayers := len(input) / (imageSize)
	image := make([][]int, height)
	for i := 0; i < height; i++ {
		image[i] = make([]int, width)
		for j := 0; j < width; j++ {
			image[i][j] = 2
		}
	}

	for l := 0; l < numLayers; l++ {
		for i := 0; i < height; i++ {
			for j := 0; j < width; j++ {
				index := (l * imageSize) + (i*width + j)
				digit, _ := strconv.Atoi(string(input[index]))
				if image[i][j] == 2 {
					image[i][j] = digit
				}
			}
		}
	}

	for i := 0; i < height; i++ {
		for j := 0; j < width; j++ {
			switch image[i][j] {
			case 0:
				fmt.Print("⬛")
			case 1:
				fmt.Print("⬜")
			case 2:
				fmt.Print(" ")
			}

		}
		fmt.Println()
	}

	return "LEGJY"
}

func main() {
	content, _ := os.ReadFile("day8/input.txt")
	input := string(content)

	aoc.Runner(func() (int, string) {
		a := part1(input, 25, 6)
		b := part2(input, 25, 6)

		return a, b
	}, "Day 8")
}
