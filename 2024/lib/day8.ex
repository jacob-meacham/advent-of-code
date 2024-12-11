defmodule Advent.Day8.AntinodeFinder do
  use Advent.Day, no: 8
  @moduledoc "Day 8"

  defp generate_pairs(l) do
    for x <- l, y <- l, x < y, do: {x, y}
  end

  defp parse(input) do
    line = input
    |> String.split("\n", trim: true)

    antennas = line
    |> Enum.with_index
    |> Enum.flat_map(fn {line, row} ->
      line
      |> String.graphemes
      |> Enum.with_index
      |> Enum.filter(fn {char, _} -> char != "." end)
      |> Enum.group_by(fn {char, _} -> char end, fn {_, col} -> {row, col} end)
    end)
    |> Enum.reduce(%{}, fn {k, v}, acc ->
      Map.merge(acc, %{k => v}, fn _key, val1, val2 -> val1 ++ val2 end)
    end)

    {length(line), antennas}
  end

  defp in_bounds?({x, y}, bounds) do
    cond do
      x < 0 -> false
      y < 0 -> false
      x >= bounds -> false
      y >= bounds -> false
      true -> true
    end
  end

  def part1(input) do
    {bounds, antennas} = parse(input)

    antennas
    |> Enum.flat_map(fn {name, positions} ->
      generate_pairs(positions)
      |> Enum.flat_map(fn {{x1, y1}, {x2, y2}} ->
        {dx, dy} = {x1 - x2, y1 - y2}
        [
          {name, {x1 - 2 * dx, y1 - 2 * dy}},
          {name, {x2 + 2 * dx, y2 + 2 * dy}}
        ]
        |> Enum.filter(fn {_name, pos} -> in_bounds?(pos, bounds) end)
      end)
    end)
    |> Enum.group_by(fn {_name, pos} -> pos end, fn {name, _pos} -> name end)
    |> Enum.count
  end

  defp generate_antinodes(name, {x, y}, {dx, dy}, bounds, acc \\ []) do
    next_pos = {x + dx, y + dy}

    if in_bounds?(next_pos, bounds) do
      generate_antinodes(name, next_pos, {dx, dy}, bounds, [{name, next_pos} | acc])
    else
      acc
    end
  end

  def part2(input) do
    {bounds, antennas} = parse(input)

    t = antennas
    |> Enum.flat_map(fn {name, positions} ->
      generate_pairs(positions)
      |> Enum.flat_map(fn {{x1, y1}, {x2, y2}} ->
        {dx, dy} = {x1 - x2, y1 - y2}

        generate_antinodes(name, {x1, y1}, {dx, dy}, bounds) ++
        generate_antinodes(name, {x2, y2}, {-dx, -dy}, bounds) ++ [{name, {x1, y1}}, {name, {x2, y2}}]
      end)
    end)
    |> Enum.group_by(fn {_name, pos} -> pos end, fn {name, _pos} -> name end)

    t
    |> Enum.count
  end

  def run_part1() do input() |> part1() end
  def run_part2() do input() |> part2() end
end
