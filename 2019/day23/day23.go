package main

import (
	aoc "2019"
	VM "2019/vm"
	"os"
	"time"
)

type NetworkPacket struct {
	address int
	values  []int
}

type NetworkedComputer struct {
	vm             *VM.VM
	address        int
	ReceiveChannel chan NetworkPacket
	SendChannel    chan NetworkPacket
	IsIdle         bool
}

func (computer *NetworkedComputer) Run() {
	receiveQueue := make([]int, 10)

	// I need to do this here instead of sending in the main thread; otherwise we have trouble
	// Because the computers initialized too slowly
	receiveQueue[0] = computer.address
	idleCounter := 0
	inputFn := func() int {
		if len(receiveQueue) > 0 {
			val := receiveQueue[0]
			receiveQueue = receiveQueue[1:]
			return val
		}

		select {
		case msg := <-computer.ReceiveChannel:
			idleCounter = 0
			computer.IsIdle = false
			for _, v := range msg.values[1:] {
				receiveQueue = append(receiveQueue, v)
			}
			return msg.values[0]
		case <-time.After(2 * time.Millisecond):
			idleCounter++
			if idleCounter > 1 {
				computer.IsIdle = true
			}
			return -1
		}
	}

	outputFn := VM.CurryOutput(3, func(vals ...int) {
		idleCounter = 0
		computer.IsIdle = false
		packet := NetworkPacket{
			address: vals[0],
			values:  []int{vals[1], vals[2]},
		}

		computer.SendChannel <- packet
	})

	// Run this on the computer thread, instead of using another goroutine and additional channels
	computer.vm.Run(VM.WithInputFunction(inputFn), VM.WithOutputFunction(outputFn))
}

func initializeNetwork(input string) map[int]*NetworkedComputer {
	memory := VM.MemoryFromProgram(input)
	network := make(map[int]*NetworkedComputer)

	for address := 0; address < 50; address++ {
		vm := &VM.VM{}
		vm.Init(memory, VM.WithTotalMemory(4096))
		computer := NetworkedComputer{
			vm:             vm,
			address:        address,
			ReceiveChannel: make(chan NetworkPacket, 100),
			SendChannel:    make(chan NetworkPacket, 100),
		}
		network[address] = &computer
		go computer.Run()
	}

	return network
}

func part1(input string) int {
	network := initializeNetwork(input)
	for {
		for _, computer := range network {
			select {
			case msg := <-computer.SendChannel:
				sendTo := msg.address
				if sendTo == 255 {
					return msg.values[1]
				}

				network[sendTo].ReceiveChannel <- msg
			default:
			}
		}
	}
}

func part2(input string) int {
	network := initializeNetwork(input)
	curNatPacket := NetworkPacket{address: 0}
	lastYDelivered := -1

	for {
		allIdle := true
		for _, computer := range network {
			select {
			case msg := <-computer.SendChannel:
				sendTo := msg.address
				if sendTo == 255 {
					curNatPacket.values = msg.values
				} else {
					network[sendTo].ReceiveChannel <- msg
				}
			default:
			}

			if !computer.IsIdle {
				allIdle = false
			}
		}

		if allIdle {
			if lastYDelivered == curNatPacket.values[1] {
				return lastYDelivered
			}
			lastYDelivered = curNatPacket.values[1]
			network[0].ReceiveChannel <- curNatPacket
			network[0].IsIdle = false
		}
	}
}

func main() {
	content, _ := os.ReadFile("day23/input.txt")

	aoc.Runner(func() (int, int) {
		a := part1(string(content))
		b := part2(string(content))

		return a, b
	}, "Day 19")
}
