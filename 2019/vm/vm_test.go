package VM

import (
	"reflect"
	"testing"
)

func TestAdd(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{1, 0, 0, 0, 99})
	vm.Run()

	if vm.Memory[0] != 2 {
		t.Errorf("Expected 2, got %v", vm.Memory[0])
	}
}

func TestMult(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{2, 4, 4, 5, 99, 0})
	vm.Run()

	if vm.Memory[5] != 9801 {
		t.Errorf("Expected 9801, got %v", vm.Memory[5])
	}
}

func TestSmokeTest(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{1, 1, 1, 4, 99, 5, 6, 0, 99})
	vm.Run()

	if !reflect.DeepEqual(vm.Memory, []int{30, 1, 1, 4, 2, 5, 6, 0, 99}) {
		t.Error("Memory incorrect")
	}
}

func TestOpcodeParse(t *testing.T) {
	paramModes := make([]Mode, 5)
	opcode := opcodeAndParams(1002, paramModes)
	if opcode != OpMul {
		t.Errorf("Expected Mul, got %v", opcode)
	}

	opcode = opcodeAndParams(1111102, paramModes)
	if opcode != OpMul {
		t.Errorf("Expected Mul, got %v", opcode)
	}

	if !reflect.DeepEqual(paramModes, []Mode{Immediate, Immediate, Immediate, Immediate, Immediate}) {
		t.Error("Modes incorrect")
	}

	opcode = opcodeAndParams(1, paramModes)
	if opcode != OpAdd {
		t.Errorf("Expected Add (legacy), got %v", opcode)
	}

	if !reflect.DeepEqual(paramModes, []Mode{Position, Position, Position, Position, Position}) {
		t.Error("Modes incorrect")
	}

	opcode = opcodeAndParams(99, paramModes)
	if opcode != OpHalt {
		t.Errorf("Expected Halt, got %v", opcode)
	}

	if !reflect.DeepEqual(paramModes, []Mode{Position, Position, Position, Position, Position}) {
		t.Error("Modes incorrect")
	}
}

func TestMakeOpcode(t *testing.T) {
	opcode := makeOpcode(OpAdd, []Mode{Immediate, Position, Immediate})
	if opcode != 10101 {
		t.Errorf("Expected 10101, got %v", opcode)
	}
	opcode = makeOpcode(OpAdd, []Mode{Position, Position, Position})

	if opcode != 1 {
		t.Errorf("Expected 1, got %v", opcode)
	}
	opcode = makeOpcode(OpAdd, []Mode{Immediate, Immediate, Immediate})

	if opcode != 11101 {
		t.Errorf("Expected 11101, got %v", opcode)
	}
}

func TestGetParameterValue(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{1, 0, 2, 3, 99})

	val := vm.getParameterValue(Position, 1, 0)
	if val != 1 {
		t.Errorf("Expected 1, got %v", val)
	}

	val = vm.getParameterValue(Immediate, 1, 0)
	if val != 0 {
		t.Errorf("Expected 0, got %v", val)
	}
}

func TestGetPointer(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{1, 3, 80, 63, 99})

	p := vm.getAddress(Position, 1, 0)
	if p != 3 {
		t.Errorf("Expected 63, got %v", p)
	}

	p = vm.getAddress(Immediate, 1, 0)
	if p != 1 {
		t.Errorf("Expected 1, got %v", p)
	}
}

func TestInputFn(t *testing.T) {
	vm := &VM{}
	inputCalled := false
	vm.Init([]int{3, 2, 2}, WithInputFunction(func() int {
		inputCalled = true
		return 99
	}))
	vm.Run()

	if !inputCalled {
		t.Error("Input not called")
	}

	if vm.Memory[2] != 99 {
		t.Errorf("Expected 99, got %v", vm.Memory[2])
	}
}

func TestDefaultInputFn(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{3, 2, 0})

	defer func() {
		if r := recover(); r == nil {
			t.Errorf("The code did not panic")
		}
	}()

	vm.Run()
}

func TestOutputFn(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{4, 2, 99}, WithOutputFunction(func(val int) {
		if val != 99 {
			t.Errorf("Expected 99, got %v", val)
		}
	}))

	vm.Run()
}

