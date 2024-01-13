package main

import (
	aoc "2019"
	vec "2019/math"
	array "2019/util"
	"os"
	"strconv"
	"strings"
)

type Moon struct {
	pos          vec.Vec3I
	vel          vec.Vec3I
	gravityWells []*Moon
}

func parseMoons(input []string) []*Moon {
	moons := make([]*Moon, len(input))
	for i, line := range input {
		coords := strings.Split(line, ", ")
		x, _ := strconv.Atoi(strings.Trim(coords[0], "<>xyz="))
		y, _ := strconv.Atoi(strings.Trim(coords[1], "<>xyz="))
		z, _ := strconv.Atoi(strings.Trim(coords[2], "<>xyz="))
		moons[i] = &Moon{
			pos:          vec.Vec3I{X: x, Y: y, Z: z},
			vel:          vec.Vec3I{},
			gravityWells: make([]*Moon, 0),
		}
	}

	for _, pair := range array.GeneratePairs(moons) {
		a := pair[0]
		b := pair[1]

		a.gravityWells = append(a.gravityWells, b)
		b.gravityWells = append(b.gravityWells, a)
	}

	return moons
}

func step(moons []*Moon) {
	for _, moon := range moons {
		for _, well := range moon.gravityWells {
			if moon.pos.X < well.pos.X {
				moon.vel.X++
			} else if moon.pos.X > well.pos.X {
				moon.vel.X--
			}
			if moon.pos.Y < well.pos.Y {
				moon.vel.Y++
			} else if moon.pos.Y > well.pos.Y {
				moon.vel.Y--
			}
			if moon.pos.Z < well.pos.Z {
				moon.vel.Z++
			} else if moon.pos.Z > well.pos.Z {
				moon.vel.Z--
			}
		}
	}

	for _, moon := range moons {
		moon.pos.X += moon.vel.X
		moon.pos.Y += moon.vel.Y
		moon.pos.Z += moon.vel.Z
	}
}

func vecEnergy(v *vec.Vec3I) int {
	return vec.Abs(v.X) + vec.Abs(v.Y) + vec.Abs(v.Z)
}

func (moon *Moon) energy() int {
	pot := vecEnergy(&moon.pos)
	kin := vecEnergy(&moon.vel)

	return pot * kin
}

func getSystemEnergy(moons []*Moon) int {
	total := 0
	for _, moon := range moons {
		total += moon.energy()
	}

	return total
}

func part1(input []string, steps int) int {
	moons := parseMoons(input)

	for i := 0; i < steps; i++ {
		step(moons)
		//if i%10 == 0 {
		//fmt.Printf("After %d Steps \n", i+1)
		//for _, moon := range moons {
		//	fmt.Printf("Position: %v, Velocity: %v\n", moon.pos, moon.vel)
		//}
		//fmt.Println()
		//}
	}

	return getSystemEnergy(moons)
}

func findStepsOnAxis(moons []*Moon, cmpFn func(originalVels []vec.Vec3I, current []*Moon) bool) int {
	originalVels := make([]vec.Vec3I, len(moons))

	for i := range moons {
		originalVels[i] = moons[i].vel
	}

	numSteps := 0
	for {
		step(moons)
		numSteps++

		if cmpFn(originalVels, moons) {
			// takes double the
			return numSteps * 2
		}
	}
}

func part2(input []string) int {
	moons := parseMoons(input)
	xStep := findStepsOnAxis(moons, func(originalVels []vec.Vec3I, current []*Moon) bool {
		for i := range originalVels {
			if originalVels[i].X != moons[i].vel.X {
				return false
			}
		}

		return true
	})

	moons = parseMoons(input)
	yStep := findStepsOnAxis(moons, func(originalVels []vec.Vec3I, current []*Moon) bool {
		for i := range originalVels {
			if originalVels[i].Y != moons[i].vel.Y {
				return false
			}
		}

		return true
	})

	moons = parseMoons(input)
	zStep := findStepsOnAxis(moons, func(originalVels []vec.Vec3I, current []*Moon) bool {
		for i := range originalVels {
			if originalVels[i].Z != moons[i].vel.Z {
				return false
			}
		}

		return true
	})

	a := vec.LCM(xStep, yStep)
	result := vec.LCM(a, zStep)

	return result
}

func main() {
	content, _ := os.ReadFile("day12/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input, 1000)
		b := part2(input)

		return a, b
	}, "Day 12")
}
