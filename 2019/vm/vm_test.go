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

func TestLargeNumber(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{1102, 34915192, 34915192, 7, 4, 7, 99, 0}, WithOutputFunction(func(val int) {
		if val != 1219070632396864 {
			t.Errorf("Expected 1219070632396864, got %v", val)
		}
	}))

	vm.Run()
}

func TestLargeNumberOutput(t *testing.T) {
	vm := &VM{}
	vm.Init([]int{104, 1125899906842624, 99}, WithOutputFunction(func(val int) {
		if val != 1125899906842624 {
			t.Errorf("Expected 1125899906842624, got %v", val)
		}
	}))

	vm.Run()
}

func TestQuine(t *testing.T) {
	program := []int{109, 1, 204, -1, 1001, 100, 1, 100, 1008, 100, 16, 101, 1006, 101, 0, 99}
	var output []int
	vm := &VM{}
	vm.Init(program, WithOutputFunction(func(val int) {
		output = append(output, val)
	}))

	vm.Run()

	if !reflect.DeepEqual(program, output) {
		t.Error("Not a quine")
	}
}

func TestBoost(t *testing.T) {
	var output []int
	vm := &VM{}
	vm.Init([]int{1102, 34463338, 34463338, 63, 1007, 63, 34463338, 63, 1005, 63, 53, 1102, 1, 3, 1000, 109, 988, 209, 12, 9, 1000, 209, 6, 209, 3, 203, 0, 1008, 1000, 1, 63, 1005, 63, 65, 1008, 1000, 2, 63, 1005, 63, 904, 1008, 1000, 0, 63, 1005, 63, 58, 4, 25, 104, 0, 99, 4, 0, 104, 0, 99, 4, 17, 104, 0, 99, 0, 0, 1102, 26, 1, 1005, 1101, 0, 24, 1019, 1102, 1, 32, 1007, 1101, 0, 704, 1027, 1102, 0, 1, 1020, 1101, 0, 348, 1029, 1102, 28, 1, 1002, 1101, 34, 0, 1016, 1102, 29, 1, 1008, 1102, 1, 30, 1013, 1102, 25, 1, 1012, 1101, 0, 33, 1009, 1102, 1, 37, 1001, 1101, 31, 0, 1017, 1101, 245, 0, 1022, 1102, 39, 1, 1000, 1101, 27, 0, 1011, 1102, 770, 1, 1025, 1101, 0, 22, 1015, 1102, 1, 1, 1021, 1101, 711, 0, 1026, 1101, 20, 0, 1004, 1101, 0, 23, 1018, 1101, 242, 0, 1023, 1102, 21, 1, 1003, 1101, 38, 0, 1010, 1101, 0, 35, 1014, 1101, 0, 36, 1006, 1101, 0, 357, 1028, 1102, 1, 775, 1024, 109, -3, 2102, 1, 9, 63, 1008, 63, 36, 63, 1005, 63, 203, 4, 187, 1105, 1, 207, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 8, 21101, 40, 0, 5, 1008, 1010, 41, 63, 1005, 63, 227, 1106, 0, 233, 4, 213, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 16, 2105, 1, 2, 1105, 1, 251, 4, 239, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 1, 21107, 41, 40, -4, 1005, 1018, 271, 1001, 64, 1, 64, 1105, 1, 273, 4, 257, 1002, 64, 2, 64, 109, -18, 1207, 0, 21, 63, 1005, 63, 295, 4, 279, 1001, 64, 1, 64, 1105, 1, 295, 1002, 64, 2, 64, 109, -3, 1207, 0, 36, 63, 1005, 63, 311, 1105, 1, 317, 4, 301, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 6, 2108, 20, -3, 63, 1005, 63, 339, 4, 323, 1001, 64, 1, 64, 1106, 0, 339, 1002, 64, 2, 64, 109, 28, 2106, 0, -7, 4, 345, 1001, 64, 1, 64, 1106, 0, 357, 1002, 64, 2, 64, 109, -18, 1206, 4, 373, 1001, 64, 1, 64, 1105, 1, 375, 4, 363, 1002, 64, 2, 64, 109, -6, 2107, 31, -4, 63, 1005, 63, 397, 4, 381, 1001, 64, 1, 64, 1105, 1, 397, 1002, 64, 2, 64, 109, 1, 21102, 42, 1, -1, 1008, 1011, 39, 63, 1005, 63, 421, 1001, 64, 1, 64, 1106, 0, 423, 4, 403, 1002, 64, 2, 64, 109, -2, 2108, 26, -2, 63, 1005, 63, 439, 1106, 0, 445, 4, 429, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 6, 21102, 43, 1, -5, 1008, 1011, 43, 63, 1005, 63, 467, 4, 451, 1105, 1, 471, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 6, 21101, 44, 0, -3, 1008, 1019, 44, 63, 1005, 63, 493, 4, 477, 1105, 1, 497, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -9, 1206, 7, 511, 4, 503, 1105, 1, 515, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 14, 1205, -7, 531, 1001, 64, 1, 64, 1106, 0, 533, 4, 521, 1002, 64, 2, 64, 109, -27, 1201, 0, 0, 63, 1008, 63, 39, 63, 1005, 63, 555, 4, 539, 1105, 1, 559, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 10, 2101, 0, -5, 63, 1008, 63, 24, 63, 1005, 63, 583, 1001, 64, 1, 64, 1105, 1, 585, 4, 565, 1002, 64, 2, 64, 109, -11, 2107, 21, 5, 63, 1005, 63, 601, 1105, 1, 607, 4, 591, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 10, 1208, 0, 36, 63, 1005, 63, 627, 1001, 64, 1, 64, 1106, 0, 629, 4, 613, 1002, 64, 2, 64, 109, 15, 21108, 45, 45, -9, 1005, 1015, 647, 4, 635, 1105, 1, 651, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -19, 2101, 0, -4, 63, 1008, 63, 37, 63, 1005, 63, 677, 4, 657, 1001, 64, 1, 64, 1106, 0, 677, 1002, 64, 2, 64, 109, 22, 1205, -6, 695, 4, 683, 1001, 64, 1, 64, 1105, 1, 695, 1002, 64, 2, 64, 109, -10, 2106, 0, 10, 1001, 64, 1, 64, 1105, 1, 713, 4, 701, 1002, 64, 2, 64, 109, -9, 1201, -8, 0, 63, 1008, 63, 36, 63, 1005, 63, 733, 1105, 1, 739, 4, 719, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 7, 21107, 46, 47, 0, 1005, 1015, 757, 4, 745, 1106, 0, 761, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 14, 2105, 1, -5, 4, 767, 1105, 1, 779, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -34, 2102, 1, 6, 63, 1008, 63, 39, 63, 1005, 63, 799, 1105, 1, 805, 4, 785, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, 25, 21108, 47, 49, -4, 1005, 1016, 825, 1001, 64, 1, 64, 1106, 0, 827, 4, 811, 1002, 64, 2, 64, 109, -6, 1208, -8, 36, 63, 1005, 63, 845, 4, 833, 1106, 0, 849, 1001, 64, 1, 64, 1002, 64, 2, 64, 109, -10, 1202, 2, 1, 63, 1008, 63, 36, 63, 1005, 63, 875, 4, 855, 1001, 64, 1, 64, 1105, 1, 875, 1002, 64, 2, 64, 109, -5, 1202, 10, 1, 63, 1008, 63, 30, 63, 1005, 63, 895, 1106, 0, 901, 4, 881, 1001, 64, 1, 64, 4, 64, 99, 21101, 27, 0, 1, 21101, 0, 915, 0, 1105, 1, 922, 21201, 1, 65916, 1, 204, 1, 99, 109, 3, 1207, -2, 3, 63, 1005, 63, 964, 21201, -2, -1, 1, 21101, 942, 0, 0, 1105, 1, 922, 21201, 1, 0, -1, 21201, -2, -3, 1, 21102, 1, 957, 0, 1105, 1, 922, 22201, 1, -1, -2, 1106, 0, 968, 22102, 1, -2, -2, 109, -3, 2105, 1, 0},
		WithInputFunction(func() int {
			return 1
		}),
		WithOutputFunction(func(val int) {
			output = append(output, val)
		}))

	vm.Run()

	if len(output) > 1 {
		t.Error("Some opcodes are not functioning")
	}

}
