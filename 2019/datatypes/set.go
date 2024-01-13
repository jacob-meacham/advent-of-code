package datatypes

type Set[T comparable] struct {
	values map[T]bool
}

func (set *Set[T]) Add(value T) {
	set.values[value] = true
}

func (set *Set[T]) Exists(value T) bool {
	return set.values[value]
}

func (set *Set[T]) Length() int {
	return len(set.values)
}

func NewSet[T comparable]() Set[T] {
	return Set[T]{make(map[T]bool)}
}
