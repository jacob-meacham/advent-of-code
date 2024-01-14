package main

import (
	"os"
	"testing"
)

func TestPart1(t *testing.T) {
	content, _ := os.ReadFile("input.txt")

	result := part1(string(content), false)
	if result != 361 {
		t.Error("Incorrect answer")
	}
}

func TestPart2(t *testing.T) {
	content, _ := os.ReadFile("input.txt")

	result := part2(string(content), false)
	if result != 17590 {
		t.Error("Incorrect answer")
	}
}
