package main

import (
	"os"
	"strings"
	"testing"
)

func TestPart1(t *testing.T) {
	content, _ := os.ReadFile("input.txt")
	input := strings.Split(string(content), "\n")

	result := part1(input)
	if result != 347 {
		t.Error("Incorrect answer")
	}
}

func TestPart2(t *testing.T) {
	content, _ := os.ReadFile("input.txt")
	input := strings.Split(string(content), "\n")

	result := part2(input, false)
	if result != 829 {
		t.Error("Incorrect answer")
	}
}
