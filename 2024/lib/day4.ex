defmodule Advent.Day4.WordSearch do
  use Advent.Day, no: 4
  @moduledoc "Day 4"

  @directions [
    {1, 0}, {-1, 0}, {0, 1}, {0, -1},
    {1, 1}, {-1, -1}, {1, -1}, {-1, 1}
  ]

  defp parse(input) do
    input
    |> String.split("\n", trim: true)
    |> Enum.with_index
    |> Enum.flat_map(fn {line, row} ->
      line
      |> String.graphemes
      |> Enum.with_index
      |> Enum.map(fn {char, col} -> {{row, col}, char} end)
    end)
    |> Map.new
  end

  def part1(input) do
    grid = parse(input)

    grid
    |> Enum.filter(fn {_, c} -> c == "X" end)
    |> Enum.reduce(0, fn {position, _}, count ->
      # Check in each direction
      @directions
        |> Enum.reduce(count, fn direction, count ->
          if matches_word?(["X", "M", "A", "S"], grid, position, direction), do: count + 1, else: count
        end)
    end)
  end

  defp matches_word?([], _, _, _) do
    true
  end
  defp matches_word?([c|word], grid, {px, py}, {dx, dy}) do
    case Map.get(grid, {px, py}, "#") do
      ^c ->
        matches_word?(word, grid, {px + dx, py + dy}, {dx, dy})
      _ ->
        false
    end
  end

  def part2(input) do
    grid = parse(input)
    grid
    |> Enum.filter(fn {_, c} -> c == "A" end)
    |> Enum.reduce(0, fn {{px, py}, _}, count ->
      diagonals = [{-1, -1}, {1, -1}, {-1, 1}, {1, 1}]
      |> Enum.map(fn {dx, dy} -> Map.get(grid, {px + dx, py + dy}, "#") end)

      if valid_diagonal?(diagonals), do: count + 1, else: count
    end)
  end

  defp valid_diagonal?([ul, ur, ll, lr]) do
    ([ul, lr] == ["S", "M"] or [ul, lr] == ["M", "S"]) and
    ([ur, ll] == ["S", "M"] or [ur, ll] == ["M", "S"])
  end

  def run_part1() do input() |> part1() end
  def run_part2() do input() |> part2() end
end
