package datatypes

import (
	"reflect"
	"testing"
)

func TestStack(t *testing.T) {
	t.Parallel()
	t.Run("push", func(t *testing.T) {
		t.Parallel()
		testCases := map[string]struct {
			input    []int
			expected Stack[int]
		}{
			"normal_case": {
				input:    []int{1, 2, 3, 4, 5},
				expected: Stack[int]{values: []int{1, 2, 3, 4, 5}},
			},
		}
		for name, tc := range testCases {
			t.Run(name, func(t *testing.T) {
				var s Stack[int]
				for _, v := range tc.input {
					s.Push(v)
				}
				if !reflect.DeepEqual(s.values, tc.expected.values) {
					t.Errorf("unexpected stack state, want: %+v, got: %+v", tc.expected, s)
				}
			})
		}
	})
	t.Run("peek", func(t *testing.T) {
		t.Parallel()
		testCases := map[string]struct {
			stack    Stack[int]
			expected int
		}{
			"empty_case": {
				stack:    Stack[int]{},
				expected: 0,
			},
			"normal_case": {
				stack:    Stack[int]{values: []int{1, 2, 3, 4, 5}},
				expected: 5,
			},
		}
		for name, tc := range testCases {
			t.Run(name, func(t *testing.T) {
				val, _ := tc.stack.Peek()
				if val != tc.expected {
					t.Errorf("incorrect peek result, want: %+v, got: %+v", tc.expected, val)
				}
			})
		}
	})
	t.Run("pop", func(t *testing.T) {
		t.Parallel()
		testCases := map[string]struct {
			input  []int
			output []int
		}{
			"normal_case": {
				input:  []int{1, 2, 3, 4, 5},
				output: []int{5, 4, 3, 2, 1},
			},
		}
		for name, tc := range testCases {
			t.Run(name, func(t *testing.T) {
				var s Stack[int]
				for _, v := range tc.input {
					s.Push(v)
				}
				for _, want := range tc.output {
					got, _ := s.Pop()
					if got != want {
						t.Errorf("incorrect pop result, want: %+v, got: %+v", want, got)
					}
				}
			})
		}
	})
	t.Run("isEmpty", func(t *testing.T) {
		t.Parallel()
		testCases := map[string]struct {
			stack    Stack[int]
			expected bool
		}{
			"empty_case": {
				stack:    Stack[int]{},
				expected: true,
			},
			"normal_case": {
				stack:    Stack[int]{values: []int{1, 2, 3}},
				expected: false,
			},
		}
		for name, tc := range testCases {
			t.Run(name, func(t *testing.T) {
				if got := tc.stack.IsEmpty(); got != tc.expected {
					t.Errorf("incorrect IsEmpty result, want: %+v, got: %+v", tc.expected, got)
				}
			})
		}
	})
	t.Run("length", func(t *testing.T) {
		t.Parallel()
		testCases := map[string]struct {
			stack    Stack[int]
			expected int
		}{
			"empty_case": {
				stack:    Stack[int]{},
				expected: 0,
			},
			"normal_case": {
				stack:    Stack[int]{values: []int{1, 2, 3, 4, 5}},
				expected: 5,
			},
		}
		for name, tc := range testCases {
			t.Run(name, func(t *testing.T) {
				if got := tc.stack.Length(); got != tc.expected {
					t.Errorf("incorrect Length, want: %d, got: %d", tc.expected, got)
				}
			})
		}
	})
}
