defmodule LocationFinder do
  @moduledoc "Day 1"

  def parse_input(input) do
    numbers = input |> String.split("\n", trim: true) |>
      Enum.map(fn i -> i |> String.split("   ", trim: true) end) |>
      Enum.map(fn [a, b] -> [String.to_integer(a), String.to_integer(b)] end) |>
      Enum.map(&List.to_tuple/1)

    {l1, l2} = Enum.unzip(numbers)
    l1 = Enum.sort(l1)
    l2 = Enum.sort(l2)

    {l1, l2}
  end

  def part1(input) do
    {l1, l2} = parse_input(input)
    result = Enum.zip(l1, l2) |>
    Enum.reduce(0, fn {a, b}, acc -> abs(a - b) + acc end)

    result
  end

  @spec part2(binary()) :: number()
  def part2(input) do
    {l1, l2} = parse_input(input)
    frequencies = Enum.frequencies(l2)
    result = l1 |>
    Enum.map(fn item ->
      item * Map.get(frequencies, item, 0)
    end) |>
    Enum.sum

    result
  end
end

# TODO: Figure out better way of reading...
{:ok, contents} = File.read("lib/day1/input.txt")
IO.puts(LocationFinder.part1(contents))
IO.puts(LocationFinder.part2(contents))

# TODO: Want something better than this if I stay with Elixir
Benchee.run(%{day1_part1: fn -> contents |> LocationFinder.part1() end,
             day2_part2: fn -> contents |> LocationFinder.part2() end}, warmup: 2,
time: 3)
