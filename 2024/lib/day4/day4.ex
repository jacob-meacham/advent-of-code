defmodule WordSearch do
  @moduledoc "Day 4"

  defp parse(input) do
    input
    |> String.split("\n", trim: true)
    |> Enum.with_index
    |> Enum.flat_map(fn {line, row} ->
      String.split(line, "")
      |> Enum.with_index
      |> Enum.flat_map(fn {char, col} ->
        position = {row, col}
        [{position, char}]
      end)
    end)
    |> Map.new
  end

  defp directions do
    [
      {1, 0},
      {-1, 0},
      {0, 1},
      {0, -1},
      {1, 1},
      {-1, -1},
      {1, -1},
      {-1, 1}
    ]
  end

  def part1(input) do
    grid = parse(input)
    grid
    |> Enum.filter(fn {_, c} -> c == "X" end)
    |> Enum.reduce(0, fn {position, _}, count ->
      # Check in each direction
      directions()
        |> Enum.reduce(count, fn direction, count ->
          case matches_word?(["X", "M", "A", "S"], grid, position, direction) do
            true -> count + 1
            false -> count
          end
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
      [ul, ur, ll, lr] = [{-1, -1}, {1, -1}, {-1, 1}, {1, 1}] |> Enum.map(fn {dx, dy} -> Map.get(grid, {px + dx, py + dy}, "#") end)
      case ([ul, lr] == ["S", "M"] or [ul, lr] == ["M", "S"]) and
      ([ur, ll] == ["S", "M"] or [ur, ll] == ["M", "S"]) do
        true -> count + 1
        false -> count
      end
    end)
  end
end

{:ok, contents} = File.read("lib/day4/input.txt")
IO.puts(WordSearch.part1(contents))
IO.puts(WordSearch.part2(contents))


Benchee.run(%{
  day4_part1: fn -> contents |> WordSearch.part1() end,
  day4_part2: fn -> contents |> WordSearch.part2() end,
  day4_total: fn ->
    WordSearch.part1(contents)
    WordSearch.part2(contents)
  end}, warmup: 2,
time: 3)
