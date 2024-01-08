package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
	"sync"
)

func swap(arr []int, i, j int) {
	arr[i], arr[j] = arr[j], arr[i]
}

// generate all permutations of the array like itertools.permutations in python.
func permute(arr []int, start int, result *[][]int) {
	if start == len(arr) {
		// Make a copy of the current state of arr and add it to the result.
		permutation := make([]int, len(arr))
		copy(permutation, arr)
		*result = append(*result, permutation)
		return
	}

	for i := start; i < len(arr); i++ {
		swap(arr, start, i)
		permute(arr, start+1, result)
		swap(arr, start, i)
	}
}

func allPermutations(arr []int) [][]int {
	var result [][]int
	permute(arr, 0, &result)
	return result
}

func amplifier(memory []int, initialInputs []int, input chan int, output chan int, wg *sync.WaitGroup) {
	if wg != nil {
		defer wg.Done()
	}

	inputNum := 0
	vm := &VM.VM{}
	vm.Init(memory, VM.WithInputFunction(func() int {
		if inputNum < len(initialInputs) {
			inputNum += 1
			return initialInputs[inputNum-1]
		}

		for num := range input {
			return num
		}

		// Should never get here (I think?)
		return -1
	}), VM.WithOutputFunction(func(val int) {
		output <- val
	}))

	vm.Run()
}

func part1(input string) int {
	memory := VM.MemoryFromProgram(input)

	vm := &VM.VM{}

	arr := []int{0, 1, 2, 3, 4}
	permutations := allPermutations(arr)
	maxOutputSignal := 0
	for _, p := range permutations {
		curInputSignal := 0
		for _, phaseSetting := range p {
			inputNum := 0
			vm.Init(memory, VM.WithInputFunction(func() int {
				if inputNum == 0 {
					inputNum++
					return phaseSetting
				}

				return curInputSignal

			}), VM.WithOutputFunction(func(val int) {
				curInputSignal = val
			}))

			vm.Run()
		}

		if curInputSignal > maxOutputSignal {
			maxOutputSignal = curInputSignal
		}
	}

	return maxOutputSignal
}

func part2(input string) int {
	memory := VM.MemoryFromProgram(input)

	arr := []int{5, 6, 7, 8, 9}
	permutations := allPermutations(arr)
	maxOutputSignal := 0
	for _, p := range permutations {
		var wg sync.WaitGroup
		wg.Add(4) // Wait for all to signal done except the final

		channels := make([]chan int, 5)
		for i := range channels {
			channels[i] = make(chan int)
		}

		go amplifier(memory, []int{p[0], 0}, channels[4], channels[0], &wg)
		go amplifier(memory, []int{p[1]}, channels[0], channels[1], &wg)
		go amplifier(memory, []int{p[2]}, channels[1], channels[2], &wg)
		go amplifier(memory, []int{p[3]}, channels[2], channels[3], &wg)
		go amplifier(memory, []int{p[4]}, channels[3], channels[4], nil)

		wg.Wait()
		for num := range channels[4] {
			if num > maxOutputSignal {
				maxOutputSignal = num
			}

			for _, ch := range channels {
				close(ch)
			}
		}
	}

	return maxOutputSignal
}

func main() {
	content, _ := os.ReadFile("day7/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 7")
}
