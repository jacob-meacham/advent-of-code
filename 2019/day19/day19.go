package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
)

func check(memory []int, x int, y int) int {
	in := make(chan int)
	out := make(chan int)
	defer close(in)

	vm := &VM.VM{}
	vm.Init(memory, VM.WithChannelInput(in), VM.WithChannelOut(out))
	go vm.Run()
	in <- x
	in <- y

	return <-out
}

func part1(input string) int {
	memory := VM.MemoryFromProgram(input)

	results := make([]int, 0)

	for x := 0; x < 50; x++ {
		for y := 0; y < 50; y++ {
			results = append(results, check(memory, x, y))
		}
	}

	numAffected := 0
	for _, x := range results {
		if x == 1 {
			numAffected += 1
		}
	}

	//for x := 0; x < 50; x++ {
	//	for y := 0; y < 50; y++ {
	//		if results[x*50+y] == 1 {
	//			fmt.Print("#")
	//		} else {
	//			fmt.Print(".")
	//		}
	//	}
	//	fmt.Println()
	//}

	return numAffected
}

//#.................................................
//..................................................
//..................................................
//..................................................
//..................................................
//..#...............................................
//..................................................

//...#.............................................. 3, 1 <- (n+2), n
//...#.............................................. 3, 1 <- (n+2), n
//....#............................................. 4, 1 <- (n+3), n
//....#............................................. 4, 1 <- (n+3), n
//....##............................................ 4, 2 <- (n+3), n+1
//.....#............................................ 5, 1 <- (n+4), n

//.....##........................................... 5, 2 <- (n+3), n
//.....##........................................... 5, 2 <- (n+3), n
//......##.......................................... 6, 2 <- (n+4), n
//......##.......................................... 6, 2 <- (n+4), n
//......###......................................... 6, 3 <- (n+4), n+1
//.......##......................................... 7, 2 <- (n+5), n
//.......###........................................ 7, 3
//........##........................................ 8, 2
//........##........................................ 8, 2
//........###....................................... 8, 3
//.........##....................................... 9, 2

//.........###...................................... 9, 3
//.........###...................................... 9, 3
//..........###.....................................
//..........###.....................................
//..........####....................................
//...........###....................................

//...........####...................................
//...........####...................................
//............####..................................
//............####..................................
//............#####.................................
//.............####.................................

//.............#####................................
//.............#####................................
//..............#####...............................
//..............#####...............................
//...............#####..............................
//...............#####..............................
//...............#####..............................
//................#####.............................
//................#####.............................
//................######............................
//.................#####............................

//.................######...........................
//.................######...........................
//..................######..........................

func part2(input string) int {
	return -1
}

func main() {
	content, _ := os.ReadFile("day19/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 17")
}