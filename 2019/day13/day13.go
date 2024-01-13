package main

import (
	aoc "2019"
	"2019/math"
	VM "2019/vm"
	"bytes"
	"flag"
	"fmt"
	"github.com/gdamore/tcell"
	"os"
	"time"
)

var directions = map[uint8]math.Vec2I{
	'N': {1, 0},
	'S': {-1, 0},
	'E': {0, 1},
	'W': {0, -1},
}

type Tile int

const (
	Empty Tile = iota
	Wall
	Block
	Paddle
	Ball
)

type RenderState int

const (
	X RenderState = iota
	Y
	TileType
)

func clearFrame(frame [][]Tile) {
	for x := range frame {
		for y := range frame[x] {
			frame[x][y] = Empty
		}
	}
}

func renderFrame(frame [][]Tile, score int, s tcell.Screen) {
	for y := range frame {
		for x := range frame[y] {
			switch frame[y][x] {
			case Empty:
				s.SetContent(x, y, ' ', nil, tcell.StyleDefault.Background(tcell.ColorBlack).Foreground(tcell.ColorBlack))
			case Wall:
				s.SetContent(x, y, '|', nil, tcell.StyleDefault.Background(tcell.ColorBlack).Foreground(tcell.ColorRed))
			case Block:
				s.SetContent(x, y, '█', nil, tcell.StyleDefault.Background(tcell.ColorBlack).Foreground(tcell.ColorGold))
			case Paddle:
				s.SetContent(x, y, '█', nil, tcell.StyleDefault.Background(tcell.ColorBlack).Foreground(tcell.ColorBlue))
			case Ball:
				s.SetContent(x, y, '🟢', nil, tcell.StyleDefault.Background(tcell.ColorBlack).Foreground(tcell.ColorGreen))
			}
		}
	}

	var scoreBuf bytes.Buffer
	fmt.Fprintf(&scoreBuf, "Score: %v", score)
	scoreCol := 0
	for _, r := range scoreBuf.Bytes() {
		s.SetContent(scoreCol, 24, rune(r), nil, tcell.StyleDefault.Foreground(tcell.ColorWhite).Background(tcell.ColorPurple))
		scoreCol++
	}
	s.Show()
}

func part1(input string) int {
	rows := 23
	cols := 45
	frame := make([][]Tile, rows)
	for i := range frame {
		frame[i] = make([]Tile, cols)
	}

	s, _ := tcell.NewScreen()

	outputFn := VM.CurryOutput(3, func(vals ...int) {
		frame[vals[1]][vals[0]] = Tile(vals[2])
	})

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithOutputFunction(outputFn))

	vm.Run()

	renderFrame(frame, 0, s)

	numBlocks := 0
	for x := range frame {
		for y := range frame[x] {
			if frame[x][y] == Block {
				numBlocks++
			}
		}
	}

	return numBlocks
}

func part2(input string, shouldRender bool) int {
	rows := 23
	cols := 45
	frame := make([][]Tile, rows)
	for i := range frame {
		frame[i] = make([]Tile, cols)
	}

	s, _ := tcell.NewScreen()
	if shouldRender {
		s.Init()
		s.SetStyle(tcell.StyleDefault.Background(tcell.ColorBlack).Foreground(tcell.ColorBlack))
		s.Clear()
	}

	curScore := 0
	paddleX := 0
	ballX := 0

	outputFn := VM.CurryOutput(3, func(vals ...int) {
		if vals[0] == -1 && vals[1] == 0 {
			curScore = vals[2]
		} else {
			tile := Tile(vals[2])
			frame[vals[1]][vals[0]] = tile
			if tile == Paddle {
				paddleX = vals[0]
			} else if tile == Ball {
				ballX = vals[0]
			}
		}
	})

	inputFn := func() int {
		if shouldRender {
			renderFrame(frame, curScore, s)
			time.Sleep(20 * time.Millisecond)
		}

		if paddleX < ballX {
			return 1
		} else if paddleX > ballX {
			return -1
		} else {
			return 0
		}
	}

	vm := &VM.VM{}
	vm.Init(VM.MemoryFromProgram(input), VM.WithInputFunction(inputFn), VM.WithOutputFunction(outputFn))
	vm.Memory[0] = 2

	vm.Run()

	return curScore
}

func main() {
	content, _ := os.ReadFile("day13/input.txt")

	var shouldRender *bool
	flagParser := func(next func()) {
		shouldRender = flag.Bool("render", false, "Should Render")
		next()
	}

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content), *shouldRender)

		return a, b
	}, "Day 13", aoc.WithFlagParser(flagParser))
}
