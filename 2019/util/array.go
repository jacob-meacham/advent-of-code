package array

func Swap[T any](arr []T, i, j int) {
	arr[i], arr[j] = arr[j], arr[i]
}

// generate all permutations of the array like itertools.permutations in python.
func permuteInner[T any](arr []T, start int, result *[][]T) {
	if start == len(arr) {
		// Make a copy of the current state of arr and add it to the result.
		permutation := make([]T, len(arr))
		copy(permutation, arr)
		*result = append(*result, permutation)
		return
	}

	for i := start; i < len(arr); i++ {
		Swap(arr, start, i)
		permuteInner(arr, start+1, result)
		Swap(arr, start, i)
	}
}

func Permutatations[T any](arr []T) [][]T {
	var result [][]T
	permuteInner(arr, 0, &result)
	return result
}

func GeneratePairs[T any](slice []T) [][2]T {
	pairs := make([][2]T, 0)
	for i := 0; i < len(slice); i++ {
		for j := i + 1; j < len(slice); j++ {
			pairs = append(pairs, [2]T{slice[i], slice[j]})
		}
	}

	return pairs
}
