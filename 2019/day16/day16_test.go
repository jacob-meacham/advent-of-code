package main

import "testing"

func TestExample1(t *testing.T) {
	a := part1("80871224585914546619083218645595", 100)
	if a != 24176176 {
		t.Error("Incorrect answer")
	}
}

func TestExample2(t *testing.T) {
	a := part1("19617804207202209144916044189917", 100)
	if a != 73745418 {
		t.Error("Incorrect answer")
	}
}

func TestExample3(t *testing.T) {
	a := part1("69317163492948606335995924319873", 100)
	if a != 52432133 {
		t.Error("Incorrect answer")
	}
}
