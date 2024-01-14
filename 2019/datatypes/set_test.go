package datatypes

import (
	"reflect"
	"testing"
)

func TestSet(t *testing.T) {
	// Test for Add method
	t.Run("Add", func(t *testing.T) {
		testCases := []struct {
			name     string
			input    int
			expected Set[int]
		}{
			{name: "Adding new element", input: 1, expected: Set[int]{map[int]bool{1: true}}},
			{name: "Adding existing element", input: 1, expected: Set[int]{map[int]bool{1: true}}},
		}

		for _, tc := range testCases {
			t.Run(tc.name, func(t *testing.T) {
				set := NewSet[int]()
				set.Add(tc.input)
				if !reflect.DeepEqual(set, tc.expected) {
					t.Errorf("got %v, want %v", set, tc.expected)
				}
			})
		}
	})

	// Test for Values method
	t.Run("Values", func(t *testing.T) {
		testCases := []struct {
			name     string
			set      Set[int]
			expected []int
		}{
			{name: "Getting values from empty set", set: NewSet[int](), expected: []int{}},
			{name: "Getting values from non-empty set", set: Set[int]{map[int]bool{1: true, 2: true}}, expected: []int{1, 2}},
		}

		for _, tc := range testCases {
			t.Run(tc.name, func(t *testing.T) {
				if got := tc.set.Values(); !reflect.DeepEqual(got, tc.expected) {
					t.Errorf("got %v, want %v", got, tc.expected)
				}
			})
		}
	})

	// Test for Exists method
	t.Run("Exists", func(t *testing.T) {
		testCases := []struct {
			name     string
			set      Set[int]
			input    int
			expected bool
		}{
			{name: "Checking existence of non-existing value", set: NewSet[int](), input: 1, expected: false},
			{name: "Checking existence of existing value", set: Set[int]{map[int]bool{1: true}}, input: 1, expected: true},
		}

		for _, tc := range testCases {
			t.Run(tc.name, func(t *testing.T) {
				if got := tc.set.Exists(tc.input); got != tc.expected {
					t.Errorf("got %v, want %v", got, tc.expected)
				}
			})
		}
	})

	// Test for Length method
	t.Run("Length", func(t *testing.T) {
		testCases := []struct {
			name     string
			set      Set[int]
			expected int
		}{
			{name: "Length of empty set", set: NewSet[int](), expected: 0},
			{name: "Length of non-empty set", set: Set[int]{map[int]bool{1: true, 2: true}}, expected: 2},
		}

		for _, tc := range testCases {
			t.Run(tc.name, func(t *testing.T) {
				if got := tc.set.Length(); got != tc.expected {
					t.Errorf("got %v, want %v", got, tc.expected)
				}
			})
		}
	})
}
