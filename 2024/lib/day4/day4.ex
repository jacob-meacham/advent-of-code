defmodule WordSearch do
  @moduledoc "Day 4"


  def part1(input) do
    # Parse into map that represents 2d array
    # for each X in the array
    # do a ray cast in the 8 directions to determine if any full matches
    # can be recursive
    0
  end

  def part2(input) do
    0
  end
end

{:ok, contents} = File.read("lib/day4/input.txt")
IO.puts(WordSearch.part1(contents))
IO.puts(WordSearch.part2(contents))


# Benchee.run(%{day4_part1: fn -> contents |> WordSearch.part1() end,
#             day4_part2: fn -> contents |> WordSearch.part2() end,
#              }, warmup: 2,
# time: 5)
