package datatypes

import (
	"testing"
)

func TestSet(t *testing.T) {
	mySet := NewSet[string]()

	mySet.Add("A")
	mySet.Add("B")
	mySet.Add("A")
	mySet.Add("C")

	if mySet.Length() != 3 {
		t.Errorf("Expected length 3, got %v", mySet.Length())
	}

	if !mySet.Exists("A") {
		t.Error("A did not exist in the set")
	}
}
