package datatypes

type Stack[T any] struct {
	values []T
}

func (stack *Stack[T]) Push(value T) {
	stack.values = append(stack.values, value)
}

func (stack *Stack[T]) Peek() (T, bool) {
	var val T
	if len(stack.values) > 0 {
		return stack.values[len(stack.values)-1], true
	}

	return val, false
}

func (stack *Stack[T]) Pop() (T, bool) {
	val, hasValue := stack.Peek()
	if !hasValue {
		return val, false
	}

	stack.values = stack.values[:len(stack.values)-1]
	return val, true
}

func (stack *Stack[T]) IsEmpty() bool {
	_, hasValue := stack.Peek()
	return !hasValue
}
