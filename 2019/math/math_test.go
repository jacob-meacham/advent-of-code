package math

import (
	"math"
	"testing"
)

func TestVec2Neg(t *testing.T) {
	tests := map[string]struct {
		input    Vec2F
		expected Vec2F
	}{
		"Test1": {Vec2F{1.0, -1.0}, Vec2F{-1.0, 1.0}},
		"Test2": {Vec2F{0.0, 0.0}, Vec2F{0.0, 0.0}},
		"Test3": {Vec2F{-1.23, 4.56}, Vec2F{1.23, -4.56}},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := tt.input.Neg(); got != tt.expected {
				t.Errorf("Neg() = %v, want %v", got, tt.expected)
			}
		})
	}
}

func TestVec2Magnitude(t *testing.T) {
	tests := map[string]struct {
		input    Vec2F
		expected float64
	}{
		"Test1": {Vec2F{3.0, 4.0}, 5.0},
		"Test2": {Vec2F{0.0, 0.0}, 0.0},
		"Test3": {Vec2F{1.0, 1.0}, math.Sqrt(2)},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := tt.input.Magnitude(); got != tt.expected {
				t.Errorf("Magnitude() = %v, want %v", got, tt.expected)
			}
		})
	}
}

func TestAbs(t *testing.T) {
	tests := map[string]struct {
		input    int
		expected int
	}{
		"Test1": {-1, 1},
		"Test2": {0, 0},
		"Test3": {1, 1},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := Abs(tt.input); got != tt.expected {
				t.Errorf("Abs() = %v, want %v", got, tt.expected)
			}
		})
	}
}

func TestFloor(t *testing.T) {
	tests := map[string]struct {
		input    float64
		expected float64
	}{
		"Test1": {1.1, 1.0},
		"Test2": {0.0, 0.0},
		"Test3": {-1.1, -2.0},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := Floor(tt.input); got != tt.expected {
				t.Errorf("Floor() = %v, want %v", got, tt.expected)
			}
		})
	}
}

func TestGCD(t *testing.T) {
	tests := map[string]struct {
		input1   int
		input2   int
		expected int
	}{
		"Test1": {12, 15, 3},
		"Test2": {17, 13, 1},
		"Test3": {0, 99, 99},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := GCD(tt.input1, tt.input2); got != tt.expected {
				t.Errorf("GCD() = %v, want %v", got, tt.expected)
			}
		})
	}
}

func TestLCM(t *testing.T) {
	tests := map[string]struct {
		input1   int
		input2   int
		expected int
	}{
		"Test1": {5, 7, 35},
		"Test2": {6, 8, 24},
		"Test3": {0, 22, 0},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := LCM(tt.input1, tt.input2); got != tt.expected {
				t.Errorf("LCM() = %v, want %v", got, tt.expected)
			}
		})
	}
}

func TestManhattanDistance(t *testing.T) {
	tests := map[string]struct {
		input1   Vec2I
		input2   Vec2I
		expected int
	}{
		"Test1": {Vec2I{1, -1}, Vec2I{3, 3}, 6},
		"Test2": {Vec2I{0, 0}, Vec2I{0, 0}, 0},
		"Test3": {Vec2I{5, 5}, Vec2I{5, 5}, 0},
	}

	for name, tt := range tests {
		t.Run(name, func(t *testing.T) {
			if got := ManhattanDistance(tt.input1, tt.input2); got != tt.expected {
				t.Errorf("ManhattanDistance() = %v, want %v", got, tt.expected)
			}
		})
	}
}