type inputOutputFixture struct {
	Values []int
	Input  int
	Output int
}

func TestInputOutputSmokeTests(t *testing.T) {
	fixtures := []inputOutputFixture{
		{[]int{3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8}, 8, 1},
		{[]int{3, 9, 8, 9, 10, 9, 4, 9, 99, -1, 8}, 100, 0},
		{[]int{3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8}, -4, 1},
		{[]int{3, 9, 7, 9, 10, 9, 4, 9, 99, -1, 8}, 8, 0},
		{[]int{3, 3, 1108, -1, 8, 3, 4, 3, 99}, 8, 1},
		{[]int{3, 3, 1108, -1, 8, 3, 4, 3, 99}, 100, 0},
		{[]int{3, 3, 1107, -1, 8, 3, 4, 3, 99}, -4, 1},
		{[]int{3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9}, 0, 0},
		{[]int{3, 12, 6, 12, 15, 1, 13, 14, 13, 4, 13, 99, -1, 0, 1, 9}, 10, 1},
		{[]int{3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1}, 0, 0},
		{[]int{3, 3, 1105, -1, 9, 1101, 0, 0, 12, 4, 12, 99, 1}, -110, 1},
		{[]int{3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31,
			1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104,
			999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99}, 5, 999},
		{[]int{3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31,
			1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104,
			999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99}, 8, 1000},
		{[]int{3, 21, 1008, 21, 8, 20, 1005, 20, 22, 107, 8, 21, 20, 1006, 20, 31,
			1106, 0, 36, 98, 0, 0, 1002, 21, 125, 20, 4, 20, 1105, 1, 46, 104,
			999, 1105, 1, 46, 1101, 1000, 1, 20, 4, 20, 1105, 1, 46, 98, 99}, 10, 1001},
	}

	for _, fixture := range fixtures {
		vm := &VM{}
		vm.Init(fixture.Values, WithInputFunction(func() int {
			return fixture.Input
		}), WithOutputFunction(func(val int) {
			if val != fixture.Output {
				t.Errorf("Expected 99, got %v", val)
			}
		}))

		vm.Run()
	}
}

func TestEquals(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{makeOpcode(OpEquals, []Mode{Immediate, Immediate}), 6, 6, 3, 99})

	vm.step()
	if vm.Memory[3] != 1 {
		t.Errorf("Expected 1, got %v", vm.Memory[3])
	}

	vm.Init([]int{makeOpcode(OpEquals, []Mode{Immediate, Immediate}), 6, 8, 3, 99})

	vm.step()
	if vm.Memory[3] != 0 {
		t.Errorf("Expected 0, got %v", vm.Memory[3])
	}
}

func TestLessThan(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{makeOpcode(OpLT, []Mode{Immediate, Immediate}), 3, 6, 3, 99})

	vm.step()
	if vm.Memory[3] != 1 {
		t.Errorf("Expected 1, got %v", vm.Memory[3])
	}

	vm.Init([]int{makeOpcode(OpLT, []Mode{Immediate, Immediate}), 8, 6, 3, 99})

	vm.step()
	if vm.Memory[3] != 0 {
		t.Errorf("Expected 1, got %v", vm.Memory[3])
	}
}

func TestJumpIfTrue(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{makeOpcode(OpJmpIfTrue, []Mode{Immediate, Immediate}), 1, 5, 99})

	vm.step()
	if vm.IP != 5 {
		t.Errorf("Expected 5, got %v", vm.IP)
	}

	vm.Init([]int{makeOpcode(OpJmpIfTrue, []Mode{Immediate, Immediate}), 0, 5, 99})

	vm.step()
	if vm.IP != 3 {
		t.Errorf("Expected 5, got %v", vm.IP)
	}
}

func TestJumpIfFalse(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{makeOpcode(OpJmpIfFalse, []Mode{Immediate, Immediate}), 0, 5, 99})

	vm.step()
	if vm.IP != 5 {
		t.Errorf("Expected 5, got %v", vm.IP)
	}

	vm.Init([]int{makeOpcode(OpJmpIfFalse, []Mode{Immediate, Immediate}), 1, 5, 99})

	vm.step()
	if vm.IP != 3 {
		t.Errorf("Expected 5, got %v", vm.IP)
	}
}
