package datatypes

import "testing"

func TestStack(t *testing.T) {
	stack := Stack[int]{}

	// Check if the stack is empty at the beginning.
	if stack.Length() != 0 {
		t.Errorf("TestStack: expected len=0, got %v", stack.Length())
	}

	// Push several elements to the stack.
	items := []int{3, 1, 4, 1, 5, 9} // You can change these to suit your needs.
	for _, item := range items {
		stack.Push(item)
	}

	// Check if the stack contains correct number of elements.
	if stack.Length() != len(items) {
		t.Errorf("TestStack: expected len=%v, got %v", len(items), stack.Length())
	}

	// Pop elements from the stack and check each of them.
	for i := len(items) - 1; i >= 0; i-- {
		if top, ok := stack.Pop(); !ok || top != items[i] {
			t.Errorf("TestStack: pop failed, expected %v, got %v", items[i], top)
		}
	}

	// Make sure the stack is empty at the end.
	if stack.Length() != 0 {
		t.Errorf("TestStack: expected len=0, got %v", stack.Length())
	}
}
