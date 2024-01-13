package main

import (
	aoc "2019"
	"math"
	"os"
	"strconv"
	"strings"
)

type Ingredient struct {
	amount     int
	ingredient string
}

type Reaction struct {
	inputs []Ingredient
	output Ingredient
}

func parseReactions(input []string) map[string]Reaction {
	reactions := make(map[string]Reaction, 0)
	for _, str := range input {
		reactionParts := strings.Split(str, " => ")
		ingredients := strings.Split(reactionParts[0], ", ")
		inputs := make([]Ingredient, len(ingredients))
		for i, ingredient := range ingredients {
			ingredientParts := strings.Split(ingredient, " ")
			amount, _ := strconv.Atoi(ingredientParts[0])
			inputs[i] = Ingredient{amount, ingredientParts[1]}
		}

		result := strings.Split(reactionParts[1], " ")
		amount, _ := strconv.Atoi(result[0])
		output := Ingredient{amount, result[1]}
		reactions[result[1]] = Reaction{inputs: inputs, output: output}
	}

	return reactions
}

func getNeeded(a int, b int) int {
	return int(math.Ceil(float64(a) / float64(b)))
}

func calculateOre(fuelDesired int, reactions map[string]Reaction) int {
	ingredientsNeeded := make(map[string]int)

	ingredientsNeeded["FUEL"] = -fuelDesired
	for {
		moreNeeded := false
		for ingredient, numNeeded := range ingredientsNeeded {
			if ingredient == "ORE" {
				continue
			}

			if numNeeded < 0 {
				reaction := reactions[ingredient]
				reactionMultiplier := getNeeded(-numNeeded, reaction.output.amount)
				for _, input := range reaction.inputs {
					ingredientsNeeded[input.ingredient] -= input.amount * reactionMultiplier
				}

				ingredientsNeeded[ingredient] += reaction.output.amount * reactionMultiplier
				moreNeeded = true
			}
		}

		if !moreNeeded {
			break
		}
	}

	return -ingredientsNeeded["ORE"]
}

func part1(input []string) int {
	reactions := parseReactions(input)
	return calculateOre(1, reactions)
}

func part2(input []string) int {
	reactions := parseReactions(input)

	oreAvailable := int(1e12)
	// From part 1 - I know I can make at least this much
	fuelProduced := int(oreAvailable / 387001)

	// Use a binary search-ish approach
	for {
		ore := calculateOre(fuelProduced+1, reactions)
		if ore > oreAvailable {
			return fuelProduced
		}

		fuelProduced = max(fuelProduced+1, int(math.Floor(float64((fuelProduced+1)*oreAvailable)/float64(ore))))
	}
}

// Initial stack-based attempt
//func part1(input []string) int {
//	reactions := parseReactions(input)
//	ingredientsNeeded := make(map[string]int, 0)
//	rawIngredients := datatypes.NewSet[string]()
//
//	stack := datatypes.Stack[Ingredient]{}
//	stack.Push(Ingredient{1, "FUEL"})
//
//	for !stack.IsEmpty() {
//		curIngredient, _ := stack.Pop()
//		reaction := reactions[curIngredient.ingredient]
//
//		// Determine the amount needed to produce what we need from this reaction
//		amountNeeded := getNeeded(curIngredient.amount, reaction.output.amount)
//		for _, input := range reaction.inputs {
//			//fmt.Printf("For %v %v, need %v %v\n", curIngredient.amount, curIngredient.ingredient, amountNeeded*input.amount, input.ingredient)
//			if input.ingredient == "ORE" {
//				rawIngredients.Add(curIngredient.ingredient)
//			} else {
//				ingredientsNeeded[input.ingredient] += amountNeeded * input.amount
//				stack.Push(Ingredient{amountNeeded * input.amount, input.ingredient})
//			}
//		}
//	}
//
//	totalOreAmount := 0
//	for _, ing := range rawIngredients.Values() {
//		reaction := reactions[ing]
//		needed := ingredientsNeeded[ing]
//
//		neededAmount := getNeeded(needed, reaction.output.amount) * reaction.inputs[0].amount
//		// We know this is a raw ingredient so only requires ORE
//		fmt.Printf("For %v %v, need %v ORE\n", needed, ing, neededAmount)
//		totalOreAmount += neededAmount
//	}
//
//	return totalOreAmount
//}

func main() {
	content, _ := os.ReadFile("day14/input.txt")
	input := strings.Split(string(content), "\n")

	aoc.Runner(func() (int, int) {
		a := part1(input)
		b := part2(input)

		return a, b
	}, "Day 14")
}
