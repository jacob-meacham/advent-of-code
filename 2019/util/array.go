package array

func GeneratePairs[T any](slice []T) [][2]T {
	var pairs [][2]T
	for i := 0; i < len(slice); i++ {
		for j := i + 1; j < len(slice); j++ {
			pairs = append(pairs, [2]T{slice[i], slice[j]})
		}
	}

	return pairs
}
