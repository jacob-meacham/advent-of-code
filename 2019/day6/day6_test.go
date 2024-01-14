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
	if result != 227612 {
		t.Error("Incorrect answer")
	}
}

func TestPart2(t *testing.T) {
	content, _ := os.ReadFile("input.txt")
	input := strings.Split(string(content), "\n")

	result := part2(input)
	if result != 454 {
		t.Error("Incorrect answer")
	}
}
