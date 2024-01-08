package main

import "testing"

func TestExampleInputs(t *testing.T) {
	a := part1("3,15,3,16,1002,16,10,16,1,16,15,15,4,15,99,0,0")
	if a != 43210 {
		t.Errorf("Part 1: Expected 43210 but got %v", a)
	}

	a = part1("3,23,3,24,1002,24,10,24,1002,23,-1,23,101,5,23,23,1,24,23,23,4,23,99,0,0")
	if a != 54321 {
		t.Errorf("Part 1: Expected 54321 but got %v", a)
	}

	a = part1("3,31,3,32,1002,32,10,32,1001,31,-2,31,1007,31,0,33,1002,33,7,33,1,33,31,31,1,32,31,31,4,31,99,0,0,0")
	if a != 65210 {
		t.Errorf("Part 1: Expected 65210 but got %v", a)
	}
}
