defmodule Advent.Day3.CorruptedMemoryChecker do
  use Advent.Day, no: 3
  @moduledoc "Day 3"


  def part1(input) do
    Regex.scan(~r/mul\((\d+),(\d+)\)/, input)
    |> Enum.map(fn [_, a, b] ->
      String.to_integer(a) * String.to_integer(b)
    end)
    |> Enum.sum()
  end

  def part2(input) do
    {_, total} = Regex.scan(~r/(?>mul\((\d+),(\d+)\))|(?>do\(\))|(?>don't\(\))/, input)
    |> Enum.reduce({true, 0}, fn
      ["do()"], {_, total} -> {true, total}
      ["don't()"], {_, total} -> {false, total}
      _, {false, total} -> {false, total}
      [_, a, b], {true, total} ->
        {true, total + String.to_integer(a) * String.to_integer(b)}
    end)
    total
  end

  def run_part1() do input() |> part1() end
  def run_part2() do input() |> part2() end
end
