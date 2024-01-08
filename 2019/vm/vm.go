package VM

import (
	"fmt"
	"math"
	"strconv"
	"strings"
)

type Opcode int

const (
	_ Opcode = iota
	OpAdd
	OpMul
	OpInput
	OpOutput
	OpJmpIfTrue
	OpJmpIfFalse
	OpLT
	OpEquals
	OpRelativeBase
	OpHalt = 99
)

type Mode int

const (
	Position Mode = iota
	Immediate
	Relative
)

type InputFn func() int
type OutputFn func(int)

type VM struct {
	Memory       []int
	IP           int
	RelativeBase int
	paramModes   []Mode // Scratch memory
	inputFn      InputFn
	outputFn     OutputFn
}

type VMOptions struct {
	inputFn     InputFn
	outputFn    OutputFn
	totalMemory int
}

type VMOption func(options *VMOptions)

func WithInputFunction(inputFn InputFn) VMOption {
	return func(options *VMOptions) {
		options.inputFn = inputFn
	}
}

func WithTotalMemory(totalMemory int) VMOption {
	return func(options *VMOptions) {
		options.totalMemory = totalMemory
	}
}

func WithOutputFunction(outputFn OutputFn) VMOption {
	return func(options *VMOptions) {
		options.outputFn = outputFn
	}
}

func DefaultInputFn() int {
	panic("No input function defined!")
}

func DefaultOutputFn(val int) {
	fmt.Println(val)
}

func MemoryFromProgram(program string) []int {
	input := strings.Split(program, ",")

	memory := make([]int, len(input))
	for i, line := range input {
		num, _ := strconv.Atoi(line)
		memory[i] = num
	}

	return memory
}

type CurriedOutputFn func(vals ...int)

func CurryOutput(numOutputs int, fn CurriedOutputFn) func(val int) {
	curOutput := 0
	var outputs []int
	return func(val int) {
		outputs = append(outputs, val)
		curOutput++
		if curOutput >= numOutputs {
			fn(outputs...)
			curOutput = 0
			outputs = []int{}
		}
	}
}

func (vm *VM) Init(vals []int, opts ...VMOption) {
	options := &VMOptions{
		inputFn:     DefaultInputFn,
		outputFn:    DefaultOutputFn,
		totalMemory: 65536,
	}

	for _, opt := range opts {
		opt(options)
	}

	vm.Memory = make([]int, options.totalMemory)
	copy(vm.Memory, vals)

	vm.IP = 0
	vm.RelativeBase = 0
	vm.paramModes = make([]Mode, 5)

	vm.inputFn = options.inputFn
	vm.outputFn = options.outputFn
}

func (vm *VM) getParameterValue(mode Mode, pos int, ip int) int {
	return vm.Memory[vm.getAddress(mode, pos, ip)]
}

func (vm *VM) getAddress(mode Mode, pos int, ip int) int {
	if mode == Immediate {
		return ip + pos
	} else if mode == Relative {
		return vm.RelativeBase + vm.Memory[ip+pos]
	}

	// Position is default
	return vm.Memory[ip+pos]
}

func makeOpcode(opcode Opcode, paramModes []Mode) int {
	result := 0

	for i := range paramModes {
		result += int(math.Pow10(i+2)) * int(paramModes[i])
	}

	return result + int(opcode)
}

func opcodeAndParams(fullOpcode int, paramModes []Mode) Opcode {
	for i := range paramModes {
		paramModes[i] = Mode((fullOpcode / int(math.Pow10(i+2))) % 10)
	}

	return Opcode(fullOpcode % 100)
}

// TODO: Better way of managing the parameters here? Maybe suck them up into an array as a first step?
func (vm *VM) step() bool {
	opcode := opcodeAndParams(vm.Memory[vm.IP], vm.paramModes)
	switch opcode {
	case OpAdd:
		p0 := vm.getParameterValue(vm.paramModes[0], 1, vm.IP)
		p1 := vm.getParameterValue(vm.paramModes[1], 2, vm.IP)
		resultAddress := vm.getAddress(vm.paramModes[2], 3, vm.IP)
		vm.Memory[resultAddress] = p0 + p1
		vm.IP += 4
	case OpMul:
		p0 := vm.getParameterValue(vm.paramModes[0], 1, vm.IP)
		p1 := vm.getParameterValue(vm.paramModes[1], 2, vm.IP)
		resultAddress := vm.getAddress(vm.paramModes[2], 3, vm.IP)
		vm.Memory[resultAddress] = p0 * p1
		vm.IP += 4
	case OpInput:
		val := vm.inputFn()
		resultAddress := vm.getAddress(vm.paramModes[0], 1, vm.IP)
		vm.Memory[resultAddress] = val
		vm.IP += 2
	case OpOutput:
		p0 := vm.getParameterValue(vm.paramModes[0], 1, vm.IP)
		vm.outputFn(p0)

		vm.IP += 2
	case OpJmpIfTrue, OpJmpIfFalse:
		p0 := vm.getParameterValue(vm.paramModes[0], 1, vm.IP)
		if opcode == OpJmpIfTrue && p0 != 0 {
			vm.IP = vm.getParameterValue(vm.paramModes[1], 2, vm.IP)
		} else if opcode == OpJmpIfFalse && p0 == 0 {
			vm.IP = vm.getParameterValue(vm.paramModes[1], 2, vm.IP)
		} else {
			vm.IP += 3
		}
	case OpLT, OpEquals:
		p0 := vm.getParameterValue(vm.paramModes[0], 1, vm.IP)
		p1 := vm.getParameterValue(vm.paramModes[1], 2, vm.IP)
		resultAddress := vm.getAddress(vm.paramModes[2], 3, vm.IP)

		if opcode == OpLT && p0 < p1 {
			vm.Memory[resultAddress] = 1
		} else if opcode == OpEquals && p0 == p1 {
			vm.Memory[resultAddress] = 1
		} else {
			vm.Memory[resultAddress] = 0
		}

		vm.IP += 4
	case OpRelativeBase:
		p0 := vm.getParameterValue(vm.paramModes[0], 1, vm.IP)
		vm.RelativeBase += p0

		vm.IP += 2
	case OpHalt:
		return false
	default:
		// TODO: Should return error from Run instead?
		panic("Unknown opcode!")
	}

	return true
}

func (vm *VM) Run() {
	vm.IP = 0
	vm.RelativeBase = 0
	vm.paramModes = make([]Mode, 5)

	for {
		// TODO: Should this take and return the new IP or modify the VM state?
		if !vm.step() {
			break
		}
	}
}
