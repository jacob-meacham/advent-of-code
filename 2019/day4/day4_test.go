package main

import (
	"testing"
)

func TestSolution(t *testing.T) {
	a, b := solution(382345, 843167)
	if a != 460 || b != 290 {
		t.Error("Incorrect answer")
	}
}
