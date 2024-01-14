package main

import (
	aoc "2019"
	"2019/datatypes"
	vec "2019/math"
	"2019/util"
	"flag"
	"fmt"
	"math"
	"os"
	"sort"
	"strings"
)

type Asteroid struct {
	pos      vec.Vec2I
	toOthers datatypes.Set[vec.Vec2F]
}

func debugPrint(input []string, destroyedOrder []vec.Vec2I) {
	output := make([]string, len(input))

	// Copy the elements
	copy(output, input)

	for i, asteroid := range destroyedOrder {
		var r rune
		if i < 9 {
			s := fmt.Sprintf("%v", i+1)
			r = rune(s[0])
		} else if i >= 26+9 {
			r = rune(i - 9 - 26 + 65)
		} else {
			r = rune(i - 9 + 97)
		}
		runeSlice := []rune(output[asteroid.X])
		runeSlice[asteroid.Y] = r
		output[asteroid.X] = string(runeSlice)
	}

	for x := range output {
		for y := range output[x] {
			fmt.Print(string(output[x][y]))
		}
		fmt.Println()
	}
}

func parseAsteroids(input []string) []Asteroid {
	asteroids := make([]Asteroid, 0)
	for x := range input {
		for y := range input[x] {
			if input[x][y] == '#' {
				asteroids = append(asteroids, Asteroid{pos: vec.Vec2I{x, y}, toOthers: datatypes.NewSet[vec.Vec2F]()})
			}
		}
	}

	return asteroids
}

func normalizedVec(a vec.Vec2I, b vec.Vec2I) vec.Vec2F {
	dx := a.X - b.X
	dy := a.Y - b.Y

	div := vec.GCD(dx, dy)

	return vec.Vec2F{float64(dx) / float64(div), float64(dy) / float64(div)}
}

func getBestAsteroid(asteroids []Asteroid) Asteroid {
	// Calculate the slopes and distances between every pair of points. Any slopes that are co-linear, only the closest point is visible
	for _, pair := range array.GeneratePairs(asteroids) {
		a := pair[0]
		b := pair[1]

		normalizedVec := normalizedVec(a.pos, b.pos)
		a.toOthers.Add(normalizedVec)
		b.toOthers.Add(normalizedVec.Neg())
	}

	maxVisible := 0
	var best Asteroid
	for _, asteroid := range asteroids {
		if asteroid.toOthers.Length() > maxVisible {
			best = asteroid
			maxVisible = asteroid.toOthers.Length()
		}
	}

	return best
}

func part1(input []string) int {
	asteroids := parseAsteroids(input)

	best := getBestAsteroid(asteroids)
	return best.toOthers.Length()
}

type AsteroidDifference struct {
	asteroid vec.Vec2I
	angle    float64
	distance float64
}

func part2(input []string, debug bool) int {
	asteroids := parseAsteroids(input)

	orderedAsteroids := make([]AsteroidDifference, 0)
	best := getBestAsteroid(asteroids)

	for _, asteroid := range asteroids {
		if asteroid.pos == best.pos {
			continue
		}

		diff := vec.Vec2F{float64(asteroid.pos.X - best.pos.X), float64(asteroid.pos.Y - best.pos.Y)}

		// subtract PI / 2 to go clockwise from 12 o'clock
		angle := math.Pi/2 - math.Atan2(diff.Y, diff.X)
		distance := diff.Magnitude()

		orderedAsteroids = append(orderedAsteroids, AsteroidDifference{asteroid.pos, angle, distance})
	}

	// Order first by angle, then by distance. Then in a second pass we can build up an ordered set by
	// taking only the next at any given angle
	sort.Slice(orderedAsteroids, func(i, j int) bool {
		if orderedAsteroids[i].angle == orderedAsteroids[j].angle {
			return orderedAsteroids[i].distance < orderedAsteroids[j].distance
		}
		return orderedAsteroids[i].angle < orderedAsteroids[j].angle
	})

	destroyedOrder := make([]vec.Vec2I, 0)
	i := 0
	curAngle := math.Inf(1)
	for {
		if len(orderedAsteroids) == 0 {
			break
		}

		if orderedAsteroids[i].angle == curAngle {
			// skip this asteroid until next time through
			i++
		} else {
			curAngle = orderedAsteroids[i].angle
			destroyedOrder = append(destroyedOrder, orderedAsteroids[i].asteroid)
			orderedAsteroids = append(orderedAsteroids[:i], orderedAsteroids[i+1:]...)
		}

		if i >= len(orderedAsteroids) {
			// All the way through, reset our angle
			curAngle = math.Inf(1)
			i = 0
		}
	}

	if debug {
		debugPrint(input, destroyedOrder)
	}

	target := destroyedOrder[199]
	return target.Y*100 + target.X // Flipped from usual X,Y coords
}

func main() {
	content, _ := os.ReadFile("day10/input.txt")
	input := strings.Split(string(content), "\n")

	var debug *bool
	flagParser := func(next func()) {
		debug = flag.Bool("debug", false, "Debug")
		next()
	}

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input, *debug)

		return a, b
	}, "Day 10", aoc.WithFlagParser(flagParser))
}
