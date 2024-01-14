package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	content, _ := os.ReadFile("input.txt")
	input := string(content)

	result := part1(input, 25, 6)

	if result != 2520 {
		t.Error("Incorrect answer")
	}
}

func TestPart2(t *testing.T) {
	content, _ := os.ReadFile("input.txt")
	input := string(content)

	result := part2(input, 25, 6)

	if result != "LEGJY" {
		t.Error("Incorrect answer")
	}
}
