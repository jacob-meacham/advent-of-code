package array

import (
	"reflect"
	"testing"
)

func TestSwap(t *testing.T) {
	arr := []int{1, 2, 3}
	want := []int{2, 1, 3}

	Swap(arr, 0, 1)

	for i, num := range arr {
		if num != want[i] {
			t.Errorf("Swap() at index %v, got: %v, want: %v", i, num, want[i])
		}
	}
}

func TestPermutations(t *testing.T) {
	testCases := []struct {
		name  string
		input []int
		want  [][]int
	}{
		{
			name:  "EmptySlice",
			input: []int{},
			want:  [][]int{{}},
		},
		{
			name:  "SingleElement",
			input: []int{1},
			want:  [][]int{{1}},
		},
		{
			name:  "TwoElements",
			input: []int{1, 2},
			want:  [][]int{{1, 2}, {2, 1}},
		},
		{
			name:  "ThreeElements",
			input: []int{1, 2, 3},
			want:  [][]int{{1, 2, 3}, {1, 3, 2}, {2, 1, 3}, {2, 3, 1}, {3, 2, 1}, {3, 1, 2}},
		},
	}

	for _, tc := range testCases {
		t.Run(tc.name, func(t *testing.T) {
			got := Permutatations(tc.input)
			if !reflect.DeepEqual(got, tc.want) {
				t.Fatalf("Permutations() got = %v, want = %v", got, tc.want)
			}
		})
	}
}

func TestGeneratePairs(t *testing.T) {
	tests := []struct {
		name  string
		slice []int
		want  [][2]int
	}{
		{
			name:  "EmptySlice",
			slice: []int{},
			want:  [][2]int{},
		},
		{
			name:  "SingleElementSlice",
			slice: []int{1},
			want:  [][2]int{},
		},
		{
			name:  "TwoElementSlice",
			slice: []int{1, 2},
			want:  [][2]int{{1, 2}},
		},
		{
			name:  "ThreeElementSlice",
			slice: []int{1, 2, 3},
			want:  [][2]int{{1, 2}, {1, 3}, {2, 3}},
		},
		{
			name:  "FourElementSlice",
			slice: []int{1, 2, 3, 4},
			want:  [][2]int{{1, 2}, {1, 3}, {1, 4}, {2, 3}, {2, 4}, {3, 4}},
		},
	}

	for _, tt := range tests {
		t.Run(tt.name, func(t *testing.T) {
			if got := GeneratePairs(tt.slice); !reflect.DeepEqual(got, tt.want) {
				t.Errorf("GeneratePairs() = %v, want %v", got, tt.want)
			}
		})
	}
}
