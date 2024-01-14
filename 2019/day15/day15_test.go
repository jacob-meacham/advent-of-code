package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	content, _ := os.ReadFile("input.txt")

	result := part1(string(content))
	if result != 6672 {
		t.Error("Incorrect answer")
	}
}

func TestPart2(t *testing.T) {
	content, _ := os.ReadFile("input.txt")

	result := part2(string(content))
	if result != 923017 {
		t.Error("Incorrect answer")
	}
}
