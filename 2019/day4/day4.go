package main

import (
	aoc "2019"
)

func getDigitAtPos(n, pos int) int {
	// Divide by 10^pos to move the desired digit to the rightmost position
	for i := 0; i < 5-pos; i++ {
		n /= 10
	}
	// Then return that digit
	return n % 10
}

func isValidPassword(num int) (bool, int) {
	inGroup := false
	groupPos := 0
	smallestFoundGroup := 6
	foundGroup := false
	neverDecrease := true

	for j := 0; j < 5; j++ {
		d0 := getDigitAtPos(num, j)
		d1 := getDigitAtPos(num, j+1)
		if d0 == d1 {
			if !inGroup {
				foundGroup = true
				inGroup = true
				groupPos = j
			}
		} else {
			if inGroup {
				inGroup = false
				groupSize := j - groupPos + 1
				if groupSize < smallestFoundGroup {
					smallestFoundGroup = groupSize
				}
			}
		}

		if d0 > d1 {
			neverDecrease = false
			break
		}
	}

	// Handle the case where the last digits were a group
	if inGroup {
		inGroup = false
		groupSize := 5 - groupPos + 1
		if groupSize < smallestFoundGroup {
			smallestFoundGroup = groupSize
		}
	}

	return foundGroup && neverDecrease, smallestFoundGroup
}

// You could do something more clever because there are a lot of numbers
// you don't need to test for - eg for any 0, you don't need to test any other digits
// to the right of that. However, this is already quite speedy.
func solution(low, high int) (int, int) {
	validPasswordsPart1 := 0
	validPasswordsPart2 := 0
	for num := low; num <= high; num++ {
		valid, smallestGroup := isValidPassword(num)
		if valid {
			validPasswordsPart1++
			if smallestGroup == 2 {
				validPasswordsPart2++
			}
		}
	}
	return validPasswordsPart1, validPasswordsPart2
}

func main() {
	aoc.Runner(func() (int, int) {
		a, b := solution(382345, 843167)

		return a, b
	}, "Day 3")
}
