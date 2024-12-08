defmodule LocationFinder do
  @moduledoc "Day 1"

  @spec parse_input(binary()) :: {list(integer()), list(integer())}
  def parse_input(input) do
    input
    |> String.split("\n", trim: true)
    |> Enum.map(&String.split(&1, "   ", trim: true))
    |> Enum.map(fn [a, b] -> [String.to_integer(a), String.to_integer(b)] end)
    |> Enum.map(&List.to_tuple/1)
    |> Enum.unzip()
    |> then(fn {l1, l2} -> {Enum.sort(l1), Enum.sort(l2)} end)
  end

  @spec part2(binary()) :: integer()
  def part1(input) do
    {l1, l2} = parse_input(input)

    Enum.zip(l1, l2)
    |> Enum.reduce(0, fn {a, b}, acc -> abs(a - b) + acc end)
  end

  @spec part2(binary()) :: integer()
  def part2(input) do
    {l1, l2} = parse_input(input)
    frequencies = Enum.frequencies(l2)

    l1
    |> Enum.map(&(&1 * Map.get(frequencies, &1, 0)))
    |> Enum.sum()
  end
end

{:ok, contents} = File.read("lib/day1/input.txt")
IO.puts(LocationFinder.part1(contents))
IO.puts(LocationFinder.part2(contents))

# TODO: Want something better than this if I stay with Elixir
Benchee.run(%{day1_part1: fn -> contents |> LocationFinder.part1() end,
              day1_part2: fn -> contents |> LocationFinder.part2() end,
              day1_total: fn ->
                LocationFinder.part1(contents)
                LocationFinder.part2(contents)
              end}, warmup: 2,
time: 3)
