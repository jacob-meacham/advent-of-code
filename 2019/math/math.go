package math

import "math"

type Number interface {
	int | float64
}

type Vec2[T Number] struct {
	X T
	Y T
}

type Vec3[T Number] struct {
	X T
	Y T
	Z T
}

func (vec *Vec2[T]) Neg() Vec2[T] {
	vec.X = -vec.X
	vec.Y = -vec.Y

	return *vec
}

func (vec *Vec2[T]) Add(other *Vec2[T]) Vec2[T] {
	return Vec2[T]{vec.X + other.X, vec.Y + other.Y}
}

func (vec *Vec2[T]) Magnitude() float64 {
	return math.Sqrt(float64(vec.X*vec.X + vec.Y*vec.Y))
}

func (vec *Vec3[T]) Magnitude() float64 {
	return math.Sqrt(float64(vec.X*vec.X + vec.Y*vec.Y + vec.Z*vec.Z))
}

type Vec2I = Vec2[int]
type Vec2F = Vec2[float64]

type Vec3I = Vec3[int]
type Vec3F = Vec3[float64]

func Abs[T Number](n T) T {
	if n < 0 {
		return -n
	}
	return n
}

func Floor[T Number](n T) T {
	return T(math.Floor(float64(n)))
}

func GCD(a int, b int) int {
	for b != 0 {
		a, b = b, a%b
	}

	return a
}

func LCM(a int, b int) int {
	return (a * b) / GCD(a, b)
}

func ManhattanDistance[T Number](a Vec2[T], b Vec2[T]) T {
	return Abs(a.X-b.X) + Abs(a.Y-b.Y)
}
