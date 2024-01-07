package math

type Vec2 struct {
	X int
	Y int
}

func Abs(n int) int {
	if n < 0 {
		return -n
	}
	return n
}

func ManhattanDistance(a Vec2, b Vec2) int {
	return Abs(a.X-b.X) + Abs(a.Y-b.Y)
}
