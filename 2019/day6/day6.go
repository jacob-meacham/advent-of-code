package main

import (
	aoc "2019"
	"2019/datatypes"
	"os"
	"strings"
)

type Node struct {
	Name           string
	DistanceToRoot int
	Parent         *Node
}

func buildMap(input []string) map[string][]string {
	orbitsMap := make(map[string][]string)
	for _, orbit := range input {
		orbitParts := strings.Split(orbit, ")")
		center := orbitParts[0]
		orbiting := orbitParts[1]
		orbitsMap[center] = append(orbitsMap[center], orbiting)
	}

	return orbitsMap
}

func buildTree(root Node, orbitsMap map[string][]string) map[string]Node {
	nodes := make(map[string]Node)

	stack := datatypes.Stack[Node]{}
	stack.Push(root)

	for !stack.IsEmpty() {
		curNode, _ := stack.Pop()
		for _, child := range orbitsMap[curNode.Name] {
			childNode := Node{Name: child, DistanceToRoot: curNode.DistanceToRoot + 1, Parent: &curNode}
			nodes[childNode.Name] = childNode
			stack.Push(childNode)
		}
	}

	return nodes
}

func part1(input []string) int {
	orbitsMap := buildMap(input)

	root := Node{Name: "COM", DistanceToRoot: 0}
	nodes := buildTree(root, orbitsMap)

	totalOrbiting := 0
	for _, node := range nodes {
		totalOrbiting += node.DistanceToRoot
	}

	return totalOrbiting
}

func pathToNode(start Node, dest Node) []Node {
	path := make([]Node, 0)
	curNode := start
	for curNode != dest {
		path = append(path, curNode)
		curNode = *curNode.Parent
	}
	path = append(path, dest)
	return path
}

func getMostRecentCommonAncestor(a Node, b Node, root Node) Node {
	aPath := pathToNode(a, root)
	bPath := pathToNode(b, root)

	for i := 0; i < len(aPath); i++ {
		for j := 0; j < len(bPath); j++ {
			if aPath[i] == bPath[j] {
				return aPath[i]
			}
		}
	}

	panic("No common ancestor!")
}

func part2(input []string) int {
	orbitsMap := buildMap(input)

	root := Node{Name: "COM", DistanceToRoot: 0}
	nodes := buildTree(root, orbitsMap)

	san := nodes["SAN"]
	you := nodes["YOU"]

	// Find the most recent common ancestor
	// Could build up a map instead of O(n^2) but the graphs are pretty small
	mrca := getMostRecentCommonAncestor(san, you, root)
	// Need distance to mrca from each node
	return san.DistanceToRoot - mrca.DistanceToRoot + you.DistanceToRoot - mrca.DistanceToRoot - 2
}

func main() {
	content, _ := os.ReadFile("day6/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 6")
}
