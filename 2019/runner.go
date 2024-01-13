package aoc

import (
	"flag"
	"fmt"
	"github.com/jpillora/ansi"
	"strings"
	"time"
)

type RunFn[T string | int, U string | int] func() (T, U)

// Just playing around with Go
type BenchmarkOptions struct {
	iterations int
	benchmark  bool
	flagParser func(next func())
}

type BenchmarkOption func(options *BenchmarkOptions)

func WithIterations(iterations int) BenchmarkOption {
	return func(options *BenchmarkOptions) {
		options.iterations = iterations
	}
}

func WithBenchmark(benchmark bool) BenchmarkOption {
	return func(options *BenchmarkOptions) {
		options.benchmark = benchmark
	}
}

func WithFlagParser(parseFn func(next func())) BenchmarkOption {
	return func(options *BenchmarkOptions) {
		options.flagParser = parseFn
	}
}

func Runner[T string | int, U string | int](fn RunFn[T, U], day string, opts ...BenchmarkOption) {
	// Uses an express middleware-like setup
	options := &BenchmarkOptions{
		iterations: 10,
		flagParser: func(next func()) {
			next()
		},
	}

	finalParser := func() {
		benchmark := flag.Bool("bench", false, "Benchmark")
		flag.Parse()

		options.benchmark = *benchmark
	}

	for _, opt := range opts {
		opt(options)
	}

	options.flagParser(finalParser)

	if options.benchmark {
		ts := time.Now()
		for i := 0; i < options.iterations; i++ {
			fn()
		}
		elapsed := time.Since(ts)
		avgMilliseconds := float64(elapsed.Milliseconds()) / float64(options.iterations)

		var good string
		if avgMilliseconds < 100 {
			good = "✅"
		} else {
			good = "❌"
		}

		timingString := fmt.Sprintf("%.5f", avgMilliseconds)
		fmt.Printf("| %s%s| %s%s|%s     |\n", day, numSpaces(day, 7), timingString, numSpaces(timingString, 12), good)
	} else {
		a, b := fn()
		fmt.Print(ansi.White.String("Part 1: "))
		fmt.Print(ansi.Green.String(ansi.Bright.String(fmt.Sprintf("%v", a))))
		fmt.Print(ansi.White.String(" Part 2: "))
		fmt.Print(ansi.Green.String(ansi.Bright.String(fmt.Sprintf("%v", b))))
		fmt.Println()
	}
}

func numSpaces(s string, totalLength int) string {
	return strings.Repeat(" ", totalLength-len(s))
}
